namespace Domiki.Web.Workers.Models;

/// <summary>
/// Правило кладовой Корчмы для одного типа съестного припаса.
/// </summary>
/// <remarks>
/// Собирается в <see cref="Workers.TavernManager.GetRules"/> и отдаётся на клиент как <see cref="Dto.FoodRuleDto"/>.
/// </remarks>
public class FoodRule
{
    /// <summary>
    /// Тип съестного припаса – ссылка на справочник <see cref="Reference.Models.ResourceType.Id"/>.
    /// </summary>
    public required int ResourceTypeId { get; set; }

    /// <summary>
    /// Число единиц припаса, которое Корчма не трогает при автоподборе.
    /// </summary>
    public required int Reserve { get; set; }

    /// <summary>
    /// <see langword="true"/> – Корчма никогда не подаёт этот припас к столу.
    /// </summary>
    public required bool Forbidden { get; set; }

    /// <summary>
    /// Сколько единиц припаса съедено за текущие сутки.
    /// </summary>
    public required int EatenToday { get; set; }
}
