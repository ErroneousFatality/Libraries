using AndrejKrizan.DotNet.ValueObjects.Pointables;
using AndrejKrizan.Hdf.Entities.Objects;

using HDF.PInvoke;

namespace AndrejKrizan.Hdf.Entities.Types;

public class HdfType<T> : HdfObject, IHdfType<T>
    where T : notnull
{
    // Constructors
    public HdfType() { }

    // Methods
    public override string Describe()
        => $"{typeof(T)}";

    public Pointable CreatePointable(T value)
    {
        byte[] bytes = ConvertToBytes(value);
        Pointable pointable = new(bytes);
        return pointable;
    }

    public Pointable CreatePointable(IEnumerable<T> collection)
    {
        byte[] bytes = collection.SelectMany(ConvertToBytes).ToArray();
        Pointable pointable = new(bytes);
        return pointable;
    }

    public Pointable CreatePointable<TRow>(IEnumerable<TRow> matrix) 
        where TRow: IEnumerable<T>
    {
        byte[] bytes = matrix.SelectMany(row => row.SelectMany(ConvertToBytes)).ToArray();
        Pointable pointable = new(bytes);
        return pointable;
    }

    // Protected methods
    protected override long CreateInternal()
        => GetTypeId(typeof(T));

    protected override long OpenInternal()
        => CreateInternal();

    protected override int CloseInternal()
        => HdfConstants.ResponseSuccessCode;

    // Private methods
    private static long GetTypeId(Type type) => type switch
    {
        Type _type when _type == typeof(byte) => H5T.NATIVE_B8,
        Type _type when _type == typeof(short) => H5T.NATIVE_INT16,
        Type _type when _type == typeof(ushort) => H5T.NATIVE_UINT16,
        Type _type when _type == typeof(int) => H5T.NATIVE_INT32,
        Type _type when _type == typeof(uint) => H5T.NATIVE_UINT32,
        Type _type when _type == typeof(long) => H5T.NATIVE_INT64,
        Type _type when _type == typeof(ulong) => H5T.NATIVE_UINT64,
        Type _type when _type == typeof(float) => H5T.NATIVE_FLOAT,
        Type _type when _type == typeof(double) => H5T.NATIVE_DOUBLE,
        Type _type when _type == typeof(char) => H5T.NATIVE_CHAR,
        Type _type when _type == typeof(string) => throw new NotSupportedException($"When using a string type, use the {nameof(HdfStringType)} method instead."),
        Type _type when _type == typeof(DateTime) => throw new NotSupportedException($"When using a datetime type, use the {nameof(HdfStringType)} method instead."),
        _ => throw new NotSupportedException($"Unsupported type: {type}."),
    };

    private static byte[] ConvertToBytes(T value)
    {
        object valueObject = value;
        byte[] bytes = typeof(T) switch
        {
            Type _type when _type == typeof(byte) => new byte[] { (byte)valueObject },
            Type _type when _type == typeof(short) => BitConverter.GetBytes((short)valueObject),
            Type _type when _type == typeof(ushort) => BitConverter.GetBytes((ushort)valueObject),
            Type _type when _type == typeof(int) => BitConverter.GetBytes((int)valueObject),
            Type _type when _type == typeof(uint) => BitConverter.GetBytes((uint)valueObject),
            Type _type when _type == typeof(long) => BitConverter.GetBytes((long)valueObject),
            Type _type when _type == typeof(ulong) => BitConverter.GetBytes((ulong)valueObject),
            Type _type when _type == typeof(Half) => BitConverter.GetBytes((Half)valueObject),
            Type _type when _type == typeof(float) => BitConverter.GetBytes((float)valueObject),
            Type _type when _type == typeof(double) => BitConverter.GetBytes((double)valueObject),
            Type _type when _type == typeof(char) => BitConverter.GetBytes((char)valueObject),
            Type _type when _type == typeof(string) => throw new NotSupportedException($"When using a string type, use the {nameof(HdfStringType)} instead."),
            Type _type when _type == typeof(DateTime) => throw new NotSupportedException($"When using a datetime type, use the {nameof(HdfDateTimeType)} instead."),
            _ => throw new NotSupportedException($"The type {typeof(T)} is not supported.")
        };
        return bytes;
    }
}
