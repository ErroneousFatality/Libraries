namespace AndrejKrizan.DotNet.Extensions;
public static class DecimalExtensions
{
    public static double ToDouble(this decimal value)
        => decimal.ToDouble(value);

    public static double? ToDouble(this decimal? value)
        => value?.ToDouble();
}
