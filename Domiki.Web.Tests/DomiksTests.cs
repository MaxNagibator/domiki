using Domiki.Business.Core;
using Domiki.Web.Extentions;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.Extensions.Options;

namespace Domiki.Web.Tests
{
    public class DomiksTests : TestBase
    {
        private DomikManager _domikManager;

        [SetUp]
        public void Setup()
        {
            _domikManager = new DomikManager(GetContext());
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
            var playerId = _domikManager.GetPlayerId("testUser_" + Guid.NewGuid());
            var types = _domikManager.GetDomikTypes();
            _domikManager.BuyDomik(playerId, types.First().Id);
            var domiks = _domikManager.GetDomiks(playerId);
            var domiksCount = domiks.Count();
            Assert.That(domiksCount, Is.EqualTo(1));
        }

        /// <summary>
        /// Проверка на то, что конкурирующие запросы не могут привысить лимит
        /// </summary>
        [Test]
        public void ConcurrencyBuyDomik()
        {
            for (var i = 1; i <= 100; i++)
            {
                var playerId = _domikManager.GetPlayerId("testUser_" + Guid.NewGuid());
                var types = _domikManager.GetDomikTypes();
                var domikType = types.First();

                Assert.That(domikType.MaxCount, Is.EqualTo(1));

                var numbers = Enumerable.Range(0, 10).ToList();
                Parallel.ForEach(numbers, number =>
                {
                    try
                    {
                        var domikManager = new DomikManager(GetContext());
                        domikManager.BuyDomik(playerId, domikType.Id);
                    }
                    catch (Exception ex)
                    {

                    }
                });

                var domiks = _domikManager.GetDomiks(playerId);
                var domiksCount = domiks.Count();
                Assert.That(domiksCount, Is.EqualTo(1), "iterarion number " + i);
            }
        }
    }
}