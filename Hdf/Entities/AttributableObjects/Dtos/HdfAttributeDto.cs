using AndrejKrizan.Hdf.Entities.AttributableObjects;
using AndrejKrizan.Hdf.Entities.Types;

namespace AndrejKrizan.Hdf.Entities.AttributableObjects.Dtos
{
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
                Type _type when _type == typeof(byte) => new HDFAttributeDTO<byte>(attribute.Name, (byte)attribute.Value),
                Type _type when _type == typeof(short) => new HDFAttributeDTO<short>(attribute.Name, (short)attribute.Value),
                Type _type when _type == typeof(ushort) => new HDFAttributeDTO<ushort>(attribute.Name, (ushort)attribute.Value),
                Type _type when _type == typeof(int) => new HDFAttributeDTO<int>(attribute.Name, (int)attribute.Value),
                Type _type when _type == typeof(uint) => new HDFAttributeDTO<uint>(attribute.Name, (uint)attribute.Value),
                Type _type when _type == typeof(long) => new HDFAttributeDTO<long>(attribute.Name, (long)attribute.Value),
                Type _type when _type == typeof(ulong) => new HDFAttributeDTO<ulong>(attribute.Name, (ulong)attribute.Value),
                Type _type when _type == typeof(float) => new HDFAttributeDTO<float>(attribute.Name, (float)attribute.Value),
                Type _type when _type == typeof(double) => new HDFAttributeDTO<double>(attribute.Name, (double)attribute.Value),
                Type _type when _type == typeof(char) => new HDFAttributeDTO<char>(attribute.Name, (char)attribute.Value),
                Type _type when _type == typeof(string) => new HDFAttributeDTO<string>(attribute.Name, (string)attribute.Value),
                Type _type when _type == typeof(DateTime) => new HDFAttributeDTO<DateTime>(attribute.Name, (DateTime)attribute.Value),
                _ => throw new NotSupportedException($"The type {type} is not supported.")
            };
            return attributeDTO;
        }
    }

    public class HDFAttributeDTO<T> : HdfAttributeDto
        where T : notnull
    {
        // Properties
        public T Value { get; }

        // Constructors
        public HDFAttributeDTO(string name, T value)
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
        private HDFAttribute<string> CreateAndWriteToStringAttribute(HdfAttributableObject parent, bool dispose)
        {
            HDFAttribute<string> attribute = new(parent, Name, new HdfStringType());
            attribute.CreateAndWriteTo((string)(object)Value, dispose);
            return attribute;
        }

        private HDFAttribute<DateTime> CreateAndWriteToDateTimeAttribute(HdfAttributableObject parent, bool dispose)
        {
            HDFAttribute<DateTime> attribute = new(parent, Name, new HdfDateTimeType());
            attribute.CreateAndWriteTo((DateTime)(object)Value, dispose);
            return attribute;
        }

        private HDFAttribute<T> CreateAndWriteToGenericAttribute(HdfAttributableObject parent, bool dispose)
        {
            HDFAttribute<T> attribute = new(parent, Name);
            attribute.CreateAndWriteTo(Value, dispose);
            return attribute;
        }
    }
}