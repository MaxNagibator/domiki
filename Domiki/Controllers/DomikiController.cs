using Domiki.Business.Core;
using Domiki.Business.Models;
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

            DomikTypeDto ToDto(DomikType t)
            {
                return new DomikTypeDto { Id = t.Id, Name = t.Name, LogicName = t.LogicName };
            }
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
        public void UpgradeDomik(int id)
        {
            _domikManager.UpgradeDomik(id);
        }

        [HttpGet]
        [Route("/Domiki/GetPurchaseAvaialableDomiks")]
        public IEnumerable<DomikTypeDto> GetPurchaseAvaialableDomiks()
        {
            int playerId = GetPlayerId();

            return _domikManager.GetPurchaseAvailableDomiks(playerId).Select(x => ToDto(x)).ToArray();

            DomikTypeDto ToDto(DomikType t)
            {
                return new DomikTypeDto { Id = t.Id, Name = t.Name, LogicName = t.LogicName };
            }
        }

        [HttpPost]
        [Route("/Domiki/BuyDomik/{typeId}")]
        public void BuyDomik(int typeId)
        {
            int playerId = GetPlayerId();

            _domikManager.BuyDomik(playerId, typeId);
        }

        private int GetPlayerId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var playerId = _domikManager.GetPlayerId(userId);
            return playerId;
        }
    }
}