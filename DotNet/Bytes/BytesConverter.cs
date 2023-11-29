namespace AndrejKrizan.DotNet.Bytes;
public static class BytesConverter
{
    public static byte[] ToBytes<T>(T value)
        where T : struct
    => value switch
    {
        bool _bool => BitConverter.GetBytes(_bool),
        byte _byte => [_byte],
        short _short => BitConverter.GetBytes(_short),
        ushort _ushort => BitConverter.GetBytes(_ushort),
        int _int => BitConverter.GetBytes(_int),
        uint _uint => BitConverter.GetBytes(_uint),
        long _long => BitConverter.GetBytes(_long),
        ulong _ulong => BitConverter.GetBytes(_ulong),
        Half _half => BitConverter.GetBytes(_half),
        float _float => BitConverter.GetBytes(_float),
        double _double => BitConverter.GetBytes(_double),
        char _char => BitConverter.GetBytes(_char),
        _ => throw new NotSupportedException($"The type {typeof(T)} is not supported.")
    };

    public static T FromBytes<T>(byte[] bytes)
        where T : struct
    {
        Type type = typeof(T);
        object value = type switch
        {
            _ when type == typeof(bool) => BitConverter.ToBoolean(bytes),
            _ when type == typeof(byte) => bytes.Single(),
            _ when type == typeof(short) => BitConverter.ToInt16(bytes),
            _ when type == typeof(ushort) => BitConverter.ToUInt16(bytes),
            _ when type == typeof(int) => BitConverter.ToInt32(bytes),
            _ when type == typeof(uint) => BitConverter.ToUInt32(bytes),
            _ when type == typeof(long) => BitConverter.ToInt64(bytes),
            _ when type == typeof(ulong) => BitConverter.ToUInt64(bytes),
            _ when type == typeof(Half) => BitConverter.ToHalf(bytes),
            _ when type == typeof(float) => BitConverter.ToSingle(bytes),
            _ when type == typeof(double) => BitConverter.ToDouble(bytes),
            _ when type == typeof(char) => BitConverter.ToChar(bytes),
            _ => throw new NotSupportedException($"Unsupported type: {type}."),
        };
        return (T)value;
    }
}
