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
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            var keyCache = GenarateCacheKeyFromRequest(context.HttpContext.Request);
            var cacheRespone = await cacheService.GetDataByKey(keyCache);
            if (!string.IsNullOrEmpty(cacheRespone))
            {
                var contentResult = new ContentResult { ContentType = "application/json", Content = cacheRespone, StatusCode = 200 };
                context.Result = contentResult;
                return;
            }
            var excutedConext = await next();
            if (excutedConext.Result is OkObjectResult ojResult)
            {
                await cacheService.SetData(keyCache, ojResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
            }
        }
        private static string GenarateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBulder = new StringBuilder();
            keyBulder.Append($"{request.Path}");
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBulder.Append($"|{key}-{value}");

            }
            return keyBulder.ToString();
        }
    }
}
