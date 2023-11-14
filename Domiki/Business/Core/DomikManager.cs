using Domiki.Web.Business.Models;

namespace Domiki.Web.Business.Core
{
    public class DomikManager
    {
        private Data.ApplicationDbContext _context;
        private ICalculator _calculator;
        private Data.UnitOfWork _uow;

        public DomikManager(Data.UnitOfWork uow, Data.ApplicationDbContext context, ICalculator calculator)
        {
            _context = context;
            _calculator = calculator;
            _uow = uow;
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
                    FinishDate = x.UpgradeSeconds == null ? null : (x.UpgradeCalculateDate.Value.AddSeconds((int)x.UpgradeSeconds))
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
                var domikType = GetDomikTypes().First(x => x.Id == typeId);
                var domikLevel = domikType.Levels.First(x => x.Value == 1);
                WriteOffResources(playerId, domikLevel.Resources);

                var currentId = _context.Domiks.Where(x => x.PlayerId == playerId).Max(x => (int?)x.Id) ?? 0;
                var nextId = currentId + 1;
                var date = DateTimeHelper.GetNowDate();
                _context.Domiks.Add(new Data.Domik { PlayerId = playerId, TypeId = typeId, Level = 0, Id = nextId, UpgradeSeconds = domikLevel.UpgradeSeconds, UpgradeCalculateDate = date });

                _uow.AfterEventAction = () =>
                {
                    _calculator.Insert(new CalculateInfo
                    {
                        PlayerId = playerId,
                        ObjectId = nextId,
                        Type = CalculateTypes.Domiks,
                        Date = date.AddSeconds(domikLevel.UpgradeSeconds),
                    });
                };
            }
            else
            {
                throw new BusinessException("Превышено максимальное количество");
            }
        }

        public void UpgradeDomik(int playerId, int id)
        {
            var date = DateTimeHelper.GetNowDate();

            // todo перечитать и попробовать повтоно выполнить. обработка оптимистика
            LockDbPlayerRow(playerId);

            var dbDomik = _context.Domiks.First(x => x.PlayerId == playerId && x.Id == id);
            var domikType = GetDomikTypes().First(x => x.Id == dbDomik.TypeId);
            if (dbDomik.Level >= domikType.MaxLevel)
            {
                throw new BusinessException("Максимальный уровень");
            }
            if (dbDomik.UpgradeSeconds != null)
            {
                throw new BusinessException("Домик уже улучшается");
            }

            var nextLevel = dbDomik.Level + 1;
            var domikLevel = domikType.Levels.First(x => x.Value == nextLevel);
            WriteOffResources(playerId, domikLevel.Resources);
            dbDomik.UpgradeSeconds = domikLevel.UpgradeSeconds;
            dbDomik.UpgradeCalculateDate = date;

            _uow.AfterEventAction = () =>
            {
                _calculator.Insert(new CalculateInfo
                {
                    PlayerId = playerId,
                    ObjectId = dbDomik.Id,
                    Type = CalculateTypes.Domiks,
                    Date = date.AddSeconds(domikLevel.UpgradeSeconds),
                });
            };
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

        // пессимистичная блокировка строки в БД, для борьбы с конкурентными потоками
        private void LockDbPlayerRow(int playerId)
        {
            _context.Players.First(x => x.Id == playerId).Version = Guid.NewGuid();
        }

        private void WriteOffResources(int playerId, Resource[] resources)
        {
            var dbResources = _context.Resources.Where(x => x.PlayerId == playerId).ToArray();
            foreach (var domikNeedResource in resources)
            {
                var dbResource = dbResources.FirstOrDefault(x => x.TypeId == domikNeedResource.Type.Id);
                if (dbResource == null)
                {
                    throw new BusinessException("Недостаточно " + domikNeedResource.Type);
                }
                dbResource.Value -= domikNeedResource.Value;
                if (dbResource.Value < 0)
                {
                    throw new BusinessException("Недостаточно " + domikNeedResource.Type);
                }
            }
        }

        public bool FinishDomik(DateTime date, CalculateInfo calcInfo)
        {
            var dbDomik = _context.Domiks.Single(x => x.Id == calcInfo.ObjectId && x.PlayerId == calcInfo.PlayerId);
            if (dbDomik.UpgradeSeconds != null)
            {
                var period = (date - (DateTime)dbDomik.UpgradeCalculateDate).TotalSeconds;
                var lostTime = dbDomik.UpgradeSeconds - period;
                if (lostTime <= 0)
                {
                    dbDomik.UpgradeCalculateDate = null;
                    dbDomik.UpgradeSeconds = null;
                    dbDomik.Level++;

                    return true;
                }
                return false;
            }
            return true;
        }
    }
}
