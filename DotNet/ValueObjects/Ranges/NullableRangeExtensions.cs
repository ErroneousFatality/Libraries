namespace AndrejKrizan.DotNet.ValueObjects.Ranges;
public static class NullableRangeExtensions
{
    public static sbyte GetDelta(this NullableRange<sbyte> range)
        => (sbyte)((range.To ?? sbyte.MaxValue) - (range.From ?? sbyte.MinValue));

    public static byte GetDelta(this NullableRange<byte> range)
        => (byte)((range.To ?? byte.MaxValue) - (range.From ?? byte.MinValue));

    public static short GetDelta(this NullableRange<short> range)
        => (short)((range.To ?? short.MaxValue) - (range.From ?? short.MinValue));

    public static ushort GetDelta(this NullableRange<ushort> range)
        => (ushort)((range.To ?? ushort.MaxValue) - (range.From ?? ushort.MinValue));

    public static int GetDelta(this NullableRange<int> range)
        => (range.To ?? int.MaxValue) - (range.From ?? int.MinValue);

    public static uint GetDelta(this NullableRange<uint> range)
        => (range.To ?? uint.MaxValue) - (range.From ?? uint.MinValue);

    public static long GetDelta(this NullableRange<long> range)
        => (range.To ?? long.MaxValue) - (range.From ?? long.MinValue);

    public static ulong GetDelta(this NullableRange<ulong> range)
        => (range.To ?? ulong.MaxValue) - (range.From ?? ulong.MinValue);

    public static nint GetDelta(this NullableRange<nint> range)
        => (range.To ?? nint.MaxValue) - (range.From ?? nint.MinValue);

    public static nuint GetDelta(this NullableRange<nuint> range)
        => (range.To ?? nuint.MaxValue) - (range.From ?? nuint.MinValue);

    public static float GetDelta(this NullableRange<float> range)
        => (range.To ?? float.MaxValue) - (range.From ?? float.MinValue);

    public static double GetDelta(this NullableRange<double> range)
        => (range.To ?? double.MaxValue) - (range.From ?? double.MinValue);

    public static decimal GetDelta(this NullableRange<decimal> range)
        => (range.To ?? decimal.MaxValue) - (range.From ?? decimal.MinValue);

    public static TimeSpan GetDelta(this NullableRange<DateTime> range)
        => (range.To ?? DateTime.MaxValue) - (range.From ?? DateTime.MinValue);

    public static ushort GetDelta(this NullableRange<char> range)
        => (ushort)((range.To ?? char.MaxValue) - (range.From ?? char.MinValue));
}
