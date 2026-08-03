namespace Domiki.Web.Economy.Dto;

/// <summary>
/// Счётная книга Избы старосты за текущие сутки.
/// </summary>
public sealed record LedgerDto
{
    /// <summary>
    /// Наивысший уровень Избы старосты у игрока.
    /// </summary>
    public required int Level { get; init; }

    /// <summary>
    /// <see langword="true"/> – за сутки записан хоть один приход или расход.
    /// </summary>
    public required bool HasEntries { get; init; }

    /// <summary>
    /// Приход и расход по каждому тронутому за сутки ресурсу.
    /// </summary>
    public required LedgerFlowDto[] Flows { get; init; }

    /// <summary>
    /// Ресурс, который кончится первым при нынешнем расходе.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> – ничего не убывает либо расход ещё не на чем считать
    /// (см. <see cref="ElderHouseManager"/>).
    /// </remarks>
    public LedgerShortageDto? Shortage { get; init; }

    /// <summary>
    /// Доля суток, что производственные места двора простояли.
    /// </summary>
    /// <value>Проценты, 0–100. <see langword="null"/> – производственных построек нет.</value>
    public int? IdlePercent { get; init; }
}

/// <summary>
/// Строка счётной книги по одному ресурсу.
/// </summary>
public sealed record LedgerFlowDto
{
    /// <summary>
    /// Тип ресурса.
    /// </summary>
    public required int ResourceTypeId { get; init; }

    /// <summary>
    /// Сколько единиц пришло за сутки.
    /// </summary>
    public required int Gained { get; init; }

    /// <summary>
    /// Сколько единиц ушло за сутки.
    /// </summary>
    /// <value>Положительное число: знак ставит клиент.</value>
    public required int Spent { get; init; }
}

/// <summary>
/// Прогноз по ресурсу, который кончится первым.
/// </summary>
public sealed record LedgerShortageDto
{
    /// <summary>
    /// Тип ресурса, который кончится первым.
    /// </summary>
    public required int ResourceTypeId { get; init; }

    /// <summary>
    /// На сколько часов хватит остатка при нынешнем расходе.
    /// </summary>
    /// <value><c>0</c> – припас уже на исходе.</value>
    public required int Hours { get; init; }
}
