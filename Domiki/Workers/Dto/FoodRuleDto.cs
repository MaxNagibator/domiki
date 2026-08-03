namespace Domiki.Web.Workers.Dto;

/// <summary>
/// Правило кладовой Корчмы для одного типа съестного припаса.
/// </summary>
public sealed record FoodRuleDto
{
    /// <summary>
    /// Тип съестного припаса – ссылка на справочник <see cref="Reference.Dto.ResourceTypeDto.Id"/>.
    /// </summary>
    public required int ResourceTypeId { get; init; }

    /// <summary>
    /// Число единиц припаса, которое Корчма не трогает при автоподборе.
    /// </summary>
    public required int Reserve { get; init; }

    /// <summary>
    /// <see langword="true"/> – Корчма никогда не подаёт этот припас к столу.
    /// </summary>
    public required bool Forbidden { get; init; }

    /// <summary>
    /// Сколько единиц припаса съедено за текущие сутки.
    /// </summary>
    public required int EatenToday { get; init; }
}
