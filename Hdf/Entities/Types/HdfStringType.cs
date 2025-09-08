using System.Text;

using AndrejKrizan.DotNet.Allocations;
using AndrejKrizan.Hdf.Entities.Objects;
using AndrejKrizan.Hdf.Extensions;

using HDF.PInvoke;

namespace AndrejKrizan.Hdf.Entities.Types;

public class HdfStringType : HdfObject, IHdfType<string>
{
    // Constructors
    public HdfStringType() { }

    // Methods
    public override string Describe()
        => $"string";

    public Allocation Allocate(string value)
    {
        Allocation allocation = AllocateInternal(value);
        AllocationArray allocationArray = new(allocation);
        return allocationArray;
    }

    public Allocation Allocate(IEnumerable<string> collection)
    {
        IEnumerable<Allocation> allocations = collection.Select(AllocateInternal);
        AllocationArray allocationArray = new(allocations);
        return allocationArray;
    }

    public Allocation Allocate(IEnumerable<IEnumerable<string>> matrix)
    {
        IEnumerable<Allocation> allocations = matrix.SelectMany(row => row.Select(AllocateInternal));
        AllocationArray allocationArray = new(allocations);
        return allocationArray;
    }
    public Allocation Allocate<TRow>(IEnumerable<TRow> matrix)
        where TRow : IEnumerable<string>
        => Allocate(matrix: matrix.Cast<IEnumerable<string>>());

    // Protected methods
    protected override long CreateInternal()
    {
        long id = H5T.create(H5T.class_t.STRING, H5T.VARIABLE)
            .ValidateHdfId(() => $"create a variable-length string type for {Describe()}");
        H5T.set_cset(id, H5T.cset_t.UTF8)
            .ValidateHdfResponse(() => $"set the character set to UTF8 for {Describe()}");
        H5T.set_strpad(id, H5T.str_t.NULLTERM)
            .ValidateHdfResponse(() => $"set the padding to null term for {Describe()}");
        return id;
    }

    protected override long OpenInternal()
        => CreateInternal();

    protected override int CloseInternal()
        => H5T.close(Id);

    // Private methods
    private static Allocation AllocateInternal(string value)
    {
        byte[] bytes = ConvertToBytes(value);
        Allocation allocation = new(bytes);
        return allocation;
    }

    private static byte[] ConvertToBytes(string value)
        => Encoding.UTF8.GetBytes(value + char.MinValue);
}
