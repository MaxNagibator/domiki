using Domiki.Web.Business.Models;

namespace Domiki.Web.Models
{
    public static class DomikDtoExtentions
    {
        public static DomikDto ToDto(this Domik domik)
        {
            return new DomikDto { Id = domik.Id, Level = domik.Level, TypeId = domik.Type.Id };
        }
    }
}