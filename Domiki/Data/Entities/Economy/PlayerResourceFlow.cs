using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domiki.Web.Data.Entities;

/// <summary>
/// Суточный итог прихода и расхода одного ресурса у игрока – строка счётной книги Избы старосты.
/// </summary>
/// <remarks>
/// Пишется хуком в <see cref="Infrastructure.PlayerResourceManager.GrantResource"/> и
/// <see cref="Infrastructure.PlayerResourceManager.WriteOffResources"/>, поэтому ловит все источники разом:
/// смены, заказы, обоз, торг, походы. Строки заводятся только тем игрокам, у кого стоит Изба старосты
/// (<see cref="Economy.ElderHouseManager.CountingBookMinLevel"/>) – книга считает с часа постройки, а не задним числом.
/// Записи старше <see cref="Economy.ElderHouseManager.LedgerKeepDays"/> суток сметаются по дате.
/// </remarks>
[PrimaryKey(nameof(PlayerId), nameof(Date), nameof(ResourceTypeId))]
public class PlayerResourceFlow
{
    /// <summary>
    /// Часть составного ключа – игрок-владелец книги.
    /// </summary>
    [Column(Order = 1)]
    public int PlayerId { get; set; }

    /// <summary>
    /// Часть составного ключа – сутки в UTC, к которым отнесён итог.
    /// </summary>
    /// <value>Момент в UTC, время суток всегда 00:00.</value>
    [Column(Order = 2)]
    public DateTime Date { get; set; }

    /// <summary>
    /// Часть составного ключа – тип ресурса, ссылка на справочник <see cref="ResourceType"/>.
    /// </summary>
    [Column(Order = 3)]
    public int ResourceTypeId { get; set; }

    /// <summary>
    /// Сколько единиц ресурса пришло за сутки <see cref="Date"/>.
    /// </summary>
    public int Gained { get; set; }

    /// <summary>
    /// Сколько единиц ресурса ушло за сутки <see cref="Date"/>.
    /// </summary>
    /// <value>Положительное число: расход хранится модулем, знак ставится при показе.</value>
    public int Spent { get; set; }
}
