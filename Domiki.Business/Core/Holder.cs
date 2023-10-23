using Domiki.Business.Models;

namespace Domiki.Business.Core
{
    public class Holder
    {
        public List<Domik> Domiki { get; set; }
        public List<DomikType> DomikTypes { get; set; }

        public Holder()
        {
            DomikTypes = new List<DomikType>
            { 
                new DomikType { Id = 1, Name = "Кузница", LogicName = "forge" },
                new DomikType { Id = 2, Name = "Барак", LogicName = "barracks" },
            };

            Domiki = new List<Domik>
            {
                new Domik { Type = DomikTypes[0], Level = 1 },
                new Domik { Type = DomikTypes[1], Level = 1 },
            };
        }
    }
}
