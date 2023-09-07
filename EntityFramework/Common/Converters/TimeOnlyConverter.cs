using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AndrejKrizan.EntityFramework.Common.Converters;

/// <summary>
/// Converts <see cref="TimeOnly"/> to <see cref="TimeSpan"/> and vice versa.
/// </summary>
public class TimeOnlyConverter : ValueConverter<TimeOnly, TimeSpan>
{
    /// <summary>
    /// Creates a new instance of this converter.
    /// </summary>
    public TimeOnlyConverter()
        : base(
            time => time.ToTimeSpan(),
            timeSpan => TimeOnly.FromTimeSpan(timeSpan)
        )
    { }
}

/// <summary>
/// Converts <see cref="TimeOnly"/>? to <see cref="TimeSpan"/>? and vice versa.
/// </summary>
public class NullableTimeOnlyConverter : ValueConverter<TimeOnly?, TimeSpan?>
{
    /// <summary>
    /// Creates a new instance of this converter.
    /// </summary>
    public NullableTimeOnlyConverter()
        : base(
            time => time.HasValue ? time.Value.ToTimeSpan() : null,
            timeSpan => timeSpan.HasValue ? TimeOnly.FromTimeSpan(timeSpan.Value) : null
        )
    { }
}
