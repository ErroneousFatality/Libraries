namespace AndrejKrizan.DotNet.ValueObjects
{
    public enum DateTimeUnit : byte
    {
        Milliseconds = 1,
        Seconds = 2,
        Minutes = 3,
        Hours = 4,
        Days = 5,
        Weeks = 6,
        Months = 7,
        Years = 8
    }

    public static class DateTimeUnitExtension
    {
        public static double To(this DateTimeUnit unit, DateTimeUnit targetUnit)
            => unit.ToMilliseconds() / targetUnit.ToMilliseconds();

        public static double ToMilliseconds(this DateTimeUnit unit)
            => unit switch
            {
                DateTimeUnit.Milliseconds => 1D,
                DateTimeUnit.Seconds => 1000D,
                DateTimeUnit.Minutes => 60000D,
                DateTimeUnit.Hours => 3.6e+6,
                DateTimeUnit.Days => 8.64e+7,
                DateTimeUnit.Weeks => 6.048e+8,
                DateTimeUnit.Months => 2.628e+9,
                DateTimeUnit.Years => 3.154e+10,
                _ => throw new ArgumentOutOfRangeException(nameof(unit))
            };

        public static TimeSpan ToTimeSpan(this DateTimeUnit unit)
            => TimeSpan.FromMilliseconds(unit.ToMilliseconds());
    }
}
