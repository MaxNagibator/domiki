using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domiki.Web.Data.Entities;

/// <summary>
/// Правило кладовой Корчмы игрока для одного типа съестного припаса.
/// </summary>
/// <remarks>
/// Ограничивает автоподбор еды <see cref="Workers.TavernManager.CollectFood"/>: часть запаса можно отложить про запас
/// (<see cref="Reserve"/>) или вовсе запретить подавать к столу (<see cref="Forbidden"/>).
/// </remarks>
[PrimaryKey(nameof(PlayerId), nameof(ResourceTypeId))]
public class PlayerFoodRule
{
    /// <summary>
    /// Часть составного ключа – игрок-владелец правила.
    /// </summary>
    [Column(Order = 1)]
    public int PlayerId { get; set; }

    /// <summary>
    /// Часть составного ключа – тип съестного припаса, ссылка на справочник <see cref="ResourceType"/>.
    /// </summary>
    [Column(Order = 2)]
    public int ResourceTypeId { get; set; }

    /// <summary>
    /// Число единиц припаса, которое Корчма не трогает при автоподборе.
    /// </summary>
    public int Reserve { get; set; }

    /// <summary>
    /// <see langword="true"/> – Корчма никогда не подаёт этот припас к столу.
    /// </summary>
    public bool Forbidden { get; set; }

    /// <summary>
    /// Сколько единиц припаса съедено за сутки <see cref="EatenDate"/>.
    /// </summary>
    public int EatenToday { get; set; }

    /// <summary>
    /// Сутки в UTC, к которым относится счётчик <see cref="EatenToday"/>.
    /// </summary>
    /// <value>Момент в UTC, время суток всегда 00:00.</value>
    /// <remarks>
    /// <see langword="null"/> – счётчик ещё не заводился.
    /// </remarks>
    public DateTime? EatenDate { get; set; }
}
