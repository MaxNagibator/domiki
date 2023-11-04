using Domiki.Web.Business;
using Domiki.Web.Business.Core;
using Domiki.Web.Data;

namespace Domiki.Web.Tests
{
    public class DomiksTests : TestBase
    {
        [Test]
        public void GetPlayerId()
        {
            using (var uow = GetUow())
            {
                var domikManager = new DomikManager(uow.Context);
                var playerId = domikManager.GetPlayerId("testUser_" + Guid.NewGuid());
                Assert.Greater(playerId, 0);
            }
        }

        [Test]
        public void CheckBaseResources()
        {
            using (var uow = GetUow())
            {
                var domikManager = new DomikManager(uow.Context);
                var playerId = domikManager.GetPlayerId("testUser_" + Guid.NewGuid());
                uow.Commit();

                var resources = domikManager.GetResources(playerId);
                Assert.That(resources.Count, Is.EqualTo(StaticEntities.ResourceTypes.Count));
                foreach (var resource in resources)
                {
                    Assert.That(resource.Value, Is.EqualTo(1000));
                }
            }
        }


        [Test]
        public void BuyDomik()
        {
            using (var uow = GetUow())
            {
                var domikManager = new DomikManager(uow.Context);
                var playerId = domikManager.GetPlayerId("testUser_" + Guid.NewGuid());
                var types = domikManager.GetDomikTypes();
                domikManager.BuyDomik(playerId, types.First().Id);
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
            for (var i = 1; i <= 217; i++)
            {
                int playerId;
                int domikTypeId;
                using (var uow = GetUow())
                {
                    var domikManager = new DomikManager(uow.Context);
                    playerId = domikManager.GetPlayerId("testUser_" + Guid.NewGuid());
                    uow.Commit();

                    var types = domikManager.GetDomikTypes();
                    var domikType = types.First();
                    Assert.That(domikType.MaxCount, Is.EqualTo(1));
                    domikTypeId = domikType.Id;
                }

                var numbers = Enumerable.Range(0, 10).ToList();
                Parallel.ForEach(numbers, number =>
                {
                    try
                    {
                        using (var uow = GetUow())
                        {
                            var domikManager = new DomikManager(uow.Context);
                            domikManager.BuyDomik(playerId, domikTypeId);
                            uow.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                });

                using (var uow = GetUow())
                {
                    var domikManager = new DomikManager(uow.Context);
                    var domiks = domikManager.GetDomiks(playerId);
                    var domiksCount = domiks.Count();
                    Assert.That(domiksCount, Is.EqualTo(1), "iterarion number " + i);
                }
            }
        }
    }
}