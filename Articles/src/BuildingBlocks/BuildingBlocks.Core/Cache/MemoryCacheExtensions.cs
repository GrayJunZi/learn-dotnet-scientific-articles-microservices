using Microsoft.Extensions.Caching.Memory;

namespace BuildingBlocks.Core.Cache;

public static class MemoryCacheExtensions
{
    public static T GetOrCreateByType<T>(this IMemoryCache memoryCache, Func<ICacheEntry, T> factory)
        => memoryCache.GetOrCreate(typeof(T).Name, factory);
}