using Domiki.Web.Core;
using Domiki.Web.Infrastructure;
using System.Text.Json;

namespace Domiki.Web.Tests;

/// <summary>
/// Правила кормления трудяг Корчмой.
/// </summary>
public sealed class MealTests
{
    private const int OrdinaryTraitId = 1;
    private const int SonyaTraitId = 4;

    /// <summary>
    /// Трудяга с чертой «Соня» не устаёт и не ест еду Корчмы при завершении производства.
    /// </summary>
    [Test]
    public void SonyaDoesNotEatWhenFinishingManufactureTest()
    {
        const int startBread = 3;

        var player = TestPlayer.Create()
            .WithDomik(DomikIds.Tavern)
            .WithResource(ResourceIds.Bread, startBread);

        var worker = player.Workers().Single();
        player.SetWorkerTrait(worker.Id, SonyaTraitId);

        using (App.PendingEvents())
        {
            player.StartManufacture(StartingDomikIds.ClayMine, ReceiptIds.ClayDig24h);
        }

        var manufacture = player.Manufacture(StartingDomikIds.ClayMine);
        player.FinishManufacture(manufacture.Id, manufacture.FinishDate.AddSeconds(1));

        worker = player.Workers().Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(worker.RestUntil, Is.Null);
            Assert.That(player.Resource(ResourceIds.Bread), Is.EqualTo(startBread));
        }
    }

    /// <summary>
    /// Корчма первого уровня кормит уставшего трудягу хлебом, а без Корчмы или еды отдых остаётся полным.
    /// </summary>
    /// <param name="tavernLevel">Уровень Корчмы, ноль означает её отсутствие.</param>
    /// <param name="bread">Сколько хлеба выдано игроку перед стартом.</param>
    /// <param name="expectedRestSeconds">Ожидаемая длительность отдыха трудяги.</param>
    /// <param name="expectedBread">Ожидаемый остаток хлеба после завершения производства.</param>
    [TestCase(1, 3, 3600, 2)]
    [TestCase(3, 3, 3600, 2)]
    [TestCase(0, 3, 7200, 3)]
    [TestCase(1, 0, 7200, 0)]
    public void FatiguedWorkerMealDependsOnTavernAndFoodTest(int tavernLevel, int bread, int expectedRestSeconds, int expectedBread)
    {
        var player = TestPlayer.Create();
        if (tavernLevel > 0)
        {
            player.WithDomik(DomikIds.Tavern, tavernLevel);
        }

        if (bread > 0)
        {
            player.WithResource(ResourceIds.Bread, bread);
        }

        var worker = player.Workers().Single();
        player.SetWorkerTrait(worker.Id, OrdinaryTraitId);

        using (App.PendingEvents())
        {
            player.StartManufacture(StartingDomikIds.ClayMine, ReceiptIds.ClayDig8h);
        }

        var manufacture = player.Manufacture(StartingDomikIds.ClayMine);
        var finishDate = manufacture.FinishDate.AddSeconds(1);
        player.FinishManufacture(manufacture.Id, finishDate);

        worker = player.Workers().Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(worker.RestUntil, Is.Not.Null);
            Assert.That((worker.RestUntilValue() - finishDate).TotalSeconds, Is.EqualTo(expectedRestSeconds).Within(2));
            Assert.That(player.Resource(ResourceIds.Bread), Is.EqualTo(expectedBread));
        }
    }

    /// <summary>
    /// Корчма кормит уставшего трудягу сыром, сваренным в только что завершившейся смене, даже когда строки сыра ещё нет в запасах.
    /// </summary>
    [Test]
    public void FreshlyProducedFoodFeedsFatiguedWorkerTest()
    {
        const int cheeseShiftSeconds = 2 * 3600;
        const int cheeseOutput = 2;
        const int mealCount = 1;

        var player = TestPlayer.Create()
            .WithDomik(DomikIds.Tavern)
            .WithDomik(DomikIds.Sheepfold)
            .WithResource(ResourceIds.Grain, 2);
        var worker = player.Workers().Single();
        player.SetWorkerTrait(worker.Id, OrdinaryTraitId);
        player.SetWorkerWorked(worker.Id, DomikManager.FatigueThresholdSeconds - cheeseShiftSeconds);

        using (App.PendingEvents())
        {
            player.StartManufacture(player.DomikId(DomikIds.Sheepfold), ReceiptIds.MakeCheese);
        }

        var manufacture = player.Manufacture(player.DomikId(DomikIds.Sheepfold));
        var finishDate = manufacture.FinishDate.AddSeconds(1);
        player.FinishManufacture(manufacture.Id, finishDate);

        worker = player.Workers().Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That((worker.RestUntilValue() - finishDate).TotalSeconds, Is.EqualTo(3600).Within(2));
            Assert.That(player.Resource(ResourceIds.Cheese), Is.EqualTo(cheeseOutput - mealCount));
        }
    }

    /// <summary>
    /// Корчма кормит уставшего трудягу сыром, когда хлеба нет.
    /// </summary>
    [Test]
    public void CheeseFeedsFatiguedWorkerTest()
    {
        const int startCheese = 2;

        var player = TestPlayer.Create()
            .WithDomik(DomikIds.Tavern)
            .WithResource(ResourceIds.Cheese, startCheese);
        var worker = player.Workers().Single();
        player.SetWorkerTrait(worker.Id, OrdinaryTraitId);

        using (App.PendingEvents())
        {
            player.StartManufacture(StartingDomikIds.ClayMine, ReceiptIds.ClayDig8h);
        }

        var manufacture = player.Manufacture(StartingDomikIds.ClayMine);
        var finishDate = manufacture.FinishDate.AddSeconds(1);
        player.FinishManufacture(manufacture.Id, finishDate);

        worker = player.Workers().Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That((worker.RestUntilValue() - finishDate).TotalSeconds, Is.EqualTo(3600).Within(2));
            Assert.That(player.Resource(ResourceIds.Cheese), Is.EqualTo(startCheese - 1));
        }
    }

    /// <summary>
    /// Запас кладовой не даёт Корчме тронуть отложенный хлеб (трудяга отдыхает весь срок), но пускает в дело излишек сверх запаса.
    /// </summary>
    /// <param name="bread">Хлеб на складе перед стартом.</param>
    /// <param name="reserve">Запас, который кладовая не должна трогать.</param>
    /// <param name="expectedRestSeconds">Ожидаемая длительность отдыха трудяги.</param>
    /// <param name="expectedBread">Ожидаемый остаток хлеба после завершения производства.</param>
    [TestCase(2, 2, 7200, 2)]
    [TestCase(3, 2, 3600, 2)]
    public void ReserveKeepsBreadUntouchedUnlessSurplusTest(int bread, int reserve, int expectedRestSeconds, int expectedBread)
    {
        var player = TestPlayer.Create()
            .WithDomik(DomikIds.Tavern)
            .WithResource(ResourceIds.Bread, bread);

        player.SetFoodRule(ResourceIds.Bread, reserve, false);

        var worker = player.Workers().Single();
        player.SetWorkerTrait(worker.Id, OrdinaryTraitId);

        using (App.PendingEvents())
        {
            player.StartManufacture(StartingDomikIds.ClayMine, ReceiptIds.ClayDig8h);
        }

        var manufacture = player.Manufacture(StartingDomikIds.ClayMine);
        var finishDate = manufacture.FinishDate.AddSeconds(1);
        player.FinishManufacture(manufacture.Id, finishDate);

        worker = player.Workers().Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That((worker.RestUntilValue() - finishDate).TotalSeconds, Is.EqualTo(expectedRestSeconds).Within(2));
            Assert.That(player.Resource(ResourceIds.Bread), Is.EqualTo(expectedBread));
        }
    }

    /// <summary>
    /// Запрещённый в кладовой хлеб Корчма не трогает, даже будучи дешевле сыра, и кормит трудягу сыром вместо него.
    /// </summary>
    [Test]
    public void ForbiddenBreadIsSkippedForCheeseTest()
    {
        const int startBread = 2;
        const int startCheese = 1;

        var player = TestPlayer.Create()
            .WithDomik(DomikIds.Tavern)
            .WithResource(ResourceIds.Bread, startBread)
            .WithResource(ResourceIds.Cheese, startCheese);

        player.SetFoodRule(ResourceIds.Bread, 0, true);

        var worker = player.Workers().Single();
        player.SetWorkerTrait(worker.Id, OrdinaryTraitId);

        using (App.PendingEvents())
        {
            player.StartManufacture(StartingDomikIds.ClayMine, ReceiptIds.ClayDig8h);
        }

        var manufacture = player.Manufacture(StartingDomikIds.ClayMine);
        player.FinishManufacture(manufacture.Id, manufacture.FinishDate.AddSeconds(1));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(player.Resource(ResourceIds.Bread), Is.EqualTo(startBread));
            Assert.That(player.Resource(ResourceIds.Cheese), Is.Zero);
        }
    }

    /// <summary>
    /// Когда вся еда в кладовой заповедана, трудяга отдыхает весь срок, а в журнал попадает запись о кормлении с причиной «forbidden».
    /// </summary>
    [Test]
    public void AllFoodForbiddenLeavesFullRestAndForbiddenReasonEventTest()
    {
        const int startBread = 3;

        var player = TestPlayer.Create()
            .WithDomik(DomikIds.Tavern)
            .WithResource(ResourceIds.Bread, startBread);

        player.SetFoodRule(ResourceIds.Bread, 0, true);

        var worker = player.Workers().Single();
        player.SetWorkerTrait(worker.Id, OrdinaryTraitId);

        using (App.PendingEvents())
        {
            player.StartManufacture(StartingDomikIds.ClayMine, ReceiptIds.ClayDig8h);
        }

        var manufacture = player.Manufacture(StartingDomikIds.ClayMine);
        var finishDate = manufacture.FinishDate.AddSeconds(1);
        player.FinishManufacture(manufacture.Id, finishDate);

        worker = player.Workers().Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That((worker.RestUntilValue() - finishDate).TotalSeconds, Is.EqualTo(7200).Within(2));
            Assert.That(player.Resource(ResourceIds.Bread), Is.EqualTo(startBread));
            Assert.That(GetMealEventReason(player.Id), Is.EqualTo("forbidden"));
        }
    }

    /// <summary>
    /// Успешное кормление пишет в журнал событие с именем трудяги и съеденным хлебом, а повторное кормление сливается в ту же
    /// запись со счётчиком 2 и обезличенным именем.
    /// </summary>
    [Test]
    public void SuccessfulMealWritesAndMergesWorkerMealEventTest()
    {
        const int startBread = 10;
        const int mergedCount = 2;

        var player = TestPlayer.Create()
            .WithDomik(DomikIds.Tavern)
            .WithDomik(DomikIds.Barrack)
            .WithDomik(DomikIds.ClayMine)
            .WithResource(ResourceIds.Bread, startBread);

        foreach (var worker in player.Workers())
        {
            player.SetWorkerTrait(worker.Id, OrdinaryTraitId);
        }

        var firstDomikId = StartingDomikIds.ClayMine;
        var secondDomikId = player.DomikId(DomikIds.ClayMine);

        using (App.PendingEvents())
        {
            player.StartManufacture(firstDomikId, ReceiptIds.ClayDig8h);
            player.StartManufacture(secondDomikId, ReceiptIds.ClayDig8h);
        }

        var firstManufacture = player.Manufacture(firstDomikId);
        var secondManufacture = player.Manufacture(secondDomikId);
        var firstWorkerName = player.Workers().Single(x => x.ManufactureId == firstManufacture.Id).Name;

        player.FinishManufacture(firstManufacture.Id, firstManufacture.FinishDate.AddSeconds(1));

        var firstEvents = GetMealEvents(player.Id);
        Assert.That(firstEvents, Has.Count.EqualTo(1));
        using (var firstData = JsonDocument.Parse(firstEvents[0].Data))
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(firstData.RootElement.GetProperty("workerName").GetString(), Is.EqualTo(firstWorkerName));
                Assert.That(firstData.RootElement.GetProperty("count").GetInt32(), Is.EqualTo(1));
            }
        }

        player.FinishManufacture(secondManufacture.Id, secondManufacture.FinishDate.AddSeconds(1));

        var mergedEvents = GetMealEvents(player.Id);
        Assert.That(mergedEvents, Has.Count.EqualTo(1));
        using var mergedData = JsonDocument.Parse(mergedEvents[0].Data);
        var resource = mergedData.RootElement.GetProperty("resources").EnumerateArray().Single(x => x.GetProperty("resourceTypeId").GetInt32() == ResourceIds.Bread);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(mergedData.RootElement.GetProperty("count").GetInt32(), Is.EqualTo(mergedCount));
            Assert.That(mergedData.RootElement.GetProperty("workerName").ValueKind, Is.EqualTo(JsonValueKind.Null));
            Assert.That(resource.GetProperty("value").GetInt32(), Is.EqualTo(mergedCount));
        }
    }

    /// <summary>
    /// Два трудяги, уставших в одной смене одного производства, сливаются в одну запись журнала со счётчиком 2 и
    /// обезличенным именем, даже когда оба кормления регистрируются до сохранения транзакции.
    /// </summary>
    [Test]
    public void TwoWorkersFatiguingInSameManufactureMergeIntoOneEventTest()
    {
        const int startBread = 10;
        const int mealCount = 2;
        const int manufactureDurationSeconds = 3600;

        var player = TestPlayer.Create()
            .WithDomik(DomikIds.Tavern)
            .WithDomiks(DomikIds.Barrack, 4)
            .WithDomik(DomikIds.ClayMine)
            .WithResource(ResourceIds.Bread, startBread);

        var domikId = player.DomikId(DomikIds.ClayMine);
        player.Upgrade(domikId);

        var workers = player.Workers().OrderBy(x => x.Id).ToArray();
        foreach (var worker in workers)
        {
            player.SetWorkerTrait(worker.Id, OrdinaryTraitId);
        }

        var fatiguedWorkerIds = workers.Take(mealCount).Select(x => x.Id).ToArray();
        foreach (var workerId in fatiguedWorkerIds)
        {
            player.SetWorkerWorked(workerId, DomikManager.FatigueThresholdSeconds - manufactureDurationSeconds);
        }

        using (App.PendingEvents())
        {
            player.StartManufacture(domikId, ReceiptIds.ClayDigTogether);
        }

        var manufacture = player.Manufacture(domikId);
        player.FinishManufacture(manufacture.Id, manufacture.FinishDate.AddSeconds(1));

        var events = GetMealEvents(player.Id);
        Assert.That(events, Has.Count.EqualTo(1));
        using var data = JsonDocument.Parse(events[0].Data);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(data.RootElement.GetProperty("count").GetInt32(), Is.EqualTo(mealCount));
            Assert.That(data.RootElement.GetProperty("workerName").ValueKind, Is.EqualTo(JsonValueKind.Null));
        }
    }

    /// <summary>
    /// Кладовая копит счётчик съеденного за сутки и сбрасывает его, когда сохранённые сутки уже не совпадают с текущими.
    /// </summary>
    [Test]
    public void EatenTodayAccumulatesAndResetsOnNewDayTest()
    {
        const int firstMeal = 1;
        const int secondMeal = 2;

        var player = TestPlayer.Create();

        player.RegisterMeal(ResourceIds.Bread, firstMeal);
        player.RegisterMeal(ResourceIds.Bread, secondMeal);

        var rule = player.FoodRules().Single(x => x.ResourceTypeId == ResourceIds.Bread);
        Assert.That(rule.EatenToday, Is.EqualTo(firstMeal + secondMeal));

        SetFoodRuleEatenDate(player.Id, ResourceIds.Bread, DateTimeHelper.GetNowDate().Date.AddDays(-1));

        var staleRule = player.FoodRules().Single(x => x.ResourceTypeId == ResourceIds.Bread);
        Assert.That(staleRule.EatenToday, Is.Zero);
    }

    /// <summary>
    /// Корчма списывает хлеб раньше сыра, потому что хлеб дешевле на рынке.
    /// </summary>
    [Test]
    public void BreadIsEatenBeforeCheeseTest()
    {
        const int startBread = 1;
        const int startCheese = 1;

        var player = TestPlayer.Create()
            .WithDomik(DomikIds.Tavern)
            .WithResource(ResourceIds.Bread, startBread)
            .WithResource(ResourceIds.Cheese, startCheese);
        var worker = player.Workers().Single();
        player.SetWorkerTrait(worker.Id, OrdinaryTraitId);

        using (App.PendingEvents())
        {
            player.StartManufacture(StartingDomikIds.ClayMine, ReceiptIds.ClayDig8h);
        }

        var manufacture = player.Manufacture(StartingDomikIds.ClayMine);
        player.FinishManufacture(manufacture.Id, manufacture.FinishDate.AddSeconds(1));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(player.Resource(ResourceIds.Bread), Is.Zero);
            Assert.That(player.Resource(ResourceIds.Cheese), Is.EqualTo(startCheese));
        }
    }

    private static List<Data.Entities.PlayerEvent> GetMealEvents(int playerId)
    {
        return App.Read(context => context.PlayerEvents.Where(x => x.PlayerId == playerId && x.Type == Data.Entities.PlayerEventType.WorkerMeal).ToList());
    }

    private static string? GetMealEventReason(int playerId)
    {
        var events = GetMealEvents(playerId);
        Assert.That(events, Has.Count.EqualTo(1));
        using var data = JsonDocument.Parse(events[0].Data);
        return data.RootElement.GetProperty("reason").GetString();
    }

    private static void SetFoodRuleEatenDate(int playerId, int resourceTypeId, DateTime eatenDate)
    {
        using var scope = App.Scope();
        scope.Context.PlayerFoodRules.Single(x => x.PlayerId == playerId && x.ResourceTypeId == resourceTypeId).EatenDate = eatenDate;
        scope.Commit();
    }
}
