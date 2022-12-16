using AndrejKrizan.Common.ValueObjects.Pointables;
using AndrejKrizan.Hdf.Entities.Objects;

namespace AndrejKrizan.Hdf.Entities.Types
{
    public interface IHdfType<T> : IHdfObject
        where T : notnull
    {
        // Methods
        Pointable CreatePointable(T value);
        Pointable CreatePointable(IReadOnlyCollection<T> collection);
        Pointable CreatePointable(IReadOnlyCollection<IReadOnlyCollection<T>> matrix);
    }
}
