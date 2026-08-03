namespace Domiki.Web.Workers.Dto;

/// <summary>
/// Кладовая Корчмы игрока: правила подбора еды по каждому съестному припасу.
/// </summary>
public sealed record TavernLarderDto
{
    /// <summary>
    /// Правила по всем съестным припасам справочника.
    /// </summary>
    public required FoodRuleDto[] Rules { get; init; }
}
