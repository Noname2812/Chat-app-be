using Azure.Core;
using ChatApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace ChatApp.Attributes
{
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds;
        public CacheAttribute(int timeToLiveSeconds)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var path = context.HttpContext.Request.Path;
            string? keyCache = null;
            // cache by path request
            switch (path)
            {
                case "/api/room":
                    var claimsPrincipal = context.HttpContext.User;
                    string? userId = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == "userId")?.Value;
                    keyCache = GenarateCacheKeyFromRequest(context.HttpContext.Request, userId);
                    break;
                default:
                    break;
            }
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            if(!string.IsNullOrEmpty(keyCache))
            {
                var cacheRespone = await cacheService.GetDataByKey(keyCache);
                if (!string.IsNullOrEmpty(cacheRespone))
                {
                    var contentResult = new ContentResult { ContentType = "application/json", Content = cacheRespone, StatusCode = 200 };
                    context.Result = contentResult;
                    return;
                }
            }
            var excutedConext = await next();
            // set cache
            if (excutedConext.Result is OkObjectResult ojResult && !string.IsNullOrEmpty(keyCache))
            {
                await cacheService.SetData(keyCache, ojResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
            }
        }
        private static string GenarateCacheKeyFromRequest(HttpRequest request, string? userId = null)
        {

            var keyBulder = new StringBuilder();
            if(!string.IsNullOrEmpty(userId))
            {
                keyBulder.Append($"{request.Path}:{userId}");
            }
            else
            {
                keyBulder.Append($"{request.Path}");
            }
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBulder.Append($"|{key}-{value}");
            }
            return keyBulder.ToString();
        }
    }
}
