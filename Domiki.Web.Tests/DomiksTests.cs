using Domiki.Web.Business;
using Domiki.Web.Business.Core;
using Domiki.Web.Business.Models;
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
                var domikManager = new DomikManager(uow.Context, null);
                var playerId = domikManager.GetPlayerId("testUser_" + Guid.NewGuid());
                Assert.Greater(playerId, 0);
            }
        }

        [Test]
        public void CheckBaseResources()
        {
            using (var uow = GetUow())
            {
                var domikManager = new DomikManager(uow.Context, null);
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

        /// <summary>
        /// ѕокупаем домик и провер€ем, что у нас 1 домик первого уровн€.
        /// </summary>
        [Test]
        public void BuyDomik()
        {
            using (var uow = GetUow())
            {
                var domikManager = new DomikManager(uow.Context, null);
                var playerId = domikManager.GetPlayerId("testUser_" + Guid.NewGuid());
                var types = domikManager.GetDomikTypes();
                var beforeResources = domikManager.GetResources(playerId);
                var buyType = types.First();
                domikManager.BuyDomik(playerId, buyType.Id);
                uow.Commit();

                var afterResources = domikManager.GetResources(playerId);
                var domiks = domikManager.GetDomiks(playerId);
                var domiksCount = domiks.Count();
                Assert.That(domiksCount, Is.EqualTo(1));
                var level = domiks.First().Level;
                Assert.That(level, Is.EqualTo(1));

                foreach (var resource in buyType.Levels[0].Resources)
                {
                    var beforeResource = beforeResources.First(x => x.Type.Id == resource.Type.Id);
                    var afterResource = afterResources.First(x => x.Type.Id == resource.Type.Id);
                    var actualDiffResource = beforeResource.Value - afterResource.Value;
                    var expectedDiffResource = resource.Value;
                    Assert.That(actualDiffResource, Is.EqualTo(expectedDiffResource));
                }
            }
        }

        /// <summary>
        /// ѕроверка на то, что конкурирующие запросы не могут привысить лимит
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
                    var domikManager = new DomikManager(uow.Context, null);
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
                            var domikManager = new DomikManager(uow.Context, null);
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
                    var domikManager = new DomikManager(uow.Context, null);
                    var domiks = domikManager.GetDomiks(playerId);
                    var domiksCount = domiks.Count();
                    Assert.That(domiksCount, Is.EqualTo(1), "iterarion number " + i);
                }
            }
        }

        [Test]
        public void UpgradeDomik()
        {
            int playerId;
            DomikType buyType;
            using (var uow = GetUow())
            {
                var domikManager = new DomikManager(uow.Context, null);
                playerId = domikManager.GetPlayerId("testUser_" + Guid.NewGuid());
                var types = domikManager.GetDomikTypes();
                buyType = types.First();
                domikManager.BuyDomik(playerId, buyType.Id);
                uow.Commit();
            }

            using (var uow = GetUow())
            {
                var domikManager = new DomikManager(uow.Context, null);
                var beforeResources = domikManager.GetResources(playerId);
                domikManager.UpgradeDomik(playerId, 1);
                uow.Commit();

                var domiks = domikManager.GetDomiks(playerId);
                var level = domiks.First().Level;
                Assert.That(level, Is.EqualTo(2));

                var afterResources = domikManager.GetResources(playerId);

                foreach (var resource in buyType.Levels.First(x => x.Value == 2).Resources)
                {
                    var beforeResource = beforeResources.First(x => x.Type.Id == resource.Type.Id);
                    var afterResource = afterResources.First(x => x.Type.Id == resource.Type.Id);
                    var actualDiffResource = beforeResource.Value - afterResource.Value;
                    var expectedDiffResource = resource.Value;
                    Assert.That(actualDiffResource, Is.EqualTo(expectedDiffResource));
                }
            }
        }

        /// <summary>
        /// ѕроверка на то, что конкурирующие запросы корректно улучшают домик
        /// </summary>
        [Test]
        public void ConcurrencyUpgradeDomik()
        {
            for (var i = 1; i <= 217; i++)
            {
                int playerId;
                int domikTypeId;
                using (var uow = GetUow())
                {
                    var domikManager = new DomikManager(uow.Context, null);
                    playerId = domikManager.GetPlayerId("testUser_" + Guid.NewGuid());
                    var types = domikManager.GetDomikTypes();
                    var domikType = types.First();
                    Assert.That(domikType.MaxCount, Is.EqualTo(1));
                    domikTypeId = domikType.Id;

                    domikManager.BuyDomik(playerId, domikTypeId);
                    uow.Commit();
                }

                var actionCount = 4;
                var numbers = Enumerable.Range(0, actionCount).ToList();
                var errorCount = 0;
                Parallel.ForEach(numbers, number =>
                {
                    try
                    {
                        using (var uow = GetUow())
                        {
                            var domikManager = new DomikManager(uow.Context, null);
                            domikManager.UpgradeDomik(playerId, 1);
                            uow.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                    }
                });

                using (var uow = GetUow())
                {
                    var domikManager = new DomikManager(uow.Context, null);
                    var domiks = domikManager.GetDomiks(playerId);
                    var level = domiks.First().Level;
                    var checkValue = level + errorCount;
                    var expected = 1 + actionCount;

                    // минимум один будет успешный
                    Assert.That(checkValue, Is.GreaterThan(1));

                    // количество успешных улучшеий + 1 базовый уровень + количество ошибок равно = 1 базовый уровень + количество действи
                    Assert.That(checkValue, Is.EqualTo(expected), "iterarion number " + i + ", checkValue " + checkValue + ",  error count " + errorCount);
                }
            }
        }
    }
}