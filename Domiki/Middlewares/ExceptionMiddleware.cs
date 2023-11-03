using Domiki.Web.Business.Core;
using Domiki.Web.Data;
using Newtonsoft.Json;

namespace Domiki.Web
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // IMessageWriter is injected into InvokeAsync
        public async Task InvokeAsync(HttpContext httpContext, UnitOfWork uow)
        {
            try
            {
                await _next(httpContext);
            }
            catch (BusinessException ex)
            {
                var jsonString = JsonConvert.SerializeObject(new Response<string> { Type = ResponseType.ErrorMessage, Content = ex.Message });
                await httpContext.Response.WriteAsync(jsonString);
            }
        }
    }
}
