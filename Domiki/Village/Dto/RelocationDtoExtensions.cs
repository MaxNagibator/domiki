using Domiki.Web.Infrastructure;
using Domiki.Web.Village.Models;

namespace Domiki.Web.Village.Dto;

public static class RelocationDtoExtensions
{
    public static RelocationDto ToDto(this Relocation relocation)
    {
        return new()
        {
            Threshold = relocation.Threshold,
            Level = relocation.Level,
            EstimatedDays = relocation.EstimatedDays,
            CooldownUntil = DateTimeHelper.AsUtc(relocation.CooldownUntil),
            CanRelocate = relocation.CanRelocate,
            BlockReason = relocation.BlockReason,
            Knots = relocation.Knots,
            RelocationCount = relocation.RelocationCount,
            ValleyId = relocation.ValleyId,
            ValleyName = relocation.ValleyName,
            Perks = relocation.Perks.Select(x => x.ToDto()).ToArray(),
        };
    }

    public static RelocationPlanDto ToDto(this RelocationPlan plan)
    {
        return new()
        {
            KnotsOnRelocate = plan.KnotsOnRelocate,
            Summary = plan.Summary.ToDto(),
            Valleys = plan.Valleys.Select(x => x.ToDto()).ToArray(),
        };
    }

    public static RelocationSummaryDto ToDto(this RelocationSummary summary)
    {
        return new()
        {
            Workers = summary.Workers,
            Blueprints = summary.Blueprints,
            Gold = summary.Gold,
            GoldTotal = summary.GoldTotal,
            Coins = summary.Coins,
            Resources = summary.Resources,
            Buildings = summary.Buildings,
            StartingCoins = summary.StartingCoins,
        };
    }

    public static PerkDto ToDto(this Perk perk)
    {
        return new()
        {
            PerkType = (int)perk.Type,
            Name = perk.Name,
            Description = perk.Description,
            Costs = perk.Costs,
            Level = perk.Level,
        };
    }

    public static ValleyDto ToDto(this Valley valley)
    {
        return new()
        {
            Id = valley.Id,
            Name = valley.Name,
            LogicName = valley.LogicName,
            Description = valley.Description,
        };
    }

    public static MemorialPostDto ToDto(this MemorialPost post)
    {
        return new()
        {
            Villages = post.Villages.Select(x => x.ToDto()).ToArray(),
            LevelSum = post.LevelSum,
            RelocationCount = post.RelocationCount,
            FirstDayDate = DateTimeHelper.AsUtc(post.FirstDayDate),
        };
    }

    public static MemorialVillageDto ToDto(this MemorialVillage village)
    {
        return new()
        {
            VillageName = village.VillageName,
            CrestIcon = village.CrestIcon,
            CrestColor = village.CrestColor,
            ValleyId = village.ValleyId,
            ValleyName = village.ValleyName,
            Level = village.Level,
            Knots = village.Knots,
            LivedDays = village.LivedDays,
            Date = DateTimeHelper.AsUtc(village.Date),
        };
    }
}
