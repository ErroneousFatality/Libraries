namespace AndrejKrizan.Common.ValueObjects
{
    public enum DateTimeUnit : byte
    {
        Ticks = 1,
        Microseconds = 2,
        Milliseconds = 3,
        Seconds = 4,
        Minutes = 5,
        Hours = 6,
        Days = 7,
        Weeks = 8,
        Months = 9,
        Years = 10
    }

    public static class DateTimeUnitExtension
    {
        public static double To(this DateTimeUnit unit, DateTimeUnit targetUnit)
            => unit.ToTicks() / targetUnit.ToTicks();

        public static double ToTicks(this DateTimeUnit unit)
            => unit switch
            {
                DateTimeUnit.Ticks => 1D,
                DateTimeUnit.Microseconds => 1e+1,
                DateTimeUnit.Milliseconds => 1e+4,
                DateTimeUnit.Seconds => 1e+7,
                DateTimeUnit.Minutes => 6e+8,
                DateTimeUnit.Hours => 3.6e+10,
                DateTimeUnit.Days => 8.64e+11,
                DateTimeUnit.Weeks => 6.048e+12,
                DateTimeUnit.Months => 2.628e+13,
                DateTimeUnit.Years => 3.154e+14,
                _ => throw new ArgumentOutOfRangeException(nameof(unit))
            };

        public static TimeSpan ToTimeSpan(this DateTimeUnit unit)
            => TimeSpan.FromMicroseconds(unit.To(DateTimeUnit.Microseconds));
    }
}
