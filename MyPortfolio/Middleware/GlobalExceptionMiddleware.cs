using System.Net;

namespace MyPortfolio.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

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
                // Zaten error sayfasindaysak donguden kacin
                if (httpContext.Request.Path.StartsWithSegments("/Error"))
                {
                    httpContext.Response.StatusCode = 500;
                    await httpContext.Response.WriteAsync("Sunucu hatasi olustu.");
                    return;
                }

                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Response baslamadiysa yonlendir
            if (!context.Response.HasStarted)
            {
                context.Response.Redirect("/Error/500");
            }
            await Task.CompletedTask;
        }
    }
}