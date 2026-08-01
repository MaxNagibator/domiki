using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domiki.Web.Data.Entities;

/// <summary>
/// Строка памятного столба – деревня, которую игрок оставил при переезде в новую долину.
/// </summary>
/// <remarks>
/// Записывается один раз в <see cref="Village.RelocationManager.Relocate"/> и больше не меняется: столб копит только
/// монотонное (GAMEDESIGN.md §3.7). Имя деревни живёт здесь, а не в <see cref="Player.VillageName"/>, – уникальный
/// индекс имени не даёт хранить прожитые имена рядом с действующим.
/// </remarks>
public class VillageChronicle
{
    /// <summary>
    /// Идентификатор записи.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Игрок, чью деревню помнит запись.
    /// </summary>
    public int PlayerId { get; set; }

    /// <summary>
    /// Имя прожитой деревни на день отъезда.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> – деревня так и осталась безымянной.
    /// </remarks>
    [MaxLength(100)]
    public string? VillageName { get; set; }

    /// <summary>
    /// Индекс пиктограммы герба прожитой деревни.
    /// </summary>
    public int CrestIcon { get; set; }

    /// <summary>
    /// Индекс цвета герба прожитой деревни.
    /// </summary>
    public int CrestColor { get; set; }

    /// <summary>
    /// Долина, в которой стояла деревня, – ссылка на <see cref="Village.RelocationValleys"/>.
    /// </summary>
    /// <value><c>0</c> – стартовая долина, с которой начинается игра.</value>
    public int ValleyId { get; set; }

    /// <summary>
    /// Обжитость деревни на день отъезда.
    /// </summary>
    public int VillageLevel { get; set; }

    /// <summary>
    /// Узелки памяти, начисленные за эту деревню.
    /// </summary>
    /// <remarks>
    /// Считаются в <see cref="Village.RelocationManager.ComputeKnots"/> в момент переезда.
    /// </remarks>
    public int Knots { get; set; }

    /// <summary>
    /// Момент, с которого деревня начала жить.
    /// </summary>
    /// <value>Момент в UTC.</value>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Момент отъезда из деревни.
    /// </summary>
    /// <value>Момент в UTC.</value>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Навигационное свойство к игроку-владельцу.
    /// </summary>
    public Player Player { get; set; } = null!;
}
