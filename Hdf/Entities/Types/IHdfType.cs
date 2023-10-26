using AndrejKrizan.DotNet.Pointables;
using AndrejKrizan.Hdf.Entities.Objects;

namespace AndrejKrizan.Hdf.Entities.Types;

public interface IHdfType<T> : IHdfObject
    where T : notnull
{
    // Methods
    Pointable CreatePointable(T value);
    Pointable CreatePointable(IEnumerable<T> collection);
    Pointable CreatePointable<TRow>(IEnumerable<TRow> matrix) where TRow : IEnumerable<T>;
}
