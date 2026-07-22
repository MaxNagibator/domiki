using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domiki.Web.Data.Entities;

/// <summary>
/// Справочник уклада деревни: насколько сосед ускоряет производство в постройках своей специализации.
/// </summary>
[PrimaryKey(nameof(NeighborId), nameof(DomikTypeId))]
public class VillageProfileEffect
{
    /// <summary>
    /// Часть составного ключа – сосед, чей уклад описывает строка.
    /// </summary>
    [Column(Order = 1)]
    public int NeighborId { get; set; }

    /// <summary>
    /// Часть составного ключа – тип домика специализации соседа.
    /// </summary>
    [Column(Order = 2)]
    public int DomikTypeId { get; set; }

    /// <summary>
    /// Процент длительности производства при принятом укладе этого соседа.
    /// </summary>
    /// <value>Проценты, где <c>100</c> – без изменений, меньше <c>100</c> – ускорение (см. GAMEDESIGN.md §3 Слой 4).</value>
    public int DurationPercent { get; set; }

    /// <summary>
    /// Навигационное свойство к соседу.
    /// </summary>
    public Neighbor Neighbor { get; set; } = null!;
}
