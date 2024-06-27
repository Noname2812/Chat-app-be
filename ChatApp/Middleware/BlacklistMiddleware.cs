
using ChatApp.Services;

namespace ChatApp.Middleware
{
    public class BlacklistMiddleware
    {
        private readonly RequestDelegate _next;

        public BlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var cacheService = context.RequestServices.GetRequiredService<ICacheService>();
            var cacheRespone = await cacheService.GetDataByKey($"black-list-token:{token}");
            if (!string.IsNullOrWhiteSpace(token) && !string.IsNullOrEmpty(cacheRespone))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Token has been blacklisted");
                return;
            }
            await _next(context);
        }
    }
}
