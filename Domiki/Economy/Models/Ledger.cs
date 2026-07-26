namespace Domiki.Web.Economy.Models;

/// <summary>
/// Счётная книга Избы старосты за текущие сутки.
/// </summary>
public class Ledger
{
    /// <summary>
    /// Наивысший уровень Избы старосты у игрока.
    /// </summary>
    /// <value><c>0</c> – избы нет, книга закрыта.</value>
    public int Level { get; set; }

    /// <summary>
    /// <see langword="true"/> – за сутки записан хоть один приход или расход.
    /// </summary>
    /// <remarks>
    /// <see langword="false"/> сразу после постройки избы: книга заведена, но считает только с этого часа.
    /// </remarks>
    public bool HasEntries { get; set; }

    /// <summary>
    /// Приход и расход по каждому тронутому за сутки ресурсу.
    /// </summary>
    public LedgerFlow[] Flows { get; set; } = [];

    /// <summary>
    /// Ресурс, который кончится первым при нынешнем расходе.
    /// </summary>
    /// <value><see langword="null"/> – ничего не убывает либо расход ещё не на чем считать.</value>
    public LedgerShortage? Shortage { get; set; }

    /// <summary>
    /// Доля суток, что производственные места двора простояли.
    /// </summary>
    /// <value>Проценты, 0–100. <see langword="null"/> – производственных построек нет, считать простой не от чего.</value>
    public int? IdlePercent { get; set; }
}

/// <summary>
/// Строка счётной книги по одному ресурсу.
/// </summary>
public class LedgerFlow
{
    /// <summary>
    /// Тип ресурса.
    /// </summary>
    public int ResourceTypeId { get; set; }

    /// <summary>
    /// Сколько единиц пришло за сутки.
    /// </summary>
    public int Gained { get; set; }

    /// <summary>
    /// Сколько единиц ушло за сутки.
    /// </summary>
    /// <value>Положительное число.</value>
    public int Spent { get; set; }
}

/// <summary>
/// Прогноз по ресурсу, который кончится первым.
/// </summary>
/// <remarks>
/// Прогноз линейный: остаток делится на средний расход за уже прошедшую часть суток.
/// Это ответ книги – количественный и про сутки; бинарное «глины нет» остаётся за бесплатной доской хозяйства.
/// </remarks>
public class LedgerShortage
{
    /// <summary>
    /// Тип ресурса, который кончится первым.
    /// </summary>
    public int ResourceTypeId { get; set; }

    /// <summary>
    /// На сколько часов хватит остатка при нынешнем расходе.
    /// </summary>
    public int Hours { get; set; }
}
