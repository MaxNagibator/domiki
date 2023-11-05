using Domiki.Web.Business.Models;

namespace Domiki.Web.Business.Core
{
    public class DomikManager
    {
        private Data.ApplicationDbContext _context;

        public DomikManager(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public void UpgradeDomik(int id)
        {
            var domik = _context.Domiks.First(x => x.Id == id);
            var domikType = StaticEntities.DomikTypes.First(x => x.Id == domik.TypeId);
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

                foreach (var resourceType in StaticEntities.ResourceTypes)
                {
                    _context.Resources.Add(new Data.Resource { TypeId = resourceType.Id, Player = dbPlayer, Value = 1000 });
                }

                _context.SaveChanges();
            }
            return dbPlayer.Id;
        }

        public IEnumerable<DomikType> GetPurchaseAvailableDomiks(int playerId)
        {
            var available = new List<DomikType>();
            var domiks = GetDomiks(playerId);
            foreach (var domikType in StaticEntities.DomikTypes)
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
                    Type = StaticEntities.DomikTypes.First(y => y.Id == x.TypeId),
                    Level = x.Level,
                }).ToList();
        }

        public IEnumerable<DomikType> GetDomikTypes()
        {
            return StaticEntities.DomikTypes;
        }

        public void BuyDomik(int playerId, int typeId)
        {

            // todo покупать домики за ресурсики
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

        public IEnumerable<Resource> GetResources(int playerId)
        {
            return _context.Resources.Where(x => x.PlayerId == playerId).ToArray().Select(x =>
                new Resource
                {
                    Type = StaticEntities.ResourceTypes.First(y => y.Id == x.TypeId),
                    Value = x.Value,
                }).ToList();
        }

        public IEnumerable<ResourceType> GetResourceTypes()
        {
            return StaticEntities.ResourceTypes;
        }
    }
}
