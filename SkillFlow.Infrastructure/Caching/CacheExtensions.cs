using Microsoft.Extensions.Caching.Memory;

namespace SkillFlow.Infrastructure.Caching
{
    public static class CacheExtensions
    {
        public static async Task<T> GetOrCreateAsync<T>(
            this IMemoryCache cache,
            string key,
            TimeSpan ttl,
            Func<Task<T>> factory)
        {
            if (cache.TryGetValue(key, out T? cached) && cached is not null)
                return cached;

            var value = await factory();

            cache.Set(key, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            });

            return value;
        }
    }
}
