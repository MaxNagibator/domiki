using Domiki.Web.Data;
using Domiki.Web.Data.Entities;
using Domiki.Web.Infrastructure;
using Domiki.Web.Reference;
using Domiki.Web.Workers.Models;
using Resource = Domiki.Web.Reference.Models.Resource;

namespace Domiki.Web.Workers;

/// <summary>
/// Правила Корчмы: её уровень, кладовая (правила подбора еды по припасам) и подбор еды из запасов игрока.
/// </summary>
public class TavernManager
{
    /// <summary>
    /// Уровень Корчмы, с которого она кормит уставших трудяг.
    /// </summary>
    public const int MealMinLevel = 1;

    /// <summary>
    /// Уровень Корчмы, с которого она автоматически собирает провиант в поход.
    /// </summary>
    public const int ProvisionMinLevel = 2;

    /// <summary>
    /// Уровень Корчмы, с которого открывается тёплый угол.
    /// </summary>
    public const int WarmCornerMinLevel = 3;

    /// <summary>
    /// Доля срока хвори, которую сокращает тёплый угол Корчмы.
    /// </summary>
    /// <value>Проценты.</value>
    public const int WarmCornerRecoveryPercent = 25;

    private readonly ApplicationDbContext _context;
    private readonly ResourceManager _resourceManager;
    private readonly PlayerResourceManager _playerResourceManager;

    /// <summary>
    /// Создаёт менеджер правил Корчмы.
    /// </summary>
    /// <param name="context">Контекст данных текущего запроса.</param>
    /// <param name="resourceManager">Кэш справочников игры.</param>
    /// <param name="playerResourceManager">Менеджер ресурсов и блокировки строки игрока.</param>
    public TavernManager(ApplicationDbContext context, ResourceManager resourceManager, PlayerResourceManager playerResourceManager)
    {
        _context = context;
        _resourceManager = resourceManager;
        _playerResourceManager = playerResourceManager;
    }

    /// <summary>
    /// Возвращает наивысший уровень Корчмы игрока.
    /// </summary>
    /// <param name="playerId">Идентификатор игрока.</param>
    /// <returns>Наивысший уровень Корчмы либо <c>0</c>, если её нет.</returns>
    public int GetLevel(int playerId)
    {
        var typeId = _resourceManager.GetDomikTypes().First(x => x.LogicName == "tavern").Id;
        return _context.Domiks
                   .Where(x => x.PlayerId == playerId && x.TypeId == typeId)
                   .Select(x => (int?)x.Level)
                   .Max()
               ?? 0;
    }

    /// <summary>
    /// Возвращает правила кладовой по всем съестным припасам справочника.
    /// </summary>
    /// <remarks>
    /// Для припасов без сохранённой строки возвращает значения по умолчанию, чтобы клиент никогда не выдумывал записи сам.
    /// </remarks>
    /// <param name="playerId">Идентификатор игрока.</param>
    /// <returns>Правило на каждый съестной тип ресурса.</returns>
    public FoodRule[] GetRules(int playerId)
    {
        var today = DateTimeHelper.GetNowDate().Date;
        var rules = GetRulesLookup(playerId);
        return _resourceManager.GetResourceTypes()
            .Where(x => x.IsFood)
            .Select(x =>
            {
                rules.TryGetValue(x.Id, out var rule);
                return new FoodRule
                {
                    ResourceTypeId = x.Id,
                    Reserve = rule?.Reserve ?? 0,
                    Forbidden = rule?.Forbidden ?? false,
                    EatenToday = rule != null && rule.EatenDate == today ? rule.EatenToday : 0,
                };
            })
            .OrderBy(x => x.ResourceTypeId)
            .ToArray();
    }

    /// <summary>
    /// Сохраняет правило кладовой для одного съестного припаса.
    /// </summary>
    /// <param name="playerId">Идентификатор игрока.</param>
    /// <param name="resourceTypeId">Тип ресурса – должен быть съестным.</param>
    /// <param name="reserve">Число единиц, которое Корчма не должна трогать при автоподборе.</param>
    /// <param name="forbidden"><see langword="true"/> – запретить Корчме подавать этот припас к столу.</param>
    public void SaveRule(int playerId, int resourceTypeId, int reserve, bool forbidden)
    {
        _playerResourceManager.LockDbPlayerRow(playerId);

        var type = _resourceManager.GetResourceTypes().FirstOrDefault(x => x.Id == resourceTypeId);
        if (type is not { IsFood: true })
        {
            throw new BusinessException("Этот припас к столу не подают");
        }

        if (reserve < 0)
        {
            throw new BusinessException("Запас не может быть отрицательным");
        }

        var rule = GetOrCreateRule(playerId, resourceTypeId);
        rule.Reserve = reserve;
        rule.Forbidden = forbidden;
    }

    /// <summary>
    /// Отмечает съеденную еду в счётчиках кладовой.
    /// </summary>
    /// <remarks>
    /// Счётчик <see cref="PlayerFoodRule.EatenToday"/> сбрасывается при смене суток.
    /// </remarks>
    /// <param name="playerId">Идентификатор игрока.</param>
    /// <param name="food">Съеденные ресурсы, набранные <see cref="CollectFood"/>.</param>
    public void RegisterMeal(int playerId, Resource[] food)
    {
        var today = DateTimeHelper.GetNowDate().Date;
        foreach (var eaten in food)
        {
            var rule = GetOrCreateRule(playerId, eaten.Type.Id);
            if (rule.EatenDate != today)
            {
                rule.EatenToday = 0;
            }

            rule.EatenToday += eaten.Value;
            rule.EatenDate = today;
        }
    }

    /// <summary>
    /// Подбирает еду из запасов игрока, начиная с наименее ценной на рынке, с учётом правил кладовой.
    /// </summary>
    /// <remarks>
    /// Метод только рассчитывает списание. Само списание выполняет <see cref="Infrastructure.PlayerResourceManager.WriteOffResources"/>.
    /// Запрещённые правилом <see cref="PlayerFoodRule.Forbidden"/> припасы не рассматриваются вовсе, а отложенные
    /// <see cref="PlayerFoodRule.Reserve"/> единицы остаются нетронутыми.
    /// </remarks>
    /// <param name="playerId">Идентификатор игрока.</param>
    /// <param name="count">Число нужных единиц еды.</param>
    /// <returns>Набор ресурсов для списания либо пустой массив, когда доступной еды недостаточно.</returns>
    public Resource[] CollectFood(int playerId, int count)
    {
        var stocks = GetStocks(playerId);
        var rules = GetRulesLookup(playerId);
        var foodTypes = _resourceManager.GetResourceTypes()
            .Where(x => x.IsFood && stocks.ContainsKey(x.Id))
            .OrderBy(x => ResourceManager.GetMarketValue(x.Id))
            .ThenBy(x => x.Id)
            .ToArray();
        var food = new List<Resource>();
        var remaining = count;
        foreach (var type in foodTypes)
        {
            rules.TryGetValue(type.Id, out var rule);
            if (rule is { Forbidden: true })
            {
                continue;
            }

            var available = Math.Max(0, stocks[type.Id] - (rule?.Reserve ?? 0));
            var value = Math.Min(available, remaining);
            if (value > 0)
            {
                food.Add(new() { Type = type, Value = value });
                remaining -= value;
            }

            if (remaining == 0)
            {
                return food.ToArray();
            }
        }

        return [];
    }

    /// <summary>
    /// Проверяет, вызвана ли нехватка еды правилами кладовой, а не пустым складом.
    /// </summary>
    /// <param name="playerId">Идентификатор игрока.</param>
    /// <returns><see langword="true"/> – на складе есть съестной припас, но он весь отложен про запас или запрещён.</returns>
    public bool HasForbiddenOrReservedFood(int playerId)
    {
        return GetFoodStock(playerId) > 0 && CollectFood(playerId, 1).Length == 0;
    }

    /// <summary>
    /// Возвращает сырой суммарный запас всех съестных припасов на складе, без учёта правил кладовой.
    /// </summary>
    /// <param name="playerId">Идентификатор игрока.</param>
    /// <returns>Сумма значений всех съестных ресурсов на складе.</returns>
    public int GetFoodStock(int playerId)
    {
        var stocks = GetStocks(playerId);
        return _resourceManager.GetResourceTypes()
            .Where(x => x.IsFood)
            .Sum(x => stocks.GetValueOrDefault(x.Id));
    }

    private Dictionary<int, int> GetStocks(int playerId)
    {
        return _context.Resources.Where(x => x.PlayerId == playerId).ToArray()
            .Union(_context.Resources.Local.Where(x => x.PlayerId == playerId))
            .Where(x => x.Value > 0)
            .ToDictionary(x => x.TypeId, x => x.Value);
    }

    private Dictionary<int, PlayerFoodRule> GetRulesLookup(int playerId)
    {
        return _context.PlayerFoodRules.Where(x => x.PlayerId == playerId).ToArray()
            .Union(_context.PlayerFoodRules.Local.Where(x => x.PlayerId == playerId))
            .ToDictionary(x => x.ResourceTypeId);
    }

    private PlayerFoodRule GetOrCreateRule(int playerId, int resourceTypeId)
    {
        var rule = _context.PlayerFoodRules.Local.FirstOrDefault(x => x.PlayerId == playerId && x.ResourceTypeId == resourceTypeId)
                   ?? _context.PlayerFoodRules.FirstOrDefault(x => x.PlayerId == playerId && x.ResourceTypeId == resourceTypeId);

        if (rule == null)
        {
            rule = new() { PlayerId = playerId, ResourceTypeId = resourceTypeId };
            _context.PlayerFoodRules.Add(rule);
        }

        return rule;
    }
}
