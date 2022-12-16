using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AndrejKrizan.EntityFramework.Common.Converters
{
    public class DateTimeUTCConverter : ValueConverter<DateTime, DateTime>
    {
        public DateTimeUTCConverter(ConverterMappingHints? mappingHints = null)
            : base(
                (domainValue) => domainValue.ToUniversalTime(),
                (dataValue) => DateTime.SpecifyKind(dataValue, DateTimeKind.Utc),
                mappingHints
            )
        { }

        public DateTimeUTCConverter() : this(null) { }

        public static ValueConverterInfo DefaultInfo { get; }
            = new ValueConverterInfo(typeof(DateTime), typeof(DateTime), info => new DateTimeUTCConverter(info.MappingHints));
    }
}
