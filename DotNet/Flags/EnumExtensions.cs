namespace AndrejKrizan.DotNet.Flags;

public static class EnumExtensions
{
    public static IEnumerable<TEnum> ToFlags<TEnum>(this TEnum flags)
        where TEnum : struct, Enum
        => Enum.GetValues<TEnum>().Where(flag => flags.HasFlag(flag));

    #region IsValidFlags

    /// <remarks>
    /// Expects the flags enum to be properly defined:
    /// <list type="bullet">
    /// <item><description> The class must have a <see cref="System.FlagsAttribute"/> annotation.   </description></item>
    /// <item><description> Each value must be in form of 2^n, without duplicates.  </description></item>
    /// </list>
    /// </remarks>
    public static bool IsValidFlags<TEnum>(this int flags)
        where TEnum : struct, Enum
    {
        int mask = Enum.GetValues<TEnum>().Cast<int>().Aggregate((_mask, flag) => _mask | flag);
        int invalidFlags = ~mask & flags;
        return invalidFlags == 0;
    }

    /// <remarks>
    /// Expects the flags enum to be properly defined:
    /// <list type="bullet">
    /// <item><description> The class must have a <see cref="System.FlagsAttribute"/> annotation.   </description></item>
    /// <item><description> Each value must be in form of 2^n, without duplicates.  </description></item>
    /// </list>
    /// </remarks>
    public static bool IsValidFlags<TEnum>(this uint flags)
        where TEnum : struct, Enum
    {
        uint mask = Enum.GetValues<TEnum>().Cast<uint>().Aggregate((_mask, flag) => _mask | flag);
        uint invalidFlags = ~mask & flags;
        return invalidFlags == 0;
    }

    /// <remarks>
    /// Expects the flags enum to be properly defined:
    /// <list type="bullet">
    /// <item><description> The class must have a <see cref="System.FlagsAttribute"/> annotation.   </description></item>
    /// <item><description> Each value must be in form of 2^n, without duplicates.  </description></item>
    /// </list>
    /// </remarks>
    public static bool IsValidFlags<TEnum>(this long flags)
        where TEnum : struct, Enum
    {
        long mask = Enum.GetValues<TEnum>().Cast<long>().Aggregate((_mask, flag) => _mask | flag);
        long invalidFlags = ~mask & flags;
        return invalidFlags == 0;
    }

    /// <remarks>
    /// Expects the flags enum to be properly defined:
    /// <list type="bullet">
    /// <item><description> The class must have a <see cref="System.FlagsAttribute"/> annotation.   </description></item>
    /// <item><description> Each value must be in form of 2^n, without duplicates.  </description></item>
    /// </list>
    /// </remarks>
    public static bool IsValidFlags<TEnum>(this ulong flags)
        where TEnum : struct, Enum
    {
        ulong mask = Enum.GetValues<TEnum>().Cast<ulong>().Aggregate((_mask, flag) => _mask | flag);
        ulong invalidFlags = ~mask & flags;
        return invalidFlags == 0;
    }
    #endregion
}
