using Domiki.Business.Core;
using Domiki.Business.Models;
using Domiki.Data;
using Domiki.Web.Data;
using Domiki.Web.Extentions;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static IdentityModel.ClaimComparer;

namespace Domiki.Web.Tests
{
    public class DomiksTests : TestBase
    {
        private DomikManager _domikManager;

        [SetUp]
        public void Setup()
        {
            //var uow = GetUow();
            //_domikManager = new DomikManager(uow.Context);
        }

        public class MyOperationalStoreOptions : IOptions<OperationalStoreOptions>
        {
            public OperationalStoreOptions Value => new OperationalStoreOptions()
            {
                DeviceFlowCodes = new TableConfiguration("DeviceCodes"),
                EnableTokenCleanup = false,
                PersistedGrants = new TableConfiguration("PersistedGrants"),
                TokenCleanupBatchSize = 100,
                TokenCleanupInterval = 3600,
            };
        }

        [Test]
        public void GetPlayerId()
        {
            var playerId = _domikManager.GetPlayerId("testUser_" + Guid.NewGuid());
            Assert.Greater(playerId, 0);
        }

        [Test]
        public void BuyDomik()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(_options.ConnectionStrings.DefaultConnection);
            var context = new ApplicationDbContext(optionsBuilder.Options, new MyOperationalStoreOptions());
            using (var uow = new UnitOfWork(context))
            {
                var domikManager = new DomikManager(uow.Context);
                var playerId = domikManager.GetPlayerId("testUser_" + Guid.NewGuid());
                var types = domikManager.GetDomikTypes();
                domikManager.BuyDomik(playerId, types.First().Id);
                uow.Context.SaveChanges();
                uow.Commit();

                var domiks = domikManager.GetDomiks(playerId);
                var domiksCount = domiks.Count();
                Assert.That(domiksCount, Is.EqualTo(1));
            }
        }

        /// <summary>
        /// Проверка на то, что конкурирующие запросы не могут привысить лимит
        /// </summary>
        [Test]
        public void ConcurrencyBuyDomik()
        {
            for (var i = 1; i <= 1; i++)
            {
                int playerId;
                int domikTypeId;
                using (var uow = GetUow())
                {
                    var _domikManager = new DomikManager(uow.Context);
                    playerId = _domikManager.GetPlayerId("testUser_" + Guid.NewGuid());
                    var types = _domikManager.GetDomikTypes();
                    var domikType = types.First();
                    Assert.That(domikType.MaxCount, Is.EqualTo(1));
                    domikTypeId = domikType.Id;
                }
                var numbers = Enumerable.Range(0, 10).ToList();
                Parallel.ForEach(numbers, number =>
                {
                    try
                    {
                        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                        optionsBuilder.UseSqlServer(_options.ConnectionStrings.DefaultConnection);
                        var context = new ApplicationDbContext(optionsBuilder.Options, new MyOperationalStoreOptions());
                        using (var uow = new UnitOfWork(context))
                        {
                            var domikManager = new DomikManager(uow.Context);
                            domikManager.BuyDomik(playerId, domikTypeId);
                            uow.Context.SaveChanges();
                            uow.Commit();
                            Console.WriteLine(number + " commit");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(number + ex.Message);
                    }
                });

                using (var uow = GetUow())
                {
                    var _domikManager = new DomikManager(uow.Context);
                    var domiks = _domikManager.GetDomiks(playerId);
                    var domiksCount = domiks.Count();
                    Assert.That(domiksCount, Is.EqualTo(1), "iterarion number " + i);
                }
            }
        }
    }
}