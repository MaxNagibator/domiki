using Microsoft.EntityFrameworkCore;

namespace Domiki.Web.Data.Entities;

/// <summary>
/// Купленная игроком ступень перка лесенки узелков памяти.
/// </summary>
/// <remarks>
/// Одна строка на перк: <see cref="Level"/> растёт с каждой купленной ступенью. Остаток нетраченых узелков лежит
/// в <see cref="Player.MemoryKnots"/> (см. <see cref="Village.PerkManager.BuyPerk"/>).
/// </remarks>
[PrimaryKey(nameof(PlayerId), nameof(PerkType))]
public class PlayerPerk
{
    /// <summary>
    /// Игрок-владелец перка.
    /// </summary>
    public int PlayerId { get; set; }

    /// <summary>
    /// Перк лесенки.
    /// </summary>
    public RelocationPerkType PerkType { get; set; }

    /// <summary>
    /// Число купленных ступеней перка.
    /// </summary>
    /// <remarks>
    /// Не больше числа ступеней перка в справочнике <see cref="Village.PerkManager.Perks"/>.
    /// </remarks>
    public int Level { get; set; }

    /// <summary>
    /// Навигационное свойство к игроку-владельцу.
    /// </summary>
    public Player Player { get; set; } = null!;
}
