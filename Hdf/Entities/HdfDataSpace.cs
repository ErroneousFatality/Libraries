using System.Collections;

using AndrejKrizan.DotNet.Extensions;
using AndrejKrizan.Hdf.Entities.Objects;

using HDF.PInvoke;

namespace AndrejKrizan.Hdf.Entities;

public class HdfDataSpace : HdfObject
{
    // Properties
    public ulong[] Dimensions { get; }

    // Constructors
    internal HdfDataSpace(params ulong[] dimensions)
    {
        if (dimensions.Any(dimension => dimension < 1))
        {
            throw new ArgumentException("The dimensions must be positive integers.");
        }
        Dimensions = dimensions;
    }

    // Methods
    public override string Describe()
        => $"data space{(IsScalar() ? string.Empty : $" [{Dimensions.StringJoin()}]")}";

    public bool IsScalar() => Dimensions.Length == 0;

    public void Validate<T>(T value)
        where T : notnull
    {
        if (!IsScalar())
        {
            throw new NotSupportedException("This method can only be called for scalar (0-dimensional) datasets.");
        }
    }

    public void Validate<T>(IEnumerable<T> collection)
    {
        if (Dimensions.Length != 1)
        {
            throw new NotSupportedException("This method can only be called for 1-dimensional datasets.");
        }
        int count = collection.Count();
        if (Dimensions[0] != (ulong)count)
        {
            throw new ArgumentException($"The size of the collection ({count}) does not match the dimensions ({Dimensions[0]}) of the dataset.");
        }
    }

    public void Validate<T, TRow>(IEnumerable<TRow> matrix)
        where TRow: IEnumerable<T>
    {
        if (Dimensions.Length != 2)
        {
            throw new NotSupportedException("This method can only be called for 2-dimensional datasets.");
        }
        int height = matrix.Count();
        int width = matrix.FirstOrDefault()?.Count() ?? 0;
        if (matrix.Any(row => row.Count() != width))
        {
            throw new ArgumentException("The matrix's rows don't have the same length.");
        }
        if (Dimensions[0] != (ulong)height || Dimensions[1] != (ulong)width)
        {
            throw new ArgumentException($"The size of the matrix ({height}, {width}) does not match the dimensions ({Dimensions.StringJoin()}) of the dataset.");
        }
    }

    // Protected methods
    protected override long CreateInternal()
        => IsScalar()
            ? H5S.create(H5S.class_t.SCALAR)
            : H5S.create_simple(rank: Dimensions.Length, dims: Dimensions, maxdims: Dimensions);

    protected override long OpenInternal()
        => CreateInternal();

    protected override int CloseInternal()
        => H5S.close(Id);
}
