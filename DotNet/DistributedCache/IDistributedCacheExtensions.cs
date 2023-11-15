using AndrejKrizan.DotNet.Utilities;

using Microsoft.Extensions.Caching.Distributed;

namespace AndrejKrizan.DotNet.DistributedCache;
public static class IDistributedCacheExtensions
{
    public static async Task SetValueAsync<T>(this IDistributedCache cache,
        string key, T value,
        CancellationToken cancellationToken = default
    )
        where T : struct
    {
        byte[] bytes = BytesConverter.ToBytes(value);
        await cache.SetAsync(key, bytes, cancellationToken);
    }

    public static async Task SetValueAsync<T>(this IDistributedCache cache,
        string key, T value, DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default
    )
        where T : struct
    {
        byte[] bytes = BytesConverter.ToBytes(value);
        await cache.SetAsync(key, bytes, options, cancellationToken);
    }


    public static async Task<T?> GetValueAsync<T>(this IDistributedCache cache,
        string key,
        CancellationToken cancellationToken = default
    )
        where T : struct
    {
        byte[]? bytes = await cache.GetAsync(key, cancellationToken);
        if (bytes == null)
        {
            return null;
        }
        T value = BytesConverter.FromBytes<T>(bytes);
        return value;
    }


    public static async Task<T> GetOrCreateValueAsync<T>(this IDistributedCache cache,
        string key, Func<CancellationToken, Task<T>> createAsync,
        CancellationToken cancellationToken = default
    )
    where T : struct
    {
        T? _cache = await cache.GetValueAsync<T>(key, cancellationToken);
        if (_cache.HasValue)
        {
            return _cache.Value;
        }
        T value = await createAsync(cancellationToken);
        await cache.SetValueAsync(key, value, cancellationToken);
        return value;
    }

    public static async Task<T> GetOrCreateValueAsync<T>(this IDistributedCache cache,
        string key, Func<CancellationToken, Task<T>> createAsync, DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default
    )
        where T : struct
    {
        T? _cache = await cache.GetValueAsync<T>(key, cancellationToken);
        if (_cache.HasValue)
        {
            return _cache.Value;
        }
        T value = await createAsync(cancellationToken);
        await cache.SetValueAsync(key, value, options, cancellationToken);
        return value;
    }
}
