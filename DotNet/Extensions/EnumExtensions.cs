namespace AndrejKrizan.DotNet.Extensions;

public static class EnumExtensions
{
    public static IEnumerable<TEnum> ToFlags<TEnum>(this TEnum flags)
        where TEnum : struct, Enum
        => Enum.GetValues<TEnum>().Where(e => flags.HasFlag(e));
}
