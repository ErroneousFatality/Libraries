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
        using (Allocation valueAllocation = Type.Allocate(value: value))
        {
            Write(valueAllocation);
        }
    }

    public void Write(IEnumerable<T> collection)
    {
        DataSpace.Validate(collection: collection);
        using (Allocation collectionAllocation = Type.Allocate(collection: collection))
        {
            Write(collectionAllocation);
        }
    }

    public void Write(IEnumerable<IEnumerable<T>> matrix)
    {
        DataSpace.Validate(matrix: matrix);
        using (Allocation matrixAllocation = Type.Allocate(matrix: matrix))
        {
            Write(matrixAllocation);
        }
    }
    public void Write<TRow>(IEnumerable<TRow> matrix)
        where TRow : IEnumerable<T>
        => Write(matrix: matrix.Cast<IEnumerable<T>>());


    public IDisposable CreateAndWriteTo(T value, bool dispose = true)
        => CreateAndDo(() => Write(value: value), dispose);

    public IDisposable CreateAndWriteTo(IEnumerable<T> collection, bool dispose = true)
        => CreateAndDo(() => Write(collection: collection), dispose);

    public IDisposable CreateAndWriteTo(IEnumerable<IEnumerable<T>> matrix, bool dispose = true)
        => CreateAndDo(() => Write(matrix: matrix), dispose);
    public IDisposable CreateAndWriteTo<TRow>(IEnumerable<TRow> matrix, bool dispose = true)
        where TRow : IEnumerable<T>
        => CreateAndWriteTo(matrix: matrix.Cast<IEnumerable<T>>(), dispose);

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
