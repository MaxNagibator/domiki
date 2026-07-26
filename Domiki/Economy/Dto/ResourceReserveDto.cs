using Domiki.Web.Economy.Models;

namespace Domiki.Web.Economy.Dto;

/// <summary>
/// Заповедный припас: сколько единиц ресурса наряды не трогают.
/// </summary>
public sealed record ResourceReserveDto
{
    /// <summary>
    /// Тип ресурса.
    /// </summary>
    public required int ResourceTypeId { get; init; }

    /// <summary>
    /// Сколько единиц отложено от нарядов.
    /// </summary>
    public required int Reserve { get; init; }
}

public static class ResourceReserveDtoExtensions
{
    public static ResourceReserveDto ToDto(this ResourceReserve reserve)
    {
        return new()
        {
            ResourceTypeId = reserve.ResourceTypeId,
            Reserve = reserve.Reserve,
        };
    }
}
