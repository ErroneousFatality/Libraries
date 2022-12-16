using System.Runtime.InteropServices;

namespace AndrejKrizan.Common.ValueObjects.Pointables
{
    public class Pointable : IDisposable
    {
        // Properties
        private GCHandle Handle { get; set; }

        // Computed properties
        public IntPtr Pointer
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
}
