using Domiki.Web.Core;
using Domiki.Web.Economy;
using Domiki.Web.Infrastructure;
using System.Text.Json;
using PlayerEventType = Domiki.Web.Data.Entities.PlayerEventType;

namespace Domiki.Web.Tests;

/// <summary>
/// Правила Избы старосты: счётная книга, мера наряда и заповедный припас.
/// </summary>
public sealed class ElderHouseTests
{
    private const int PotteryDomikId = 4;
    private const int DishesPerCycle = 1;
    private const int ClayPerCycle = 2;

    /// <summary>
    /// Счётная книга ведётся только у игрока с Избой старосты: без неё движения ресурсов в книгу не пишутся.
    /// </summary>
    /// <param name="elderHouseLevel">Уровень Избы старосты, ноль означает её отсутствие.</param>
    /// <param name="expectedRows">Ожидаемое число строк книги за сутки.</param>
    [TestCase(0, 0)]
    [TestCase(1, 1)]
    public void LedgerCountsOnlyWithElderHouseTest(int elderHouseLevel, int expectedRows)
    {
        const int grantedClay = 7;

        var player = TestPlayer.Create();
        if (elderHouseLevel > 0)
        {
            player.WithDomik(DomikIds.ElderHouse, elderHouseLevel);
        }

        player.GrantResource(ResourceIds.Clay, grantedClay);

        var flows = App.Read(context => context.PlayerResourceFlows.Where(x => x.PlayerId == player.Id).ToList());
        Assert.That(flows, Has.Count.EqualTo(expectedRows));
        if (expectedRows > 0)
        {
            Assert.That(flows[0].Gained, Is.EqualTo(grantedClay));
        }
    }

    /// <summary>
    /// Книга разводит приход и расход одного ресурса: списание не уменьшает приход, а копится отдельной суммой.
    /// </summary>
    [Test]
    public void LedgerKeepsGainAndSpendApartTest()
    {
        const int grantedClay = 10;
        const int spentClay = 4;

        var player = TestPlayer.Create()
            .WithDomik(DomikIds.ElderHouse);

        player.GrantResource(ResourceIds.Clay, grantedClay);
        player.WriteOffResource(ResourceIds.Clay, spentClay);

        var flow = App.Read(context => context.PlayerResourceFlows.Single(x => x.PlayerId == player.Id && x.ResourceTypeId == ResourceIds.Clay));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(flow.Gained, Is.EqualTo(grantedClay));
            Assert.That(flow.Spent, Is.EqualTo(spentClay));
        }
    }

    /// <summary>
    /// Смена, начавшаяся до полуночи, делит отработанные секунды между двумя сутками по фактическому перекрытию.
    /// </summary>
    [Test]
    public void ShiftOnDayBoundarySplitsWorkedSecondsTest()
    {
        const int durationSeconds = 3600;
        const int secondsAfterMidnight = 900;

        var player = TestPlayer.Create()
            .WithDomik(DomikIds.ElderHouse);

        var finishDate = DateTimeHelper.GetNowDate().Date.AddSeconds(secondsAfterMidnight);
        App.Act<ElderHouseManager>(m => m.RecordShift(player.Id, finishDate, durationSeconds, 1));

        var days = App.Read(context => context.PlayerLaborDays.Where(x => x.PlayerId == player.Id).OrderBy(x => x.Date).ToList());
        using (Assert.EnterMultipleScope())
        {
            Assert.That(days, Has.Count.EqualTo(2));
            Assert.That(days[0].WorkedSeconds, Is.EqualTo(durationSeconds - secondsAfterMidnight));
            Assert.That(days[1].WorkedSeconds, Is.EqualTo(secondsAfterMidnight));
        }
    }

    /// <summary>
    /// Наряд с мерой снимается сам, как только запаса ресурса набралось до меры, и оставляет событие
    /// <see cref="PlayerEventType.ManufactureMeasureMet"/> вместо события заглохшего наряда.
    /// </summary>
    [Test]
    public void MeasureStopsNaryadWhenStockReachedTest()
    {
        const int startClay = 4;

        var player = TestPlayer.Create()
            .WithDomik(DomikIds.Barrack)
            .WithDomik(DomikIds.Pottery, 3)
            .WithDomik(DomikIds.ElderHouse, 2)
            .WithResource(ResourceIds.Clay, startClay);

        using (App.PendingEvents())
        {
            player.StartManufacture(PotteryDomikId, ReceiptIds.MakeDishes, autoRepeat: true);
        }

        var manufacture = player.Manufacture(PotteryDomikId);
        player.SetMeasure(manufacture.Id, ResourceIds.Dishes, DishesPerCycle);
        player.FinishManufacture(manufacture.Id, manufacture.FinishDate.AddSeconds(1));

        var events = App.Read(context => context.PlayerEvents.Where(x => x.PlayerId == player.Id && x.Type == PlayerEventType.ManufactureMeasureMet).ToList());
        using (Assert.EnterMultipleScope())
        {
            Assert.That(events, Has.Count.EqualTo(1));
            Assert.That(player.Resource(ResourceIds.Dishes), Is.EqualTo(DishesPerCycle));
            // Второй круг не пошёл, поэтому глина осталась на нём непотраченной.
            Assert.That(player.Resource(ResourceIds.Clay), Is.EqualTo(startClay - ClayPerCycle));
            Assert.That(player.ManufactureCount(PotteryDomikId), Is.Zero);
        }

        using var data = JsonDocument.Parse(events[0].Data);
        Assert.That(data.RootElement.GetProperty("resourceTypeId").GetInt32(), Is.EqualTo(ResourceIds.Dishes));
    }

    /// <summary>
    /// Мера переносится на каждый следующий круг наряда, поэтому наряд идёт, пока запас до меры не дорос.
    /// </summary>
    [Test]
    public void MeasureCarriesOverToNextRoundTest()
    {
        const int startClay = 6;
        const int measure = 2;

        var player = TestPlayer.Create()
            .WithDomik(DomikIds.Barrack)
            .WithDomik(DomikIds.Pottery, 3)
            .WithDomik(DomikIds.ElderHouse, 2)
            .WithResource(ResourceIds.Clay, startClay);

        using (App.PendingEvents())
        {
            player.StartManufacture(PotteryDomikId, ReceiptIds.MakeDishes, autoRepeat: true);
        }

        var manufacture = player.Manufacture(PotteryDomikId);
        player.SetMeasure(manufacture.Id, ResourceIds.Dishes, measure);
        player.FinishManufacture(manufacture.Id, manufacture.FinishDate.AddSeconds(1));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(player.Resource(ResourceIds.Dishes), Is.EqualTo(measure));
            // Мера набралась за два круга, третий не пошёл, хотя глина на него ещё была.
            Assert.That(player.Resource(ResourceIds.Clay), Is.EqualTo(startClay - measure * ClayPerCycle));
        }
    }

    /// <summary>
    /// Меру нельзя назначить, пока Изба старосты не доросла до мерной рейки.
    /// </summary>
    [Test]
    public void MeasureNeedsElderHouseLevelTest()
    {
        var player = TestPlayer.Create()
            .WithDomik(DomikIds.Barrack)
            .WithDomik(DomikIds.Pottery, 3)
            .WithDomik(DomikIds.ElderHouse)
            .WithResource(ResourceIds.Clay, 4);

        using (App.PendingEvents())
        {
            player.StartManufacture(PotteryDomikId, ReceiptIds.MakeDishes, autoRepeat: true);
        }

        var manufacture = player.Manufacture(PotteryDomikId);
        Assert.That(Throws.Business(() => player.SetMeasure(manufacture.Id, ResourceIds.Dishes, DishesPerCycle)).Message, Does.Contain("мерной рейки"));
    }

    /// <summary>
    /// Заповеданный припас останавливает наряд, не списывая ресурс, и оставляет событие
    /// <see cref="PlayerEventType.ManufactureReserveHeld"/>.
    /// </summary>
    [Test]
    public void ReserveStopsNaryadWithoutSpendingTest()
    {
        const int startClay = 4;
        const int reserve = 3;

        var player = TestPlayer.Create()
            .WithDomik(DomikIds.Barrack)
            .WithDomik(DomikIds.Pottery, 3)
            .WithDomik(DomikIds.ElderHouse, 3)
            .WithResource(ResourceIds.Clay, startClay);

        player.SetReserve(ResourceIds.Clay, reserve);

        using (App.PendingEvents())
        {
            player.StartManufacture(PotteryDomikId, ReceiptIds.MakeDishes, autoRepeat: true);
        }

        var manufacture = player.Manufacture(PotteryDomikId);
        player.FinishManufacture(manufacture.Id, manufacture.FinishDate.AddSeconds(1));

        var events = App.Read(context => context.PlayerEvents.Where(x => x.PlayerId == player.Id && x.Type == PlayerEventType.ManufactureReserveHeld).ToList());
        using (Assert.EnterMultipleScope())
        {
            Assert.That(events, Has.Count.EqualTo(1));
            Assert.That(player.Resource(ResourceIds.Clay), Is.EqualTo(startClay - ClayPerCycle));
            Assert.That(player.ManufactureCount(PotteryDomikId), Is.Zero);
        }
    }

    /// <summary>
    /// Заповедь связывает только наряды: запуск смены руками берёт отложенный припас как обычно.
    /// </summary>
    [Test]
    public void ReserveDoesNotBindManualStartTest()
    {
        const int startClay = 4;
        const int reserve = 3;

        var player = TestPlayer.Create()
            .WithDomik(DomikIds.Barrack)
            .WithDomik(DomikIds.Pottery, 3)
            .WithDomik(DomikIds.ElderHouse, 3)
            .WithResource(ResourceIds.Clay, startClay);

        player.SetReserve(ResourceIds.Clay, reserve);

        using (App.PendingEvents())
        {
            player.StartManufacture(PotteryDomikId, ReceiptIds.MakeDishes);
        }

        Assert.That(player.Resource(ResourceIds.Clay), Is.EqualTo(startClay - ClayPerCycle));
    }

    /// <summary>
    /// Заповедать припас нельзя, пока Изба старосты не доросла до заповедного ларя.
    /// </summary>
    [Test]
    public void ReserveNeedsElderHouseLevelTest()
    {
        var player = TestPlayer.Create()
            .WithDomik(DomikIds.ElderHouse, 2);

        Assert.That(Throws.Business(() => player.SetReserve(ResourceIds.Clay, 1)).Message, Does.Contain("заповедного ларя"));
    }
}

file static class ElderHouseTestsActs
{
    public static TestPlayer GrantResource(this TestPlayer p, int resourceTypeId, int value)
    {
        App.Act<PlayerResourceManager>(m => m.GrantResource(p.Id, resourceTypeId, value));
        return p;
    }

    public static TestPlayer WriteOffResource(this TestPlayer p, int resourceTypeId, int value)
    {
        App.Act<PlayerResourceManager>(m => m.WriteOffResources(p.Id, [new() { Type = new() { Id = resourceTypeId }, Value = value }]));
        return p;
    }

    public static TestPlayer SetMeasure(this TestPlayer p, int manufactureId, int resourceTypeId, int value)
    {
        App.Act<DomikManager>(m => m.SetManufactureMeasure(p.Id, manufactureId, resourceTypeId, value));
        return p;
    }

    public static TestPlayer SetReserve(this TestPlayer p, int resourceTypeId, int reserve)
    {
        App.Act<ElderHouseManager>(m => m.SaveReserve(p.Id, resourceTypeId, reserve));
        return p;
    }

    public static int ManufactureCount(this TestPlayer p, int domikId)
    {
        return App.Read(context => context.Manufactures.Count(x => x.DomikPlayerId == p.Id && x.DomikId == domikId));
    }
}
