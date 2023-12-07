using Domiki.Web;
using Domiki.Web.Business.Core;
using Domiki.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Domiki.Controllers
{
    [ApiController]
    public class SystemController : ControllerBase
    {
        private readonly ILogger<DomikiController> _logger;
        private readonly DomikManager _domikManager;
        private readonly ResourceManager _resourceManager;

        public SystemController(ILogger<DomikiController> logger, DomikManager domikManager, ResourceManager resourceManager)
        {
            _logger = logger;
            _domikManager = domikManager;
            _resourceManager = resourceManager;
        }

        [HttpGet]
        [Route("/System/Test")]
        public Response<DomikTypeDto[]> GetDomikTypes()
        {
            var content = _resourceManager.GetDomikTypes().Select(x => x.ToDto()).ToArray();
            return new Response<DomikTypeDto[]>(content);
        }
    }
}