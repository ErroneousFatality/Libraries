using System.Runtime.InteropServices;

namespace AndrejKrizan.DotNet.Pointables;

public class Pointable : IDisposable
{
    // Properties
    private GCHandle Handle { get; set; }

    // Computed properties
    public nint Pointer
        => Handle.AddrOfPinnedObject();

    // Constructors
    public Pointable(object data)
        => SetHandle(data);

    protected Pointable() { }

    // Finalizers
    ~Pointable()
        => Dispose();

    // Methods
    public virtual void Dispose()
    {
        Handle.Free();
        GC.SuppressFinalize(this);
    }

    protected void SetHandle(object data)
        => Handle = GCHandle.Alloc(data, GCHandleType.Pinned);
}
