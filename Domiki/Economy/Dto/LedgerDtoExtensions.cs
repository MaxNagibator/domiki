using Domiki.Web.Economy.Models;

namespace Domiki.Web.Economy.Dto;

public static class LedgerDtoExtensions
{
    public static LedgerDto ToDto(this Ledger ledger)
    {
        return new()
        {
            Level = ledger.Level,
            HasEntries = ledger.HasEntries,
            Flows = ledger.Flows.Select(x => new LedgerFlowDto
                {
                    ResourceTypeId = x.ResourceTypeId,
                    Gained = x.Gained,
                    Spent = x.Spent,
                })
                .ToArray(),
            Shortage = ledger.Shortage == null
                ? null
                : new LedgerShortageDto
                {
                    ResourceTypeId = ledger.Shortage.ResourceTypeId,
                    Hours = ledger.Shortage.Hours,
                },
            IdlePercent = ledger.IdlePercent,
        };
    }
}
