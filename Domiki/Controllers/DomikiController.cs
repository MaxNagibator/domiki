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
        private readonly Holder _holder;

        public DomikiController(ILogger<DomikiController> logger, Holder holder)
        {
            _logger = logger;
            _holder = holder;
        }

        [HttpGet]
        [Route("/Domiki/GetDomikTypes")] // todo разобраться с роут префиксом
        public IEnumerable<DomikTypeDto> GetDomikTypes()
        {
            DomikTypeDto ToDto(DomikType t)
            {
                return new DomikTypeDto { Id = t.Id, Name = t.Name, LogicName = t.LogicName };
            }

            return _holder.DomikTypes.Select(x => ToDto(x)).ToArray();
        }

        [HttpGet]
        [Route("/Domiki/GetDomiks")]
        public IEnumerable<DomikDto> GetDomiks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var playerId = _holder.GetPlayerId(userId);

            DomikDto ToDto(Domik domik)
            {
                return new DomikDto { Id = domik.Id, Level = domik.Level, TypeId = domik.Type.Id };
            }

            return _holder.Domiki.Where(x => x.PlayerId == playerId).Select(x => ToDto(x)).ToArray();
        }

        [HttpPost]
        [Route("/Domiki/UpgradeDomik/{id}")]
        public void UpgradeDomik(int id)
        {
            _holder.UpgradeModik(id);
        }
    }
}