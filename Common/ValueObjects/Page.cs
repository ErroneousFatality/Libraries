using System.Collections.Immutable;

using AndrejKrizan.Common.Extensions;

namespace AndrejKrizan.Common.ValueObjects
{
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

        public Page(T[] results, ulong totalCount, uint pageSize)
            : this(ImmutableArray.Create(results), totalCount, pageSize) { }

        // Methods
        public Page<T2> Convert<T2>(Func<T, T2> converter)
            => new Page<T2>(Results.Convert(converter), TotalCount, PageSize);
        public async Task<Page<T2>> ConvertAsync<T2>(
            Func<T, CancellationToken, Task<T2>> asyncConverter, bool concurrently = false,
            CancellationToken cancellationToken = default
        )
            => new Page<T2>(await Results.ToImmutableArrayAsync(asyncConverter, concurrently, cancellationToken), TotalCount, PageSize);
    }
}
