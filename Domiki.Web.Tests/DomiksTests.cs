using Domiki.Web.Business;
using Domiki.Web.Business.Core;
using Domiki.Web.Business.Models;

namespace Domiki.Web.Tests
{
    public class DomiksTests : TestBase
    {
        [Test]
        public void GetPlayerIdTest()
        {
            var playerId = GetPlayerId();
            Assert.Greater(playerId, 0);
        }

        [Test]
        public void CheckBaseResourcesTest()
        {
            var playerId = GetPlayerId();
            var resources = GetResources(playerId);
            Assert.That(resources.Count, Is.EqualTo(1));
            Assert.That(resources.First().Type.Id, Is.EqualTo(1));
            Assert.That(resources.First().Value, Is.EqualTo(1000));
        }

        /// <summary>
        /// �������� ����� � ���������, ��� � ��� 1 ����� ������� ������.
        /// </summary>
        [Test]
        public void BuyDomikTest()
        {
            var playerId = GetPlayerId();
            var beforeResources = GetResources(playerId);
            var types = GetDomikTypes();
            var buyType = types.First();
            BuyDomik(playerId, buyType.Id);

            var afterResources = GetResources(playerId);
            var domiks = GetDomiks(playerId);
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

        /// <summary>
        /// �������� �� ��, ��� ������������� ������� �� ����� ��������� �����
        /// </summary>
        [Test]
        public void ConcurrencyBuyDomikTest()
        {
            for (var i = 1; i <= 217; i++)
            {
                var playerId = GetPlayerId();
                var types = GetDomikTypes();
                var domikType = types.First();
                Assert.That(domikType.MaxCount, Is.EqualTo(1));
                var domikTypeId = domikType.Id;

                var numbers = Enumerable.Range(0, 10).ToList();
                Parallel.ForEach(numbers, number =>
                {
                    try
                    {
                        BuyDomik(playerId, domikTypeId);
                    }
                    catch (Exception ex)
                    {
                    }
                });

                var domiks = GetDomiks(playerId);
                var domiksCount = domiks.Count();
                Assert.That(domiksCount, Is.EqualTo(1), "iterarion number " + i);
            }
        }

        [Test]
        public void UpgradeDomikTest()
        {
            var playerId = GetPlayerId();
            var types = GetDomikTypes();
            var buyType = types.First();
            BuyDomik(playerId, buyType.Id);
            var beforeResources = GetResources(playerId);
            UpgradeDomik(playerId, 1);

            var domiks = GetDomiks(playerId);
            var level = domiks.First().Level;
            Assert.That(level, Is.EqualTo(2));

            var afterResources = GetResources(playerId);

            foreach (var resource in buyType.Levels.First(x => x.Value == 2).Resources)
            {
                var beforeResource = beforeResources.First(x => x.Type.Id == resource.Type.Id);
                var afterResource = afterResources.First(x => x.Type.Id == resource.Type.Id);
                var actualDiffResource = beforeResource.Value - afterResource.Value;
                var expectedDiffResource = resource.Value;
                Assert.That(actualDiffResource, Is.EqualTo(expectedDiffResource));
            }
        }

        /// <summary>
        /// �������� �� ��, ��� ������������� ������� ��������� �������� �����
        /// </summary>
        [Test]
        public void ConcurrencyUpgradeDomikTest()
        {
            for (var i = 1; i <= 217; i++)
            {
                var playerId = GetPlayerId();
                var types = GetDomikTypes();
                var domikType = types.First();
                Assert.That(domikType.MaxCount, Is.EqualTo(1));
                var domikTypeId = domikType.Id;
                BuyDomik(playerId, domikTypeId);

                var actionCount = 4;
                var numbers = Enumerable.Range(0, actionCount).ToList();
                var errorCount = 0;
                Parallel.ForEach(numbers, number =>
                {
                    try
                    {
                        UpgradeDomik(playerId, 1);
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                    }
                });

                var domiks = GetDomiks(playerId);
                var level = domiks.First().Level;
                var checkValue = level + errorCount;
                var expected = 1 + actionCount;

                // ������� ���� ����� ��������
                Assert.That(checkValue, Is.GreaterThan(1));

                // ���������� �������� �������� + 1 ������� ������� + ���������� ������ ����� = 1 ������� ������� + ���������� �������
                Assert.That(checkValue, Is.EqualTo(expected), "iterarion number " + i + ", checkValue " + checkValue + ",  error count " + errorCount);
            }
        }

        /// <summary>
        /// ������� ���� ����� � �������� �������.
        /// </summary>
        [Test]
        public void ManufactureTest()
        {
            var playerId = GetPlayerId();
            var clayMineId = 5;
            var barakTypeId = 2;
            BuyDomik(playerId, clayMineId);
            BuyDomik(playerId, barakTypeId);
            var coinResourceTypeId = 1;
            var clayResourceTypeId = 4;
            var clayDigReceiptId = 1;
            var beforeResources = GetResources(playerId);
            var beforeCointResourceValue = beforeResources.First(x => x.Type.Id == coinResourceTypeId).Value;
            StartManufacture(playerId, 1, clayDigReceiptId);
            var afterResources = GetResources(playerId);
            var afterClayResourceValue = afterResources.First(x => x.Type.Id == clayResourceTypeId).Value;
            var afterCointResourceValue = afterResources.First(x => x.Type.Id == coinResourceTypeId).Value;
            var coinDiff = afterCointResourceValue - beforeCointResourceValue;
            Assert.That(afterClayResourceValue, Is.EqualTo(1));
            Assert.That(coinDiff, Is.EqualTo(-1));
        }

        /// <summary>
        /// ���� ����� ��� ����� ������ ��������, ������ ��������� ��� ������������ � ����� �������.
        /// </summary>
        [Test]
        public void MaxWorkerCountTest()
        {
            var playerId = GetPlayerId();
            var clayMineTypeId = 5;
            var barakTypeId = 2;
            BuyDomik(playerId, barakTypeId);
            BuyDomik(playerId, clayMineTypeId);
            BuyDomik(playerId, clayMineTypeId);
            var clayDigReceiptId = 1;
            var clayMineId3 = 2;
            var clayMineId2 = 3;
            StartManufacture(playerId, clayMineId2, clayDigReceiptId, false);
            Assert.Throws<BusinessException>(() => StartManufacture(playerId, clayMineId3, clayDigReceiptId, false), "Exception not throw");
        }

        private int GetPlayerId()
        {
            using (var uow = GetUow())
            {
                var domikManager = GetDomikManager(uow);
                var playerId = domikManager.GetPlayerId("testUser_" + Guid.NewGuid());
                uow.Commit();
                return playerId;
            }
        }

        private IEnumerable<Resource> GetResources(int playerId)
        {
            using (var uow = GetUow())
            {
                var domikManager = GetDomikManager(uow);
                var resources = domikManager.GetResources(playerId);
                uow.Commit();
                return resources;
            }
        }

        private IEnumerable<Domik> GetDomiks(int playerId)
        {
            using (var uow = GetUow())
            {
                var domikManager = GetDomikManager(uow);
                var domiks = domikManager.GetDomiks(playerId);
                return domiks;
            }
        }
        private IEnumerable<DomikType> GetDomikTypes()
        {
            using (var uow = GetUow())
            {
                var domikManager = GetResourceManager(uow);
                var types = domikManager.GetDomikTypes();
                return types;
            }
        }

        private void UpgradeDomik(int playerId, int domikId)
        {
            using (var uow = GetUow())
            {
                var domikManager = GetDomikManager(uow);
                domikManager.UpgradeDomik(playerId, domikId);
                uow.Commit();
            }
        }

        private void BuyDomik(int playerId, int domikTypeId)
        {
            using (var uow = GetUow())
            {
                var domikManager = GetDomikManager(uow);
                domikManager.BuyDomik(playerId, domikTypeId);
                uow.Commit();
            }
        }
        private void StartManufacture(int playerId, int domikId, int receiptId, bool calculatorJustFinishMod = true)
        {
            using (var uow = GetUow())
            {
                var domikManager = GetDomikManager(uow, calculatorJustFinishMod);
                domikManager.StartManufacture(playerId, domikId, receiptId);
                uow.Commit();
            }
        }
    }
}