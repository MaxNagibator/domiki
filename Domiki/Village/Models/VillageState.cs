namespace Domiki.Web.Village.Models;

/// <summary>
/// Идентичность деревни – название и герб.
/// </summary>
/// <remarks>
/// Отдаётся на клиент как <see cref="Dto.VillageDto"/>.
/// </remarks>
public class VillageState
{
    /// <summary>
    /// Название деревни, выбранное игроком.
    /// </summary>
    public string? VillageName { get; set; }

    /// <summary>
    /// Индекс пиктограммы герба из готового набора.
    /// </summary>
    public int CrestIcon { get; set; }

    /// <summary>
    /// Индекс цвета герба из готового набора.
    /// </summary>
    public int CrestColor { get; set; }

    /// <summary>
    /// Сосед, чей уклад деревни принят игроком.
    /// </summary>
    /// <remarks>
    /// Ссылка на справочник соседей (см. <see cref="Reference.ResourceManager.GetNeighbors"/>). <see langword="null"/> – уклад не принят.
    /// </remarks>
    public int? ProfileNeighborId { get; set; }

    /// <summary>
    /// Момент, с которого уклад деревни можно сменить снова.
    /// </summary>
    /// <value>Момент в UTC.</value>
    /// <remarks>
    /// <see cref="Data.Entities.Player.ProfileChangedDate"/> плюс <see cref="VillageProfileManager.ProfileChangeCooldownDays"/> суток.
    /// <see langword="null"/> – уклад ещё ни разу не принимался.
    /// </remarks>
    public DateTime? ProfileChangeAvailableDate { get; set; }
}
