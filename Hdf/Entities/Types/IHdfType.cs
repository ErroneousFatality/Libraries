using AndrejKrizan.DotNet.Allocations;
using AndrejKrizan.Hdf.Entities.Objects;

namespace AndrejKrizan.Hdf.Entities.Types;

public interface IHdfType<T> : IHdfObject
    where T : notnull
{
    // Methods
    Allocation Allocate(T value);
    Allocation Allocate(IEnumerable<T> collection);
    Allocation Allocate<TRow>(IEnumerable<TRow> matrix) where TRow : IEnumerable<T>;
}
