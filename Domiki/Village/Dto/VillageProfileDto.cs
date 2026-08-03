namespace Domiki.Web.Village.Dto;

/// <summary>
/// Одна строка справочника уклада деревни: насколько сосед ускоряет производство в постройках своей специализации.
/// </summary>
public sealed record VillageProfileDto
{
    /// <summary>
    /// Сосед, чей уклад описывает строка – ссылка на <see cref="Economy.Dto.NeighborReputationDto.NeighborId"/>.
    /// </summary>
    public required int NeighborId { get; init; }

    /// <summary>
    /// Тип построек специализации соседа – ссылка на <see cref="Core.Dto.DomikTypeDto.Id"/>.
    /// </summary>
    public required int DomikTypeId { get; init; }

    /// <summary>
    /// Процент длительности производства при принятом укладе этого соседа.
    /// </summary>
    /// <value>Проценты, где <c>100</c> – без изменений. Канонная величина уклада – <c>85</c> (см. GAMEDESIGN.md §3 Слой 4).</value>
    public required int DurationPercent { get; init; }
}
