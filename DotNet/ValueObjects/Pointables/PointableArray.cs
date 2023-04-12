using System.Collections.Immutable;

namespace AndrejKrizan.DotNet.ValueObjects.Pointables;

public sealed class PointableArray : Pointable
{
    // Properties
    private ImmutableArray<Pointable> Pointables { get; }

    // Constructors
    public PointableArray(IEnumerable<Pointable> pointables)
    {
        Pointables = pointables.ToImmutableArray();
        IntPtr[] pointers = Pointables.Select(pointable => pointable.Pointer).ToArray();
        SetHandle(pointers);
    }

    public PointableArray(params Pointable[] pointables)
        : this(pointables.AsEnumerable()) { }

    // Finalizers
    ~PointableArray()
        => Dispose();

    // Methods
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public override void Dispose()
    {
        base.Dispose();
        foreach (Pointable pointable in Pointables)
        {
            pointable.Dispose();
        }
    }
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
}
