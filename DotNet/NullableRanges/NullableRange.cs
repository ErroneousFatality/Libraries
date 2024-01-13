using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace AndrejKrizan.DotNet.NullableRanges;

public sealed class NullableRange<T> : IComparable<NullableRange<T>>, IEquatable<NullableRange<T>>
    where T : struct
{
    // Properties

    /// <summary>Inclusive.</summary>
    public T? From { get; private set; }

    /// <summary>Inclusive.</summary>
    public T? To { get; private set; }

    // Constructors

    /// <param name="from">Inclusive.</param>
    /// <param name="to">Inclusive.</param>
    /// <param name="validate">Should throw an <see cref="ArgumentException"/> if <paramref name="from"/> is greater <paramref name="to"/>?</param>
    public NullableRange(T? from = null, T? to = null, bool validate = true)
    {
        Initialize(from, to, validate);
        From = from;
        To = to;
        if (validate)
        {
            Validate();
        }
    }

    /// <param name="from">Inclusive.</param>
    /// <param name="to">Inclusive.</param>
    [JsonConstructor]
    public NullableRange(T? from = null, T? to = null)
    {
        Initialize(from, to, validate: true);
    }

    /// <param name="list">A list with two elements</param>
    /// <param name="validate">Should throw an <see cref="ArgumentException"/> if first element is greater than the second?</param>
    public NullableRange(IReadOnlyList<T?> list, bool validate = true)
    {
        if (list.Count != 2)
        {
            throw new ArgumentException($"The range can only be created from a {nameof(IReadOnlyList<T?>)} which has exactly two elements.", nameof(list));
        }
        Initialize(list[0], list[1], validate);
    }

    private NullableRange() { }

    // Methods
    public void Validate()
    {
        if (Comparer<T?>.Default.Compare(From, To) > 0)
        {
            throw new ArgumentException($"{nameof(From)} cannot be greater than {nameof(To)}.");
        }
    }

    public bool HasBoundaries()
        => From.HasValue || To.HasValue;

    public bool Contains(T value)
        => (!From.HasValue || Comparer<T>.Default.Compare(value, From.Value) >= 0)
        && (!To.HasValue || Comparer<T>.Default.Compare(value, To.Value) <= 0);

    public int CompareTo(NullableRange<T>? other)
    {
        if (other == null)
        {
            return 1;
        }
        Comparer<T?> comparer = Comparer<T?>.Default;
        int fromDiff = comparer.Compare(From, other.From);
        if (fromDiff != 0)
        {
            return fromDiff;
        }
        int toDiff = comparer.Compare(To, other.To);
        return toDiff;
    }

    public bool Equals(NullableRange<T>? other)
        => ReferenceEquals(this, other) || CompareTo(other) == 0;

    public void Deconstruct(out T? from, out T? to)
    {
        from = From;
        to = To;
    }

    public override string ToString()
        => $"[{From?.ToString() ?? "null"}, {To?.ToString() ?? "null"}]";

    public override bool Equals(object? other)
        => ReferenceEquals(this, other) || other is NullableRange<T> range && CompareTo(range) == 0;

    public override int GetHashCode()
        => From.GetHashCode() ^ To.GetHashCode();

    // Implicit conversions

    public static implicit operator NullableRange<T>((T? From, T? To) pair) => new NullableRange<T>(pair.From, pair.To);
    public static implicit operator (T? From, T? To)(NullableRange<T> range) => (range.From, range.To);

    public static implicit operator NullableRange<T>(T?[] array) => new NullableRange<T>(array);
    public static implicit operator T?[](NullableRange<T> range) => new T?[] { range.From, range.To };

    public static implicit operator NullableRange<T>(ImmutableArray<T?> array) => new NullableRange<T>(array);
    public static implicit operator ImmutableArray<T?>(NullableRange<T> range) => ImmutableArray.Create(range.From, range.To);

    public static implicit operator NullableRange<T>(List<T?> list) => new NullableRange<T>(list);
    public static implicit operator List<T?>(NullableRange<T> range) => new List<T?>(2) { range.From, range.To };

    // Comparisons

    public static bool operator ==(NullableRange<T>? x, NullableRange<T>? y)
        => ReferenceEquals(x, y) || x is not null && x.CompareTo(y) == 0;

    public static bool operator !=(NullableRange<T>? x, NullableRange<T>? y)
        => !(x == y);

    public static bool operator <(NullableRange<T>? left, NullableRange<T>? right)
        => left is null
            ? right is not null
            : left.CompareTo(right) < 0;

    public static bool operator <=(NullableRange<T>? left, NullableRange<T>? right)
        => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(NullableRange<T>? left, NullableRange<T>? right)
        => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(NullableRange<T>? left, NullableRange<T>? right)
        => left is null
            ? right is null
            : left.CompareTo(right) >= 0;

    // Private methods
    private void Initialize(T? from, T? to, bool validate)
    {
        From = from;
        To = to;
        if (validate)
        {
            Validate();
        }
    }
}
