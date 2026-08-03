namespace Domiki.Web.Economy.Models;

/// <summary>
/// Заповедный припас: сколько единиц ресурса наряды не трогают.
/// </summary>
public class ResourceReserve
{
    /// <summary>
    /// Тип ресурса.
    /// </summary>
    public int ResourceTypeId { get; set; }

    /// <summary>
    /// Сколько единиц отложено от нарядов.
    /// </summary>
    public int Reserve { get; set; }
}
