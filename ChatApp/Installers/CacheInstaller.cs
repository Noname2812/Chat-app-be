
using ChatApp.Configurations;
using ChatApp.Services;
using StackExchange.Redis;

namespace ChatApp.Installers
{
    public class CacheInstaller : IInstaller
    {
        public void InstallerService(IServiceCollection services, IConfiguration configuration)
        {
            var redisConfig = new RedisSettings();
            configuration.GetSection("RedisConfiguration").Bind(redisConfig);
            services.AddSingleton(redisConfig);
            if (!redisConfig.Enable)
            {
                return;
            }
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConfig.ConnectionString));
            services.AddStackExchangeRedisCache(options => options.Configuration = redisConfig.ConnectionString);
            services.AddSingleton<ICacheService, CacheService>();
        }
    }
}
