using AndrejKrizan.DotNet.ValueObjects.Pointables;
using AndrejKrizan.Hdf.Entities.AttributableObjects;
using AndrejKrizan.Hdf.Entities.Objects;
using AndrejKrizan.Hdf.Entities.Types;
using AndrejKrizan.Hdf.Extensions;

using HDF.PInvoke;

namespace AndrejKrizan.Hdf.Entities
{
    public abstract class HdfAttribute : HdfObject
    {
        // Protected constructors
        protected HdfAttribute(HdfAttributableObject parent, string name)
            : base(parent, name) { }

        // Protected methods
        protected override long OpenInternal()
            => H5A.open(Parent!.Id, Name!);

        protected override int CloseInternal()
            => H5A.close(Id);
    }

    public class HdfAttribute<T> : HdfAttribute
        where T : notnull
    {
        // Properties
        public IHdfType<T> Type { get; }
        public HdfDataSpace DataSpace { get; }

        // Constructors
        public HdfAttribute(HdfAttributableObject parent, string name, IHdfType<T> type, params ulong[] dimensions)
            : base(parent, name)
        {
            Type = type;
            DataSpace = new(dimensions);
        }
        public HdfAttribute(HdfAttributableObject parent, string name, params ulong[] dimensions)
            : this(parent, name, new HdfType<T>(), dimensions) { }

        // Methods
        public override string Describe()
            => $"{Type.Describe()} attribute";

        public void Write(T value)
        {
            DataSpace.Validate(value: value);
            using (Pointable pointable = Type.CreatePointable(value))
            {
                Write(pointable);
            }
        }

        public void Write(IEnumerable<T> collection)
        {
            DataSpace.Validate(collection: collection);
            using (Pointable pointableArray = Type.CreatePointable(collection))
            {
                Write(pointableArray);
            }
        }

        public void Write<TRow>(IEnumerable<TRow> matrix)
            where TRow: IEnumerable<T>
        {
            DataSpace.Validate<T, TRow>(matrix: matrix);
            using (Pointable pointableMatrix = Type.CreatePointable(matrix))
            {
                Write(pointableMatrix);
            }
        }

        public IDisposable CreateAndWriteTo(T value, bool dispose = true)
            => CreateAndDo(() => Write(value), dispose);

        public IDisposable CreateAndWriteTo(IReadOnlyCollection<T> collection, bool dispose = true)
            => CreateAndDo(() => Write(collection), dispose);

        public IDisposable CreateAndWriteTo(IReadOnlyCollection<IReadOnlyCollection<T>> matrix, bool dispose = true)
            => CreateAndDo(() => Write(matrix), dispose);

        // Protected methods
        protected override long CreateInternal()
        {
            using (Type.OpenOrCreate())
            using (DataSpace.OpenOrCreate())
            {
                return H5A.create(Parent!.Id, Name!, Type.Id, DataSpace.Id);
            }
        }

        protected void Write(Pointable pointable)
        {
            using (Type.OpenOrCreate())
            {
                H5A.write(
                    attr_id: Id,
                    mem_type_id: Type.Id,
                    buf: pointable.Pointer
                ).ValidateHdfResponse(() => $"write to {DescriptionWithPathName}");
            }
        }
    }
}
