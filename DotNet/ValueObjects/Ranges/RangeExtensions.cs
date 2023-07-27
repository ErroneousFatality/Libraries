namespace AndrejKrizan.DotNet.ValueObjects.Ranges;
public static class RangeExtensions
{
    public static sbyte GetDelta(this Range<sbyte> range)
        => (sbyte)(range.To - range.From);

    public static byte GetDelta(this Range<byte> range)
        => (byte)(range.To - range.From);

    public static short GetDelta(this Range<short> range)
        => (short)(range.To - range.From);

    public static ushort GetDelta(this Range<ushort> range)
        => (ushort)(range.To - range.From);

    public static int GetDelta(this Range<int> range)
        => range.To - range.From;

    public static uint GetDelta(this Range<uint> range)
        => range.To - range.From;

    public static long GetDelta(this Range<long> range)
        => range.To - range.From;

    public static ulong GetDelta(this Range<ulong> range)
        => range.To - range.From;

    public static nint GetDelta(this Range<nint> range)
        => range.To - range.From;

    public static nuint GetDelta(this Range<nuint> range)
        => range.To - range.From;

    public static float GetDelta(this Range<float> range)
        => range.To - range.From;

    public static double GetDelta(this Range<double> range)
        => range.To - range.From;

    public static decimal GetDelta(this Range<decimal> range)
        => range.To - range.From;

    public static TimeSpan GetDelta(this Range<DateTime> range)
        => range.To - range.From;

    public static ushort GetDelta(this Range<char> range)
        => (ushort)(range.To - range.From);
}
