﻿using Domiki.Web.Business.Models;

namespace Domiki.Web.Models
{
    public static class DomikTypeDtoExtentions
    {
        public static DomikTypeDto ToDto(this DomikType t)
        {
            return new DomikTypeDto { Id = t.Id, Name = t.Name, LogicName = t.LogicName, MaxCount = t.MaxCount, MaxLevel = t.MaxLevel };
        }
    }
}