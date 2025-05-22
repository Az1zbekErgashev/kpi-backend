using Kpi.Domain.Models.Response;
using Kpi.Service.Exception;

namespace Kpi.Api.Middlewres
{
    public class KpiExceptionMiddlewares
    {
        private readonly RequestDelegate next;
        public KpiExceptionMiddlewares(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (KpiException ex)
            {
                await HandleException(context, ex.Code, ex.Message, ex.Global);
            }
            catch (Exception ex)
            {
                await HandleException(context, 500, "", true);
            }
        }

        public async Task HandleException(HttpContext context, int code, string message, bool? Global)
        {
            context.Response.StatusCode = code;
            await context.Response.WriteAsJsonAsync(new
            ResponseModel<string>
            {
                Status = false,
                Error = message,
                Data = null,
                Global = Global
            });
        }
    }
}
