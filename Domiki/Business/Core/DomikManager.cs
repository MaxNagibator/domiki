using Domiki.Web.Business.Models;
using Microsoft.EntityFrameworkCore;

namespace Domiki.Web.Business.Core
{
    public class DomikManager
    {
        private Data.ApplicationDbContext _context;

        private static List<DomikType> DomikTypes = new List<DomikType>
            {
                new DomikType { Id = 1, Name = "Кузница", LogicName = "forge", MaxCount = 1, MaxLevel = 10 },
                new DomikType { Id = 2, Name = "Барак", LogicName = "barracks", MaxCount = 5, MaxLevel = 5 },

            };

        public DomikManager(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public void UpgradeDomik(int id)
        {
            var domik = _context.Domiks.First(x => x.Id == id);
            var domikType = DomikTypes.First(x => x.Id == domik.TypeId);
            if (domik.Level < domikType.MaxLevel)
            {
                domik.Level++;
                _context.SaveChanges();
            }
            else
            {
                throw new BusinessException("Максимальный уровень");
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

        public IEnumerable<DomikType> GetPurchaseAvailableDomiks(int playerId)
        {
            var available = new List<DomikType>();
            var domiks = GetDomiks(playerId);
            foreach (var domikType in DomikTypes)
            {
                var current = domiks.Count(x => x.Type.Id == domikType.Id);
                if (current < domikType.MaxCount)
                {
                    available.Add(domikType);
                }
            }
            return available;
        }

        public IEnumerable<Domik> GetDomiks(int playerId)
        {
            return _context.Domiks.Where(x => x.PlayerId == playerId).ToArray().Select(x =>
                new Domik
                {
                    Id = x.Id,
                    Type = DomikTypes.First(y => y.Id == x.TypeId),
                    Level = x.Level,
                    PlayerId = x.PlayerId,
                }).ToList();
        }

        public IEnumerable<DomikType> GetDomikTypes()
        {
            return DomikTypes;
        }

        public void BuyDomik(int playerId, int typeId)
        {
            _context.Players.First(x => x.Id == playerId).Version = Guid.NewGuid();

            var available = GetPurchaseAvailableDomiks(playerId);
            if (available.Any(x => x.Id == typeId))
            {
                var currentId = _context.Domiks.Where(x => x.PlayerId == playerId).Max(x => (int?)x.Id) ?? 0;
                var nextId = currentId + 1;
                _context.Domiks.Add(new Data.Domik { PlayerId = playerId, TypeId = typeId, Level = 1, Id = nextId });
            }
            else
            {
                throw new BusinessException("Превышено максимальное количество");
            }
        }
    }
}
