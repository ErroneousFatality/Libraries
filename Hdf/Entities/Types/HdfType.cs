using AndrejKrizan.DotNet.Pointables;
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
        where TRow : IEnumerable<T>
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
    private static long GetTypeId(Type type)
        => type switch
        {
            _ when type == typeof(bool) => H5T.NATIVE_HBOOL,
            _ when type == typeof(byte) => H5T.NATIVE_B8,
            _ when type == typeof(short) => H5T.NATIVE_INT16,
            _ when type == typeof(ushort) => H5T.NATIVE_UINT16,
            _ when type == typeof(int) => H5T.NATIVE_INT32,
            _ when type == typeof(uint) => H5T.NATIVE_UINT32,
            _ when type == typeof(long) => H5T.NATIVE_INT64,
            _ when type == typeof(ulong) => H5T.NATIVE_UINT64,
            _ when type == typeof(float) => H5T.NATIVE_FLOAT,
            _ when type == typeof(double) => H5T.NATIVE_DOUBLE,
            _ when type == typeof(char) => H5T.NATIVE_CHAR,
            _ when type == typeof(string) => throw new NotSupportedException($"When using a string type, use the {nameof(HdfStringType)} method instead."),
            _ when type == typeof(DateTime) => throw new NotSupportedException($"When using a datetime type, use the {nameof(HdfStringType)} method instead."),
            _ => throw new NotSupportedException($"Unsupported type: {type}."),
        };

    private static byte[] ConvertToBytes(T value)
        => value switch
        {
            bool _value => BitConverter.GetBytes(_value),
            byte _value => [_value],
            short _value => BitConverter.GetBytes(_value),
            ushort _value => BitConverter.GetBytes(_value),
            int _value => BitConverter.GetBytes(_value),
            uint _value => BitConverter.GetBytes(_value),
            long _value => BitConverter.GetBytes(_value),
            ulong _value => BitConverter.GetBytes(_value),
            Half _value => BitConverter.GetBytes(_value),
            float _value => BitConverter.GetBytes(_value),
            double _value => BitConverter.GetBytes(_value),
            char _value => BitConverter.GetBytes(_value),
            string _value => throw new NotSupportedException($"When using a string type, use the {nameof(HdfStringType)} instead."),
            DateTime _value => throw new NotSupportedException($"When using a datetime type, use the {nameof(HdfDateTimeType)} instead."),
            _ => throw new NotSupportedException($"The type {typeof(T)} is not supported.")
        };
}
