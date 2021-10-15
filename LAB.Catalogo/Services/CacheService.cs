using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LAB.Catalogo.Services
{
    public delegate T GetDataToCache<T>();

    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(string cachekey, GetDataToCache<T> dataToCache) where T : class;
    }

    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _redis;
        private readonly RedisConfig _redisConfig;
        private readonly ILoggerService _logger;

        public CacheService(
            IDistributedCache redis, 
            IOptions<RedisConfig> redisConfig, 
            ILoggerService logger
            )
        {
            _redis = redis;
            _redisConfig = redisConfig.Value;
            _logger = logger;
        }

        public async Task<T> GetOrCreateAsync<T>(string cachekey, GetDataToCache<T> dataToCache) where T : class
        {
            var resultCache = await _redis.GetStringAsync(cachekey);
            if (!string.IsNullOrEmpty(resultCache))
            {
                _logger.Info("Dados recuperados do cache");
                return JsonSerializer.Deserialize<T>(resultCache);
            }

            var noCached = dataToCache?.Invoke();
            await _redis.SetStringAsync(cachekey, JsonSerializer.Serialize(noCached), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_redisConfig.AbsoluteExpirationRelativeToNowFromSeconds),
                SlidingExpiration = TimeSpan.FromSeconds(_redisConfig.SlidingExpirationFromSeconds)
            });
            _logger.Info("Dados recuperados do banco de dados");
            return noCached;
        }
    }

    public class RedisConfig
    {
        public string InstanceName { get; set; }
        public string Configuration { get; set; }
        public double AbsoluteExpirationRelativeToNowFromSeconds { get; internal set; }
        public double SlidingExpirationFromSeconds { get; internal set; }
    }

    public static class CacheConfig
    {
        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration config)
        {
            var section = config.GetSection(nameof(RedisConfig));
            services.Configure<RedisConfig>(section);
            var redisConfig = section.Get<RedisConfig>();

            services.AddStackExchangeRedisCache(options =>
            {
                options.InstanceName = redisConfig.InstanceName;
                options.Configuration = redisConfig.Configuration;
            });

            services.AddScoped<ICacheService, CacheService>();
            return services;
        }
    }
}