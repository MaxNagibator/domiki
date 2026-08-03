namespace Domiki.Web.Village.Models;

/// <summary>
/// Влияние уклада деревни на длительность производства в постройке специализации соседа.
/// </summary>
/// <remarks>
/// Модель-зеркало сущности <see cref="Data.Entities.VillageProfileEffect"/>, загружается целиком (см.
/// <see cref="Reference.ResourceManager.GetVillageProfileEffects"/>) и используется в <see cref="Core.DomikManager.StartManufacture"/>.
/// </remarks>
public class VillageProfileEffect
{
    /// <summary>
    /// Сосед, чей уклад описывает эффект – ссылка на <see cref="Economy.Models.Neighbor.Id"/>.
    /// </summary>
    public int NeighborId { get; set; }

    /// <summary>
    /// Тип построек специализации соседа – ссылка на <see cref="Core.Models.DomikType.Id"/>.
    /// </summary>
    public int DomikTypeId { get; set; }

    /// <summary>
    /// Процент длительности производства при принятом укладе этого соседа.
    /// </summary>
    /// <value>Проценты, где <c>100</c> – без изменений. Канонная величина уклада – <c>85</c> (см. GAMEDESIGN.md §3 Слой 4).</value>
    public int DurationPercent { get; set; }
}
