using System.Runtime.InteropServices;

namespace AndrejKrizan.DotNet.Allocations;

public class Allocation : IDisposable
{
    // Properties
    private GCHandle Handle { get; set; }

    // Computed properties
    public nint Pointer
        => Handle.AddrOfPinnedObject();

    // Constructors
    public Allocation(object data)
        => Allocate(data);

    protected Allocation() { }

    // Finalizers
    ~Allocation()
        => Dispose();

    // Methods
    public virtual void Dispose()
    {
        Handle.Free();
        GC.SuppressFinalize(this);
    }

    protected void Allocate(object data)
        => Handle = GCHandle.Alloc(data, GCHandleType.Pinned);
}
