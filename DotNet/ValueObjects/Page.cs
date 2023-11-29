using System.Collections.Immutable;

using AndrejKrizan.DotNet.Collections;

namespace AndrejKrizan.DotNet.ValueObjects;

public class Page<T>
{
    // Properties
    public ImmutableArray<T> Results { get; }
    public ulong TotalCount { get; }
    public uint PageSize { get; }

    // Constructors
    public Page(ImmutableArray<T> results, ulong totalCount, uint pageSize)
    {
        Results = results;
        TotalCount = totalCount;
        PageSize = pageSize;
    }

    public Page(IEnumerable<T> results, ulong totalCount, uint pageSize)
        : this(results.ToImmutableArray(), totalCount, pageSize) { }

    // Methods
    public Page<T2> Convert<T2>(Func<T, T2> selector)
        => new Page<T2>(Results.Convert(selector), TotalCount, PageSize);

    public async Task<Page<T2>> ConvertConcurrentlyAsync<T2>(
        Func<T, CancellationToken, Task<T2>> asyncSelector,
        CancellationToken cancellationToken = default
    )
        => new Page<T2>(await Results.ConvertConcurrentlyAsync(asyncSelector, cancellationToken), TotalCount, PageSize);

    public async Task<Page<T2>> ConvertSequentiallyAsync<T2>(
        Func<T, CancellationToken, Task<T2>> asyncSelector,
        CancellationToken cancellationToken = default
    )
        => new Page<T2>(await Results.ConvertSequentiallyAsync(asyncSelector, (int)PageSize, cancellationToken), TotalCount, PageSize);
}
