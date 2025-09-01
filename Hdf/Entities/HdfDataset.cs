using AndrejKrizan.DotNet.Allocations;
using AndrejKrizan.Hdf.Entities.AttributableObjects;
using AndrejKrizan.Hdf.Entities.Types;
using AndrejKrizan.Hdf.Extensions;

using HDF.PInvoke;

namespace AndrejKrizan.Hdf.Entities;

public class HdfDataset<T> : HdfAttributableObject
    where T : notnull
{
    // Properties
    public IHdfType<T> Type { get; }
    public HdfDataSpace DataSpace { get; }

    // Constructors
    public HdfDataset(HdfContainer parent, string name, IHdfType<T> type, ulong[] dimensions, params HdfAttributeDto[] attributes)
        : base(parent, name, attributes)
    {
        Type = type;
        DataSpace = new(dimensions);
    }
    public HdfDataset(HdfContainer parent, string name, IHdfType<T> type, params HdfAttributeDto[] attributes)
        : this(parent, name, type, dimensions: [], attributes) { }

    public HdfDataset(HdfContainer parent, string name, ulong[] dimensions, params HdfAttributeDto[] attributes)
        : this(parent, name, type: HdfTypeFactory.Create<T>(), dimensions, attributes) { }
    public HdfDataset(HdfContainer parent, string name, params HdfAttributeDto[] attributes)
        : this(parent, name, type: HdfTypeFactory.Create<T>(), dimensions: [], attributes) { }

    // Methods
    public override string Describe()
        => $"{Type.Describe()} dataset";

    public void Write(T value)
    {
        DataSpace.Validate(value: value);
        using (Allocation valueAllocation = Type.Allocate(value))
        {
            Write(valueAllocation);
        }
    }

    public void Write(IEnumerable<T> collection)
    {
        DataSpace.Validate(collection: collection);
        using (Allocation collectionAllocation = Type.Allocate(collection))
        {
            Write(collectionAllocation);
        }
    }

    public void Write<TRow>(IEnumerable<TRow> matrix)
        where TRow : IEnumerable<T>
    {
        DataSpace.Validate<T, TRow>(matrix: matrix);
        using (Allocation matrixAllocation = Type.Allocate(matrix))
        {
            Write(matrixAllocation);
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

    protected void Write(Allocation allocation)
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
                buf: allocation.Pointer
            ).ValidateHdfResponse(() => $"write to {DescriptionWithPathName}");
        }
    }
}
