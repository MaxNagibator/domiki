using Domiki.Web.Village.Models;

namespace Domiki.Web.Village.Dto;

public static class VillageProfileDtoExtensions
{
    public static VillageProfileDto ToDto(this VillageProfileEffect effect)
    {
        return new()
        {
            NeighborId = effect.NeighborId,
            DomikTypeId = effect.DomikTypeId,
            DurationPercent = effect.DurationPercent,
        };
    }
}
