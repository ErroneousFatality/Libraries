using AndrejKrizan.Common.ValueObjects.Pointables;
using AndrejKrizan.Hdf.Entities.AttributableObjects;
using AndrejKrizan.Hdf.Entities.AttributableObjects.Dtos;
using AndrejKrizan.Hdf.Entities.Objects;
using AndrejKrizan.Hdf.Entities.Types;
using AndrejKrizan.Hdf.Extensions;

using HDF.PInvoke;

namespace AndrejKrizan.Hdf.Entities
{
    public class HdfDataset<T> : HdfAttributableObject
        where T : notnull
    {
        // Properties
        public IHdfType<T> Type { get; }
        public HdfDataSpace DataSpace { get; }

        // Constructors
        public HdfDataset(HdfObject parent, string name, IHdfType<T> type, ulong[] dimensions, params HdfAttributeDto[] attributes)
            : base(parent, name, attributes)
        {
            Type = type;
            DataSpace = new(dimensions);
        }
        public HdfDataset(HdfObject parent, string name, IHdfType<T> type, params HdfAttributeDto[] attributes)
            : this(parent, name, type, Array.Empty<ulong>(), attributes) { }

        public HdfDataset(HdfObject parent, string name, ulong[] dimensions, params HdfAttributeDto[] attributes)
            : this(parent, name, new HDFType<T>(), dimensions, attributes) { }
        public HdfDataset(HdfObject parent, string name, params HdfAttributeDto[] attributes)
            : this(parent, name, new HDFType<T>(), Array.Empty<ulong>(), attributes) { }

        // Methods
        public override string Describe()
            => $"{Type.Describe()} dataset";

        public void Write(T value)
        {
            DataSpace.Validate(value);
            using (Pointable pointable = Type.CreatePointable(value))
            {
                Write(pointable);
            }
        }

        public void Write(IReadOnlyCollection<T> collection)
        {
            DataSpace.Validate(collection);
            using (Pointable pointableArray = Type.CreatePointable(collection))
            {
                Write(pointableArray);
            }
        }

        public void Write(IReadOnlyCollection<IReadOnlyCollection<T>> matrix)
        {
            DataSpace.Validate(matrix);
            using (Pointable pointableMatrix = Type.CreatePointable(matrix))
            {
                Write(pointableMatrix);
            }
        }

        // Protected methods
        protected override long CreateInternal()
        {
            using (Type.OpenOrCreate())
            using (DataSpace.Create())
            {
                return H5D.create(Parent!.Id, Name!, Type.Id, DataSpace.Id);
            }
        }

        protected override long OpenInternal()
            => H5D.open(Parent!.Id, Name!);

        protected override int CloseInternal()
            => H5D.close(Id);

        protected void Write(Pointable pointable)
        {
            using (Type.Open())
            using (DataSpace.Open())
            {
                H5D.write(
                    dset_id: Id,
                    mem_type_id: Type.Id,
                    mem_space_id: DataSpace.Id,
                    file_space_id: H5S.ALL,
                    plist_id: H5P.DEFAULT,
                    buf: pointable.Pointer
                ).ValidateHDFResponse(() => $"write to {DescriptionWithPathName}");
            }
        }
    }
}
