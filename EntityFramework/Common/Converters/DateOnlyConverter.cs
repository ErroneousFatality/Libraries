using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AndrejKrizan.EntityFramework.Common.Converters;

/// <summary>
/// Converts <see cref="DateOnly" /> to <see cref="DateTime"/> and vice versa.
/// </summary>
public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
{
    /// <summary>
    /// Creates a new instance of this converter.
    /// </summary>
    public DateOnlyConverter()
        : base(
            date => date.ToDateTime(TimeOnly.MinValue),
            dateTime => DateOnly.FromDateTime(dateTime)
        )
    { }
}

/// <summary>
/// Converts <see cref="DateOnly" />? to <see cref="DateTime"/>? and vice versa.
/// </summary>
public class NullableDateOnlyConverter : ValueConverter<DateOnly?, DateTime?>
{
    /// <summary>
    /// Creates a new instance of this converter.
    /// </summary>
    public NullableDateOnlyConverter()
        : base(
            date => date.HasValue ? date.Value.ToDateTime(TimeOnly.MinValue) : null,
            dateTime => dateTime.HasValue ? DateOnly.FromDateTime(dateTime.Value) : null
        )
    { }
}
