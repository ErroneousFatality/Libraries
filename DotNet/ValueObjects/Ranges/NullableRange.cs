using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace AndrejKrizan.DotNet.ValueObjects.Ranges;

public sealed class NullableRange<T> : IComparable<NullableRange<T>>
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
    [JsonConstructor]
    public NullableRange(T? from, T? to)
        => Initialize(from, to, validate: true);

    /// <param name="from">Inclusive.</param>
    /// <param name="to">Inclusive.</param>
    /// <param name="validate">Should throw an <see cref="ArgumentException"/> if from is greater than to?</param>
    public NullableRange(T? from, T? to, bool validate = true)
        => Initialize(from, to, validate);

    /// <param name="list">A list with two elements</param>
    /// <param name="validate">Should throw an <see cref="ArgumentException"/> if from is greater than to?</param>
    public NullableRange(IReadOnlyList<T?> list, bool validate = true)
    {
        if (list.Count != 2)
        {
            throw new ArgumentException($"The range can only be created from a {nameof(IReadOnlyList<T>)} which has exactly two elements.", nameof(list));
        }
        Initialize(list[0], list[1], validate);
    }

    private NullableRange() { }

    // Methods
    public void Deconstruct(out T? from, out T? to)
    {
        from = From;
        to = To;
    }

    public override string ToString()
        => $"[{From?.ToString() ?? "null"}, {To?.ToString() ?? "null"}]";

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

    // Private methods
    /// <param name="from">Inclusive.</param>
    /// <param name="to">Inclusive.</param>
    /// <param name="validate">Should throw an <see cref="ArgumentException"/> if from is greater than to?</param>
    private void Initialize(T? from, T? to, bool validate)
    {
        if (validate && Comparer<T?>.Default.Compare(from, to) > 0)
        {
            throw new ArgumentException($"{nameof(from)} cannot be greater than {nameof(to)}.");
        }

        From = from;
        To = to;
    }

    // Static converters
    public static implicit operator NullableRange<T>(T?[] array) => new NullableRange<T>(array);
    public static implicit operator T?[](NullableRange<T> range) => new T?[] { range.From, range.To };

    public static implicit operator NullableRange<T>(ImmutableArray<T?> array) => new NullableRange<T>(array);
    public static implicit operator ImmutableArray<T?>(NullableRange<T> range) => ImmutableArray.Create(range.From, range.To);

    public static implicit operator NullableRange<T>(List<T?> list) => new NullableRange<T>(list);
    public static implicit operator List<T?>(NullableRange<T> range) => new List<T?>(2) { range.From, range.To };
}
