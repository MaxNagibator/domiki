using Domiki.Web.Core;
using Domiki.Web.Infrastructure;
using Domiki.Web.Workers.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Domiki.Web.Workers;

public class WorkerController : GameControllerBase
{
    private readonly WorkerManager _workerManager;
    private readonly TavernManager _tavernManager;

    public WorkerController(DomikManager domikManager, WorkerManager workerManager, TavernManager tavernManager)
        : base(domikManager)
    {
        _workerManager = workerManager;
        _tavernManager = tavernManager;
    }

    [HttpGet]
    [Route("/Domiki/GetWorkers")]
    public WorkerDto[] GetWorkers()
    {
        var playerId = GetPlayerId();

        return _workerManager.GetWorkers(playerId).Select(x => x.ToDto()).ToArray();
    }

    /// <summary>
    /// Сохраняет правило кладовой Корчмы для одного съестного припаса игрока.
    /// </summary>
    /// <param name="resourceTypeId">Идентификатор типа ресурса – должен быть съестным.</param>
    /// <param name="reserve">Число единиц, которое Корчма не должна трогать при автоподборе.</param>
    /// <param name="forbidden"><see langword="true"/> – запретить Корчме подавать этот припас к столу.</param>
    [HttpPost]
    [Route("/Domiki/SetFoodRule/{resourceTypeId}")]
    public void SetFoodRule(int resourceTypeId, [FromQuery] int reserve, [FromQuery] bool forbidden)
    {
        var playerId = GetPlayerId();
        _tavernManager.SaveRule(playerId, resourceTypeId, reserve, forbidden);
    }
}
