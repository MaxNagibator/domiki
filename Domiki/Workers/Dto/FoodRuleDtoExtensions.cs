using Domiki.Web.Workers.Models;

namespace Domiki.Web.Workers.Dto;

public static class FoodRuleDtoExtensions
{
    public static FoodRuleDto ToDto(this FoodRule rule)
    {
        return new()
        {
            ResourceTypeId = rule.ResourceTypeId,
            Reserve = rule.Reserve,
            Forbidden = rule.Forbidden,
            EatenToday = rule.EatenToday,
        };
    }

    public static TavernLarderDto ToDto(this FoodRule[] rules)
    {
        return new()
        {
            Rules = rules.Select(x => x.ToDto()).ToArray(),
        };
    }
}
