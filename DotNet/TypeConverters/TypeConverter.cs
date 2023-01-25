using System.ComponentModel;
using System.Globalization;

namespace AndrejKrizan.DotNet.TypeConverters
{
    public class TypeConverter<Source, Target> : TypeConverter
    {
        // Fields
        private readonly Func<Source, Target> ToTarget;
        private readonly Func<Target, Source> FromTarget;

        // Constructors
        public TypeConverter(Func<Source, Target> convertTo, Func<Target, Source> convertFrom)
        {
            ToTarget = convertTo;
            FromTarget = convertFrom;
        }
        // Methods
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            if (sourceType == typeof(Target))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is Target targetValue)
            {
                Source sourceValue = FromTarget(targetValue);
                return sourceValue;
            }
            return base.ConvertFrom(context, culture, value);
        }


        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            if (destinationType == typeof(Target))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (destinationType == typeof(Target) && value is Source sourceValue)
            {
                Target targetValue = ToTarget(sourceValue);
                return targetValue;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
