using Domiki.Web.Business.Models;

namespace Domiki.Web.Models
{
    public static class DomikDtoExtentions
    {
        public static DomikDto ToDto(this Domik domik)
        {
            return new DomikDto
            {
                Id = domik.Id,
                Level = domik.Level,
                TypeId = domik.Type.Id,
// todo вынести в нормальный хэлпер или в БД с часовым поясом хранить, или ещё чего
                FinishDate = domik.FinishDate == null ? null : DateTime.SpecifyKind(domik.FinishDate.Value, DateTimeKind.Utc)
            };
        }
    }
}