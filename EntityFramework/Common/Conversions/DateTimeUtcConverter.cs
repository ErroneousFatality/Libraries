using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AndrejKrizan.EntityFramework.Common.Conversions;

public class DateTimeUtcConverter : ValueConverter<DateTime, DateTime>
{
    public DateTimeUtcConverter(ConverterMappingHints? mappingHints = null)
        : base(
            (domainValue) => domainValue.ToUniversalTime(),
            (dataValue) => DateTime.SpecifyKind(dataValue, DateTimeKind.Utc),
            mappingHints
        )
    { }

    public DateTimeUtcConverter() : this(null) { }

    public static ValueConverterInfo DefaultInfo { get; }
        = new ValueConverterInfo(typeof(DateTime), typeof(DateTime), info => new DateTimeUtcConverter(info.MappingHints));
}
