using AndrejKrizan.DotNet.Bytes;

using Microsoft.Extensions.Caching.Distributed;

namespace AndrejKrizan.DotNet.DistributedCache;
public static class IDistributedCacheExtensions
{
    public static async Task SetAsync<T>(this IDistributedCache cache,
        string key, T value,
        CancellationToken cancellationToken = default
    )
        where T : struct
    {
        byte[] bytes = BytesConverter.ToBytes(value);
        await cache.SetAsync(key, bytes, cancellationToken);
    }

    public static async Task SetAsync<T>(this IDistributedCache cache,
        string key, T value, DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default
    )
        where T : struct
    {
        byte[] bytes = BytesConverter.ToBytes(value);
        await cache.SetAsync(key, bytes, options, cancellationToken);
    }


    public static async Task<T?> GetAsync<T>(this IDistributedCache cache,
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


    public static async Task<T> GetOrSetAsync<T>(this IDistributedCache cache,
        string key, Func<CancellationToken, Task<T>> createAsync,
        CancellationToken cancellationToken = default
    )
    where T : struct
    {
        T? _cache = await cache.GetAsync<T>(key, cancellationToken);
        if (_cache.HasValue)
        {
            return _cache.Value;
        }
        T value = await createAsync(cancellationToken);
        await cache.SetAsync(key, value, cancellationToken);
        return value;
    }

    public static async Task<T> GetOrSetAsync<T>(this IDistributedCache cache,
        string key, Func<CancellationToken, Task<T>> createAsync, DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default
    )
        where T : struct
    {
        T? _cache = await cache.GetAsync<T>(key, cancellationToken);
        if (_cache.HasValue)
        {
            return _cache.Value;
        }
        T value = await createAsync(cancellationToken);
        await cache.SetAsync(key, value, options, cancellationToken);
        return value;
    }

    public static async Task<T> GetOrSetAsync<T>(this IDistributedCache cache, 
        string key, Func<CancellationToken, Task<(T Value, DistributedCacheEntryOptions Options)>> createAsync, 
        CancellationToken cancellationToken = default
    )
        where T : struct
    {
        T? _cache = await cache.GetAsync<T>(key, cancellationToken);
        if (_cache.HasValue)
        {
            return _cache.Value;
        }
        (T value, DistributedCacheEntryOptions options) = await createAsync(cancellationToken);
        await cache.SetAsync(key, value, options, cancellationToken);
        return value;
    }


    public static async Task<string> GetOrSetAsync(this IDistributedCache cache,
        string key, Func<CancellationToken, Task<string>> createAsync,
        CancellationToken cancellationToken = default
    )
    {
        string? _cache = await cache.GetStringAsync(key, cancellationToken);
        if (_cache != null)
        {
            return _cache;
        }
        string value = await createAsync(cancellationToken);
        await cache.SetStringAsync(key, value, cancellationToken);
        return value;
    }

    public static async Task<string> GetOrSetAsync(this IDistributedCache cache,
        string key, Func<CancellationToken, Task<string>> createAsync, DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default
    )
    {
        string? _cache = await cache.GetStringAsync(key, cancellationToken);
        if (_cache != null)
        {
            return _cache;
        }
        string value = await createAsync(cancellationToken);
        await cache.SetStringAsync(key, value, options, cancellationToken);
        return value;
    }

    public static async Task<string> GetOrSetAsync(this IDistributedCache cache, 
        string key, Func<CancellationToken, Task<(string Value, DistributedCacheEntryOptions Options)>> createAsync, 
        CancellationToken cancellationToken = default
    )
    {
        string? _cache = await cache.GetStringAsync(key, cancellationToken);
        if (_cache != null)
        {
            return _cache;
        }
        (string value, DistributedCacheEntryOptions options) = await createAsync(cancellationToken);
        await cache.SetStringAsync(key, value, options, cancellationToken);
        return value;
    }
}
