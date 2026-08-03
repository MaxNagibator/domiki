using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domiki.Web.Data.Entities;

/// <summary>
/// Заповедный припас игрока: сколько единиц ресурса наряды не имеют права трогать.
/// </summary>
/// <remarks>
/// Открывается уровнем <see cref="Economy.ElderHouseManager.ReserveMinLevel"/> Избы старосты.
/// Проверяется только при возобновлении наряда (<see cref="Core.DomikManager.FinishManufacture"/>) – запуск смены руками
/// заповедь не связывает: игрок волен потратить отложенное сам.
/// </remarks>
[PrimaryKey(nameof(PlayerId), nameof(ResourceTypeId))]
public class PlayerResourceReserve
{
    /// <summary>
    /// Часть составного ключа – игрок-владелец заповеди.
    /// </summary>
    [Column(Order = 1)]
    public int PlayerId { get; set; }

    /// <summary>
    /// Часть составного ключа – тип ресурса, ссылка на справочник <see cref="ResourceType"/>.
    /// </summary>
    [Column(Order = 2)]
    public int ResourceTypeId { get; set; }

    /// <summary>
    /// Сколько единиц ресурса заповедано от нарядов.
    /// </summary>
    /// <value><c>0</c> – заповеди нет.</value>
    public int Reserve { get; set; }
}
