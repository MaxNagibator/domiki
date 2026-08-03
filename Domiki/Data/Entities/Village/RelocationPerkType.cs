namespace Domiki.Web.Data.Entities;

/// <summary>
/// Перк лесенки узелков памяти (<see cref="PlayerPerk.PerkType"/>).
/// </summary>
/// <remarks>
/// Цены ступеней и величина эффекта живут в справочнике <see cref="Village.PerkManager.Perks"/>, а не в БД: добавление
/// перка – новая строка справочника и новый член этого перечисления, схема не меняется.
/// </remarks>
public enum RelocationPerkType
{
    /// <summary>
    /// Значение не задано.
    /// </summary>
    None = 0,

    /// <summary>
    /// Подъёмные – монеты в казну новой деревни за каждую ступень.
    /// </summary>
    Liftings = 1,

    /// <summary>
    /// Долгая привычка – сокращение времени всех производств за каждую ступень.
    /// </summary>
    LongHabit = 2,

    /// <summary>
    /// Запасная койка – лишний житель деревни за каждую ступень.
    /// </summary>
    SpareBunk = 3,
}
