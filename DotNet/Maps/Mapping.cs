using System.Diagnostics.CodeAnalysis;

namespace AndrejKrizan.DotNet.Maps;
public sealed class Mapping<TLeft, TRight>
    where TLeft : notnull
    where TRight : notnull
{
    // Properties
    public required TLeft Left { get; init; }
    public required TRight Right { get; init; }

    // Constructors
    public Mapping() { }

    [SetsRequiredMembers]
    public Mapping(TLeft left, TRight right)
    {
        Left = left;
        Right = right;
    }

    // Conversions
    public static implicit operator Mapping<TLeft, TRight>((TLeft Left, TRight Right) pair)
        => new(pair.Left, pair.Right);
}
