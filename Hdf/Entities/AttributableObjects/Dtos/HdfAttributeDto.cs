using AndrejKrizan.Hdf.Entities.Types;

namespace AndrejKrizan.Hdf.Entities.AttributableObjects.Dtos;

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
    {
        HdfAttribute attribute;
        Type type = typeof(T);
        if (type == typeof(string))
        {
            attribute = CreateAndWriteToStringAttribute(parent, dispose);
        }
        else if (type == typeof(DateTime))
        {
            attribute = CreateAndWriteToDateTimeAttribute(parent, dispose);
        }
        else
        {
            attribute = CreateAndWriteToGenericAttribute(parent, dispose);
        }
        return attribute;
    }

    // Private methods
    private HdfAttribute<string> CreateAndWriteToStringAttribute(HdfAttributableObject parent, bool dispose)
    {
        HdfAttribute<string> attribute = new(parent, Name, new HdfStringType());
        attribute.CreateAndWriteTo((string)(object)Value, dispose);
        return attribute;
    }

    private HdfAttribute<DateTime> CreateAndWriteToDateTimeAttribute(HdfAttributableObject parent, bool dispose)
    {
        HdfAttribute<DateTime> attribute = new(parent, Name, new HdfDateTimeType());
        attribute.CreateAndWriteTo((DateTime)(object)Value, dispose);
        return attribute;
    }

    private HdfAttribute<T> CreateAndWriteToGenericAttribute(HdfAttributableObject parent, bool dispose)
    {
        HdfAttribute<T> attribute = new(parent, Name);
        attribute.CreateAndWriteTo(Value, dispose);
        return attribute;
    }
}