using Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Infrastructure.Services
{
    public class MemoryCacheService : IMemoryCacheService
    {
        public readonly IMemoryCache _memoryCache;
        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public TResult Get<TResult>(string key)
        {
            if (_memoryCache.TryGetValue(key, out TResult result))
            {
                return result;
            }
            return default;
        }

        public void Set<TObject>(string key, TObject value)
        {
            var cacheOptions = new MemoryCacheEntryOptions();
            cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(30))
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                        .SetPriority(CacheItemPriority.Normal)
                        .SetSize(4096);

            _memoryCache.Set(key, value, cacheOptions);
        }
    }
}
