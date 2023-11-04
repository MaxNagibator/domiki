using Domiki.Web.Business.Models;

namespace Domiki.Web.Business
{
    public class StaticEntities
    {
        public static List<DomikType> DomikTypes = new List<DomikType>
            {
                new DomikType { Id = 1, Name = "Кузница", LogicName = "forge", MaxCount = 1, MaxLevel = 10 },
                new DomikType { Id = 2, Name = "Барак", LogicName = "barracks", MaxCount = 5, MaxLevel = 5 },
            };

        public static List<ResourceType> ResourceTypes = new List<ResourceType>
            {
                new ResourceType { Id = 1, Name = "Золото", LogicName = "gold" },
                new ResourceType { Id = 2, Name = "Камень", LogicName = "stone" },
            };
    }
}
