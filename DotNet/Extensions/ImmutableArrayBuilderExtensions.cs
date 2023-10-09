using System.Collections.Immutable;

namespace AndrejKrizan.DotNet.Extensions;
public static class ImmutableArrayBuilderExtensions
{
    /// <summary>
    /// If possible, uses <see cref="ImmutableArray{T}.Builder.MoveToImmutable"/>, 
    /// otherwise uses <see cref="ImmutableArray{T}.Builder.ToImmutable"/>.
    /// </summary>
    public static ImmutableArray<T> GetImmutableArray<T>(this ImmutableArray<T>.Builder builder)
        => builder.Count == builder.Capacity
            ? builder.MoveToImmutable()
            : builder.ToImmutable();
}
