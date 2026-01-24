using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace MyPortfolio.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        // Logger eklenebilir, simdilik basic tutuyoruz.

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // Hata loglama islemi burada yapilabilir
                // _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "text/html";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // 500 hatasina yonlendir
            context.Response.Redirect("/Error/500");
            await Task.CompletedTask;
        }
    }
}
