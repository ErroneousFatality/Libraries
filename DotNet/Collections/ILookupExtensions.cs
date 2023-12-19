using System.Collections.Immutable;

namespace AndrejKrizan.DotNet.Collections;
public static class ILookupExtensions
{
    public static ImmutableDictionary<TKey, ImmutableArray<TValue>> ToImmutableDictionary<TKey, TValue>(this ILookup<TKey, TValue> lookup)
        where TKey : notnull
        => lookup.ToImmutableDictionary(group => group.Key, group => group.ToImmutableArray());

    public static Dictionary<TKey, List<TValue>> ToDictionaryOfLists<TKey, TValue>(this ILookup<TKey, TValue> lookup)
        where TKey : notnull
        => lookup.ToDictionary(group => group.Key, group => group.ToList());

    public static Dictionary<TKey, TValue[]> ToDictionaryOfArrays<TKey, TValue>(this ILookup<TKey, TValue> lookup)
        where TKey : notnull
        => lookup.ToDictionary(group => group.Key, group => group.ToArray());
}
