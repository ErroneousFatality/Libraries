namespace AndrejKrizan.Hdf.Entities.AttributableObjects;

public abstract class HdfAttributeDto
{
    // Properties
    public string Name { get; }


    // Protected constructors
    protected HdfAttributeDto(string name)
    {
        Name = name;
    }

    // Internal methods
    internal abstract HdfAttribute CreateAndWriteToAttribute(HdfAttributableObject parent, bool dispose = true);

    // Converters
    public static implicit operator HdfAttributeDto((string Name, object Value) attribute)
    {
        Type type = attribute.Value.GetType();
        HdfAttributeDto attributeDTO = type switch
        {
            Type _type when _type == typeof(byte) => new HdfAttributeDto<byte>(attribute.Name, (byte)attribute.Value),
            Type _type when _type == typeof(short) => new HdfAttributeDto<short>(attribute.Name, (short)attribute.Value),
            Type _type when _type == typeof(ushort) => new HdfAttributeDto<ushort>(attribute.Name, (ushort)attribute.Value),
            Type _type when _type == typeof(int) => new HdfAttributeDto<int>(attribute.Name, (int)attribute.Value),
            Type _type when _type == typeof(uint) => new HdfAttributeDto<uint>(attribute.Name, (uint)attribute.Value),
            Type _type when _type == typeof(long) => new HdfAttributeDto<long>(attribute.Name, (long)attribute.Value),
            Type _type when _type == typeof(ulong) => new HdfAttributeDto<ulong>(attribute.Name, (ulong)attribute.Value),
            Type _type when _type == typeof(float) => new HdfAttributeDto<float>(attribute.Name, (float)attribute.Value),
            Type _type when _type == typeof(double) => new HdfAttributeDto<double>(attribute.Name, (double)attribute.Value),
            Type _type when _type == typeof(char) => new HdfAttributeDto<char>(attribute.Name, (char)attribute.Value),
            Type _type when _type == typeof(string) => new HdfAttributeDto<string>(attribute.Name, (string)attribute.Value),
            Type _type when _type == typeof(DateTime) => new HdfAttributeDto<DateTime>(attribute.Name, (DateTime)attribute.Value),
            _ => throw new NotSupportedException($"The type {type} is not supported.")
        };
        return attributeDTO;
    }
}

public class HdfAttributeDto<T> : HdfAttributeDto
    where T : notnull
{
    // Properties
    public T Value { get; }

    // Constructors
    public HdfAttributeDto(string name, T value)
        : base(name)
    {
        Value = value;
    }

    // Methods
    override internal HdfAttribute CreateAndWriteToAttribute(HdfAttributableObject parent, bool dispose = true)
        => HdfAttribute<T>.CreateAndWriteTo(parent, Name, Value, dispose);

    // Converters
    public static implicit operator HdfAttributeDto<T>((string Name, T Value) attribute)
        => new(attribute.Name, attribute.Value);
}