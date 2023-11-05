using Domiki.Web;
using Domiki.Web.Business.Core;
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
        public Response<DomikTypeDto[]> GetDomikTypes()
        {
            var content = _domikManager.GetDomikTypes().Select(x => x.ToDto()).ToArray();
            return new Response<DomikTypeDto[]>(content);
        }

        [HttpGet]
        [Route("/Domiki/GetDomiks")]
        public Response<DomikDto[]> GetDomiks()
        {
            int playerId = GetPlayerId();

            var content = _domikManager.GetDomiks(playerId).Select(x => x.ToDto()).ToArray();
            return new Response<DomikDto[]>(content);
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
        public Response<DomikTypeDto[]> GetPurchaseAvaialableDomiks()
        {
            int playerId = GetPlayerId();

            var content = _domikManager.GetPurchaseAvailableDomiks(playerId).Select(x => x.ToDto()).ToArray();
            return new Response<DomikTypeDto[]>(content);
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
        public Response<ResourceTypeDto[]> GetResourceTypes()
        {
            var content = _domikManager.GetResourceTypes().Select(x => x.ToDto()).ToArray();
            return new Response<ResourceTypeDto[]>(content);
        }

        [HttpGet]
        [Route("/Domiki/GetResources")]
        public Response<ResourceDto[]> GetResources()
        {
            int playerId = GetPlayerId();

            var content = _domikManager.GetResources(playerId).Select(x => x.ToDto()).ToArray();
            return new Response<ResourceDto[]>(content);
        }

        private int GetPlayerId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var playerId = _domikManager.GetPlayerId(userId);
            return playerId;
        }
    }
}