namespace AndrejKrizan.Common.ValueObjects
{
    public enum TimeUnit : byte
    {
        Nanoseconds = 1,
        Ticks = 2,
        Microseconds = 3,
        Milliseconds = 4,
        Seconds = 5,
        Minutes = 6,
        Hours = 7,
        Days = 8,
        Weeks = 9
    }

    public static class TimeUnitExtension
    {
        public static double To(this TimeUnit unit, TimeUnit targetUnit)
            => unit.ToNanoseconds() / targetUnit.ToNanoseconds();

        public static double ToNanoseconds(this TimeUnit unit)
            => unit switch
            {
                TimeUnit.Nanoseconds => 1D,
                TimeUnit.Ticks => 100D,
                TimeUnit.Microseconds => 1000D,
                TimeUnit.Milliseconds => 1000000D,
                TimeUnit.Seconds => 1000000000D,
                TimeUnit.Minutes => 60000000000D,
                TimeUnit.Hours => 3600000000000D,
                TimeUnit.Days => 86400000000000D,
                TimeUnit.Weeks => 604800000000000D,
                _ => throw new ArgumentOutOfRangeException(nameof(unit))
            };
    }
}
