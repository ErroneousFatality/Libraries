using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace AndrejKrizan.DotNet.Ranges;

public sealed class Range<T> : IComparable<Range<T>>, IEquatable<Range<T>>
    where T : struct
{
    // Properties

    /// <summary>Inclusive.</summary>
    public T From { get; init; }

    /// <summary>Inclusive.</summary>
    public T To { get; init; }

    // Constructors

    /// <param name="from">Inclusive.</param>
    /// <param name="to">Inclusive.</param>
    /// <param name="validate">Should throw an <see cref="ArgumentException"/> if from is greater than to?</param>
    public Range(T from, T to, bool validate = true)
    {
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
    public Range(T from, T to)
        : this(from, to, validate: true) { }

    /// <param name="list">A list with two elements</param>
    /// <param name="validate">Should throw an <see cref="ArgumentException"/> if from is greater than to?</param>
    public Range(IReadOnlyList<T> list, bool validate = true)
        : this(list.ElementAtOrDefault(0), list.ElementAtOrDefault(1), validate)
    {
        if (list.Count != 2)
        {
            throw new ArgumentException($"The range can only be created from a {nameof(IReadOnlyList<T>)} which has exactly two elements.", nameof(list));
        }
    }

    public Range() { }

    // Methods
    public void Validate()
    {
        if (Comparer<T>.Default.Compare(From, To) > 0)
        {
            throw new ArgumentException($"{nameof(From)} cannot be greater than {nameof(To)}.");
        }
    }

    public bool Contains(T value)
        => Comparer<T>.Default.Compare(value, From) >= 0
        && Comparer<T>.Default.Compare(value, To) <= 0;

    public int CompareTo(Range<T>? other)
    {
        if (other == null)
        {
            return 1;
        }
        Comparer<T> comparer = Comparer<T>.Default;
        int fromDiff = comparer.Compare(From, other.From);
        if (fromDiff != 0)
        {
            return fromDiff;
        }
        int toDiff = comparer.Compare(To, other.To);
        return toDiff;
    }

    public bool Equals(Range<T>? other)
        => ReferenceEquals(this, other) || CompareTo(other) == 0;

    public void Deconstruct(out T from, out T to)
    {
        from = From;
        to = To;
    }

    public override string ToString()
        => $"[{From}, {To}]";

    public override bool Equals(object? other)
        => ReferenceEquals(this, other) || other is Range<T> range && CompareTo(range) == 0;

    public override int GetHashCode()
        => From.GetHashCode() ^ To.GetHashCode();

    // Implicit conversions

    public static implicit operator Range<T>((T From, T To) pair) => new Range<T>(pair.From, pair.To);
    public static implicit operator (T From, T To)(Range<T> range) => (range.From, range.To);

    public static implicit operator Range<T>(T[] array) => new Range<T>(array);
    public static implicit operator T[](Range<T> range) => new T[] { range.From, range.To };

    public static implicit operator Range<T>(ImmutableArray<T> array) => new Range<T>(array);
    public static implicit operator ImmutableArray<T>(Range<T> range) => ImmutableArray.Create(range.From, range.To);

    public static implicit operator Range<T>(List<T> list) => new Range<T>(list);
    public static implicit operator List<T>(Range<T> range) => new List<T>(2) { range.From, range.To };

    // Comparisons

    public static bool operator ==(Range<T>? x, Range<T>? y)
        => ReferenceEquals(x, y) || x is not null && x.CompareTo(y) == 0;

    public static bool operator !=(Range<T>? x, Range<T>? y)
        => !(x == y);

    public static bool operator <(Range<T>? left, Range<T>? right)
        => left is null
            ? right is not null
            : left.CompareTo(right) < 0;

    public static bool operator <=(Range<T>? left, Range<T>? right)
        => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Range<T>? left, Range<T>? right)
        => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Range<T>? left, Range<T>? right)
        => left is null
            ? right is null
            : left.CompareTo(right) >= 0;
}
