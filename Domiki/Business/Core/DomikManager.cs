using Domiki.Web.Business.Models;
using Microsoft.EntityFrameworkCore;

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
                 _context.Resources.Add(new Data.Resource { TypeId = 1, Player = dbPlayer, Value = 1000 });

                _context.SaveChanges();
            }
            return dbPlayer.Id;
        }

        public IEnumerable<DomikType> GetPurchaseAvailableDomiks(int playerId)
        {
            var available = new List<DomikType>();
            var domiks = GetDomiks(playerId);
            foreach (var domikType in GetDomikTypes())
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
            var manufactureGroups = _context.Manufactures.Where(x => x.DomikPlayerId == playerId)
                .ToArray().GroupBy(x => x.DomikId);
            var domikTypes = GetDomikTypes();
            return _context.Domiks.Where(x => x.PlayerId == playerId).ToArray().Select(domik =>
                new Domik
                {
                    Id = domik.Id,
                    Type = domikTypes.First(y => y.Id == domik.TypeId),
                    Level = domik.Level,
                    FinishDate = domik.UpgradeSeconds == null ? null : (domik.UpgradeCalculateDate.Value.AddSeconds((int)domik.UpgradeSeconds)),
                    Manufactures = manufactureGroups.FirstOrDefault(m => m.Key == domik.Id)?.Select(x => new Manufacture
                    {
                        Id = x.Id,
                        FinishDate = x.FinishDate,
                        PlodderCount = x.PlodderCount,
                        ReceiptId = x.ReceiptId,
                    }).ToArray(),
                }).ToList();
        }

        public IEnumerable<DomikType> GetDomikTypes()
        {
            var modificators = _context.DomikTypeLevelModificators.ToArray();
            var recepts = _context.DomikTypeLevelRecepts.ToArray();
            var resources = _context.DomikTypeLevelResources.ToArray();
            var domikTypes = _context.DomikTypes.Include(x => x.Levels).ToArray().Select(domikType => new DomikType
            {
                Id = domikType.Id,
                LogicName = domikType.LogicName,
                Name = domikType.Name,
                MaxCount = domikType.MaxCount,
                Levels = domikType.Levels.Select(level => new UpgradeLevel
                {
                    Value = level.Value,
                    UpgradeSeconds = level.UpgradeSeconds,
                    Modificators = modificators
                        .Where(m => m.DomikTypeLevelDomikTypeId == domikType.Id
                            && m.DomikTypeLevelValue == level.Value)
                        .Select(x => new Modificator { Type = new ModificatorType { Id = x.ModificatorTypeId } }).ToArray(),
                    Receipts = recepts
                        .Where(m => m.DomikTypeLevelDomikTypeId == domikType.Id
                            && m.DomikTypeLevelValue == level.Value)
                        .Select(x => new Receipt { Id = x.ReceiptId }).ToArray(),
                    Resources = resources
                        .Where(m => m.DomikTypeLevelDomikTypeId == domikType.Id
                            && m.DomikTypeLevelValue == level.Value)
                        .Select(x => new Resource { Type = new ResourceType { Id = x.ResourceTypeId }, Value = x.Value }).ToArray(),
                }).ToArray(),
            }).ToArray();
            return domikTypes;
        }

        public IEnumerable<ModificatorType> GetModificatorTypes()
        {
            var modificatorTypes = _context.ModificatorTypes.Select(x => new ModificatorType
            {
                Id = x.Id,
                LogicName = x.LogicName,
                Name = x.Name,
            }).ToArray();
            return modificatorTypes;
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
                    Type = GetResourceTypes().First(y => y.Id == x.TypeId),
                    Value = x.Value,
                }).ToList();
        }

        public IEnumerable<ResourceType> GetResourceTypes()
        {
            return _context.ResourceTypes
                .Select(x => new ResourceType { Id = x.Id, LogicName = x.LogicName, Name = x.Name }).ToArray();
        }

        public IEnumerable<Receipt> GetReceipts()
        {
            var receipts = _context.Receipts.Include(x => x.Resources).Select(x => new Receipt
            {
                Id = x.Id,
                LogicName = x.LogicName,
                Name = x.Name,
                PlodderCount = x.PlodderCount,
                DurationsSeconds = x.DurationsSeconds,
                InputResources = x.Resources.Where(x => x.IsInput)
                    .Select(x => new Resource
                    {
                        Type = new ResourceType { Id = x.ResourceTypeId },
                        Value = x.Value
                    }).ToArray(),
                OutputResources = x.Resources.Where(x => !x.IsInput)
                    .Select(x => new Resource
                    {
                        Type = new ResourceType { Id = x.ResourceTypeId },
                        Value = x.Value
                    }).ToArray(),
            }).ToArray();
            return receipts;
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
            LockDbPlayerRow(calcInfo.PlayerId);

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

        public void StartManufacture(int playerId, int domikId, int receiptId)
        {
            var date = DateTimeHelper.GetNowDate();

            LockDbPlayerRow(playerId);

            // todo проверка что есть нужное количество рабочих
            // todo что в домик влезет столько рабочих

            var needPlodderCount = 1;
            var dbDomik = _context.Domiks.First(x => x.PlayerId == playerId && x.Id == domikId);
            var domikLevel = GetDomikTypes().First(x => x.Id == dbDomik.TypeId).Levels.First(x => x.Value == dbDomik.Level);
            var levelReceipt = domikLevel.Receipts.First(x => x.Id == receiptId);
            var receipt = GetReceipts().First(x => x.Id == levelReceipt.Id);

            var manufacture = new Data.Manufacture
            {
                DomikId = domikId,
                DomikPlayerId = playerId,
                ReceiptId = receiptId,
                FinishDate = date.AddSeconds(receipt.DurationsSeconds),
                PlodderCount = needPlodderCount,
            };
            _context.Manufactures.Add(manufacture);
            _context.SaveChanges();

            _uow.AfterEventAction = () =>
            {
                _calculator.Insert(new CalculateInfo
                {
                    PlayerId = playerId,
                    ObjectId = manufacture.Id,
                    Type = CalculateTypes.Manufacture,
                    Date = manufacture.FinishDate,
                });
            };
        }

        public bool FinishManufacture(DateTime date, CalculateInfo calcInfo)
        {
            LockDbPlayerRow(calcInfo.PlayerId);

            var dbManufacture = _context.Manufactures.Single(x => x.Id == calcInfo.ObjectId);
            if (date >= dbManufacture.FinishDate)
            {
                var recept = GetReceipts().First(x => x.Id == dbManufacture.ReceiptId);
                foreach (var resource in recept.OutputResources)
                {
                    var dbResource = _context.Resources.FirstOrDefault(x => x.PlayerId == calcInfo.PlayerId && x.TypeId == resource.Type.Id);
                    if (dbResource == null)
                    {
                        dbResource = new Data.Resource { PlayerId = calcInfo.PlayerId, TypeId = resource.Type.Id };
                        _context.Resources.Add(dbResource);
                    }

                    dbResource.Value += resource.Value;
                }
                _context.Manufactures.Remove(dbManufacture);
                return true;
            }
            return false;
        }
    }
}
