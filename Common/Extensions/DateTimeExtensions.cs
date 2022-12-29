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
            DateTimeUnit.Weeks => dateTime.TrimToStartOfWeek(),
            DateTimeUnit.Months => new DateTime(year: dateTime.Year, month: dateTime.Month, day: 1),
            DateTimeUnit.Years => new DateTime(year: dateTime.Year, month: 1, day: 1),
            _ => throw new ArgumentOutOfRangeException(nameof(precision)),
        };
        private static DateTime TrimToStartOfWeek(this DateTime dateTime, DayOfWeek weekStartDay)
        {
            int daysSinceStartOfWeek = dateTime.GetDaysSinceStartOfWeek(weekStartDay);
            int day = dateTime.Day - daysSinceStartOfWeek;
            if (day >= 1)
            {
                return new DateTime(year: dateTime.Year, month: dateTime.Month, day: day);
            }
            DateTime startOfWeek = dateTime.AddDays(-daysSinceStartOfWeek);
            DateTime trimmedStartOfWeek = new(year: startOfWeek.Year, month: startOfWeek.Month, day: startOfWeek.Day);
            return trimmedStartOfWeek;
        }
        private static DateTime TrimToStartOfWeek(this DateTime dateTime)
            => dateTime.TrimToStartOfWeek(Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek);

        public static int GetDaysSinceStartOfWeek(this DateTime dateTime, DayOfWeek weekStartDay)
        {
            int weekDayNumber = dateTime.DayOfWeek - weekStartDay;
            if (weekDayNumber < 0)
            {
                weekDayNumber += 7;
            }
            return weekDayNumber;
        }
        public static int GetDaysSinceStartOfWeek(this DateTime dateTime)
            => dateTime.GetDaysSinceStartOfWeek(Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
    }
}
