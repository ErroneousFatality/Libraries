using System.Collections.Immutable;

namespace AndrejKrizan.DotNet.Allocations;

public sealed class AllocationArray : Allocation
{
    // Properties
    private ImmutableArray<Allocation> Allocations { get; }

    // Constructors
    public AllocationArray(IEnumerable<Allocation> allocations)
    {
        Allocations = allocations.ToImmutableArray();
        nint[] pointers = Allocations.Select(allocation => allocation.Pointer).ToArray();
        Allocate(pointers);
    }

    public AllocationArray(params Allocation[] allocations)
        : this(allocations.AsEnumerable()) { }

    // Finalizers
    ~AllocationArray()
        => Dispose();

    // Methods
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public override void Dispose()
    {
        base.Dispose();
        foreach (Allocation allocation in Allocations)
        {
            allocation.Dispose();
        }
    }
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
}
