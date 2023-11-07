﻿using Domiki.Web.Business.Models;

namespace Domiki.Web.Models
{
    public static class UpgradeLevelDtoExtentions
    {
        public static UpgradeLevelDto ToDto(this UpgradeLevel t)
        {
            return new UpgradeLevelDto
            { 
                Value = t.Value,
                Resources = t.Resources.Select(x=>x.ToDto()).ToArray(),
            };
        }
    }
}