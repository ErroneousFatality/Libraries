using AndrejKrizan.Common.ValueObjects;

namespace AndrejKrizan.Common.Extensions
{
    public static class DateTimeExtensions
    {

        /// <exception cref="ArgumentException">The datetime is not in UTC format.</exception>
        public static DateTime AssertKindIsUTC(this DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("The datetime is not in UTC format.", nameof(dateTime));
            }
            return dateTime;
        }

        public static long ToUnixTimeSeconds(this DateTime dateTime)
            => new DateTimeOffset(dateTime).ToUnixTimeSeconds();

        public static long ToUnixTimeMilliseconds(this DateTime dateTime)
            => new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();

        public static DateTime TrimTo(this DateTime dateTime, DateTimeUnit precision)
        => precision switch
        {
            DateTimeUnit.Milliseconds => new DateTime(year: dateTime.Year, month: dateTime.Month, day: dateTime.Day, hour: dateTime.Hour, minute: dateTime.Minute, second: dateTime.Second, millisecond: dateTime.Millisecond, kind: dateTime.Kind),
            DateTimeUnit.Seconds => new DateTime(year: dateTime.Year, month: dateTime.Month, day: dateTime.Day, hour: dateTime.Hour, minute: dateTime.Minute, second: dateTime.Second, kind: dateTime.Kind),
            DateTimeUnit.Minutes => new DateTime(year: dateTime.Year, month: dateTime.Month, day: dateTime.Day, hour: dateTime.Hour, minute: dateTime.Minute, second: 0, kind: dateTime.Kind),
            DateTimeUnit.Hours => new DateTime(year: dateTime.Year, month: dateTime.Month, day: dateTime.Day, hour: dateTime.Hour, minute: 0, second: 0, kind: dateTime.Kind),
            DateTimeUnit.Days => new DateTime(year: dateTime.Year, month: dateTime.Month, day: dateTime.Day),
            DateTimeUnit.Weeks => new DateTime(year: dateTime.Year, month: dateTime.Month, day: dateTime.Day / 7 * 7),
            DateTimeUnit.Months => new DateTime(year: dateTime.Year, month: dateTime.Month, day: 1),
            DateTimeUnit.Years => new DateTime(year: dateTime.Year, month: 1, day: 1),
            _ => throw new ArgumentOutOfRangeException(nameof(precision)),
        };
    }
}
