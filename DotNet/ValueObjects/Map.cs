using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace AndrejKrizan.DotNet.ValueObjects;

/// <summary>A bidirectional dictionary.</summary>
public class Map<TLeft, TRight> : IEnumerable<KeyValuePair<TLeft, TRight>>
    where TLeft : notnull
    where TRight : notnull
{
    // Fields
    private readonly Dictionary<TLeft, TRight> Forward;
    private readonly Dictionary<TRight, TLeft> Backward;

    // Constructors
    public Map(IEqualityComparer<TLeft>? leftComparer = null, IEqualityComparer<TRight>? rightComparer = null)
    {
        Forward = new(leftComparer);
        Backward = new(rightComparer);
    }

    public Map(uint capacity, 
        IEqualityComparer<TLeft>? leftComparer = null, IEqualityComparer<TRight>? rightComparer = null
    )
    {
        if (capacity > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), $"Capacity cannot exceed {int.MaxValue}.");
        }
        int _capacity = (int)capacity;
        Forward = new(_capacity, leftComparer);
        Backward = new(_capacity, rightComparer);
    }

    public Map(IEnumerable<KeyValuePair<TLeft, TRight>> pairs,
        IEqualityComparer<TLeft>? leftComparer = null, IEqualityComparer<TRight>? rightComparer = null
    )
    {
        Forward = new(pairs, leftComparer);
        Backward = new(pairs.Select(pair => new KeyValuePair<TRight, TLeft>(pair.Value, pair.Key)), rightComparer);
    }

    public Map(IDictionary<TLeft, TRight> dictionary,
        IEqualityComparer<TLeft>? leftComparer = null, IEqualityComparer<TRight>? rightComparer = null
    )
    {
        Forward = new(dictionary, leftComparer);
        Backward = new(dictionary.Select(pair => new KeyValuePair<TRight, TLeft>(pair.Value, pair.Key)), rightComparer);
    }

    // Methods
    public void Add(TLeft left, TRight right)
    {
        Forward.Add(left, right);
        Backward.Add(right, left);
    }
    
    public void Add((TLeft Left, TRight Right) pair)
        => Add(pair.Left, pair.Right);

    public void Add(KeyValuePair<TLeft, TRight> pair)
        => Add(pair.Key, pair.Value);


    public bool ContainsLeft(TLeft left)
        => Forward.ContainsKey(left);
    public bool Contains(TLeft left)
        => ContainsLeft(left);

    public bool ContainsRight(TRight right)
        => Backward.ContainsKey(right);
    public bool Contains(TRight right)
        => ContainsRight(right);

    public bool Contains(TLeft left, TRight right)
        => ContainsLeft(left) && ContainsRight(right);

    public bool Contains((TLeft Left, TRight Right) pair)
        => Contains(pair.Left, pair.Right);


    public TRight GetRight(TLeft left)
        => Forward[left];
    public TRight Get(TLeft left)
        => GetRight(left);

    public TLeft GetLeft(TRight right)
        => Backward[right];
    public TLeft Get(TRight right)
        => GetLeft(right);


    public bool TryGetRightValue(TLeft left, [MaybeNullWhen(false)] out TRight right)
        => Forward.TryGetValue(left, out right);
    public bool TryGetValue(TLeft left, [MaybeNullWhen(false)] out TRight right)
        => TryGetRightValue(left, out right);

    public bool TryGetLeftValue(TRight right, [MaybeNullWhen(false)] out TLeft left)
        => Backward.TryGetValue(right, out left);
    public bool TryGetValue(TRight right, [MaybeNullWhen(false)] out TLeft left)
        => TryGetLeftValue(right, out left);


    public IEnumerator<KeyValuePair<TLeft, TRight>> GetForwardEnumerator()
        => Forward.GetEnumerator();
    public IEnumerator<KeyValuePair<TLeft, TRight>> GetEnumerator()
        => GetForwardEnumerator();
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public IEnumerator<KeyValuePair<TRight, TLeft>> GetBackwardEnumerator()
        => Backward.GetEnumerator();


    public bool RemoveByLeft(TLeft left)
        => Forward.Remove(left);
    public bool Remove(TLeft left)
        => RemoveByLeft(left);

    public bool RemoveByLeft(TLeft left, [MaybeNullWhen(false)] out TRight right)
        => Forward.Remove(left, out right);
    public bool Remove(TLeft left, [MaybeNullWhen(false)] out TRight right)
        => RemoveByLeft(left, out right);

    public bool RemoveByRight(TRight right)
        => Backward.Remove(right);
    public bool Remove(TRight right)
        => RemoveByRight(right);

    public bool RemoveByRight(TRight right, [MaybeNullWhen(false)] out TLeft left)
        => Backward.Remove(right, out left);
    public bool Remove(TRight right, [MaybeNullWhen(false)] out TLeft left)
        => RemoveByRight(right, out left);


    public void Clear()
    {
        Forward.Clear();
        Backward.Clear();
    }

    public TRight this[TLeft left]
    {
        get => GetRight(left);
        set => Add(left, value);
    }

    public TLeft this[TRight right]
    {
        get => GetLeft(right);
        set => Add(value, right);
    }


    public IReadOnlyCollection<TLeft> Left => Forward.Keys;
    public IReadOnlyCollection<TRight> Right => Backward.Keys;
    public int Count => Forward.Count;
}
