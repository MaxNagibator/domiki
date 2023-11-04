using Domiki.Web.Business.Core;
using Domiki.Web.Business.Models;
using Domiki.Web;
using Domiki.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Domiki.Controllers
{
    [Authorize]
    [ApiController]
    public class DomikiController : ControllerBase
    {
        private readonly ILogger<DomikiController> _logger;
        private readonly DomikManager _domikManager;

        public DomikiController(ILogger<DomikiController> logger, DomikManager holder)
        {
            _logger = logger;
            _domikManager = holder;
        }

        [HttpGet]
        [Route("/Domiki/GetDomikTypes")] // todo разобраться с роут префиксом
        public IEnumerable<DomikTypeDto> GetDomikTypes()
        {
            return _domikManager.GetDomikTypes().Select(x => ToDto(x)).ToArray();
        }

        [HttpGet]
        [Route("/Domiki/GetDomiks")]
        public IEnumerable<DomikDto> GetDomiks()
        {
            int playerId = GetPlayerId();

            return _domikManager.GetDomiks(playerId).Select(x => ToDto(x)).ToArray();

            DomikDto ToDto(Domik domik)
            {
                return new DomikDto { Id = domik.Id, Level = domik.Level, TypeId = domik.Type.Id };
            }
        }

        [HttpPost]
        [Route("/Domiki/UpgradeDomik/{id}")]
        public Response UpgradeDomik(int id)
        {
            _domikManager.UpgradeDomik(id);
            return new Response { Type = ResponseType.Success };
        }

        [HttpGet]
        [Route("/Domiki/GetPurchaseAvaialableDomiks")]
        public IEnumerable<DomikTypeDto> GetPurchaseAvaialableDomiks()
        {
            int playerId = GetPlayerId();

            return _domikManager.GetPurchaseAvailableDomiks(playerId).Select(x => ToDto(x)).ToArray();
        }

        private DomikTypeDto ToDto(DomikType t)
        {
            return new DomikTypeDto { Id = t.Id, Name = t.Name, LogicName = t.LogicName, MaxCount = t.MaxCount, MaxLevel = t.MaxLevel };
        }

        [HttpPost]
        [Route("/Domiki/BuyDomik/{typeId}")]
        public Response BuyDomik(int typeId)
        {
            int playerId = GetPlayerId();
            _domikManager.BuyDomik(playerId, typeId);
            return new Response { Type = ResponseType.Success };
        }

        [HttpGet]
        [Route("/Domiki/GetResourceTypes")]
        public IEnumerable<ResourceTypeDto> GetResourceTypes()
        {
            return _domikManager.GetResourceTypes().Select(x => ToDto(x)).ToArray();

            ResourceTypeDto ToDto(ResourceType resourceType)
            {
                return new ResourceTypeDto { Id = resourceType.Id, LogicName = resourceType.LogicName, Name = resourceType.Name };
            }
        }

        [HttpGet]
        [Route("/Domiki/GetResources")]
        public IEnumerable<ResourceDto> GetResources()
        {
            int playerId = GetPlayerId();

            return _domikManager.GetResources(playerId).Select(x => ToDto(x)).ToArray();

            ResourceDto ToDto(Resource res)
            {
                return new ResourceDto { Value = res.Value, TypeId = res.Type.Id };
            }
        }

        private int GetPlayerId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var playerId = _domikManager.GetPlayerId(userId);
            return playerId;
        }
    }
}