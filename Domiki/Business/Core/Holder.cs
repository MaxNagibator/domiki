using Domiki.Business.Models;

namespace Domiki.Business.Core
{
    public class Holder
    {
        public List<Domik> Domiki { get; set; }
        public List<DomikType> DomikTypes { get; set; }
        private Data.ApplicationDbContext _context;

        public Holder(Data.ApplicationDbContext context)
        {
            _context = context;

            Init();
        }

        private void Init()
        {
            var dbDomik = _context.Domiks.FirstOrDefault();
            if (dbDomik == null)
            {
                _context.Domiks.Add(new Data.Domik { Id = 1, PlayerId = 217, Level = 1, TypeId = 1 });
                _context.Domiks.Add(new Data.Domik { Id = 2, PlayerId = 217, Level = 1, TypeId = 2 });
                _context.SaveChanges();
            }
            DomikTypes = new List<DomikType>
            {
                new DomikType { Id = 1, Name = "Кузница", LogicName = "forge" },
                new DomikType { Id = 2, Name = "Барак", LogicName = "barracks" },
            };

            Domiki = _context.Domiks.ToArray().Select(x =>
                new Domik
                {
                    Id = x.Id,
                    Type = DomikTypes.First(y => y.Id == x.TypeId),
                    Level = x.Level,
                    PlayerId = x.PlayerId,
                }).ToList();
        }

        public void UpgradeModik(int id)
        {
            var domik = Domiki.First(x => x.Id == id);
            if (domik.Level < 10)
            {
                domik.Level++;
                _context.Domiks.First(x => x.Id == id).Level = domik.Level;
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("max level");
            }
        }

        public int GetPlayerId(string aspNetUserId)
        {
            var dbPlayer = _context.Players.FirstOrDefault(x => x.AspNetUserId == aspNetUserId);
            if (dbPlayer == null)
            {
                dbPlayer = new Data.Player();
                dbPlayer.AspNetUserId = aspNetUserId;
                dbPlayer.Name = "Держатель домиков";
                _context.Players.Add(dbPlayer);
                _context.SaveChanges();
            }
            return dbPlayer.Id;
        }
    }
}
