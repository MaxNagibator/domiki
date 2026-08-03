using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domiki.Web.Data.Entities;

/// <summary>
/// Суточный счётчик отработанных секунд производственных мест игрока – основа доли простоя двора.
/// </summary>
/// <remarks>
/// Пишется на завершении смены (<see cref="Core.DomikManager.FinishManufacture"/>): смена, занявшая одно место
/// на N секунд, добавляет N секунд тем суткам, на которые пришлась, – смену на границе суток счётчик делит между ними.
/// Ёмкость двора для доли простоя считается при чтении по постройкам, а не хранится (см. <see cref="Economy.ElderHouseManager"/>).
/// Записи старше <see cref="Economy.ElderHouseManager.LedgerKeepDays"/> суток сметаются по дате.
/// </remarks>
[PrimaryKey(nameof(PlayerId), nameof(Date))]
public class PlayerLaborDay
{
    /// <summary>
    /// Часть составного ключа – игрок-владелец счётчика.
    /// </summary>
    [Column(Order = 1)]
    public int PlayerId { get; set; }

    /// <summary>
    /// Часть составного ключа – сутки в UTC, к которым отнесены секунды.
    /// </summary>
    /// <value>Момент в UTC, время суток всегда 00:00.</value>
    [Column(Order = 2)]
    public DateTime Date { get; set; }

    /// <summary>
    /// Сколько секунд производственные места были заняты сменами за сутки <see cref="Date"/>.
    /// </summary>
    public long WorkedSeconds { get; set; }
}
