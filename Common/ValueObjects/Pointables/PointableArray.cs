using System.Collections.Immutable;

namespace AndrejKrizan.Common.ValueObjects.Pointables
{
    public sealed class PointableArray : Pointable
    {
        // Properties
        private ImmutableArray<Pointable> Pointables { get; }

        // Constructors
        public PointableArray(ImmutableArray<Pointable> pointables)
        {
            Pointables = pointables;
            IntPtr[] pointers = Pointables.Select(pointable => pointable.Pointer).ToArray();
            SetHandle(pointers);
        }

        public PointableArray(IEnumerable<object> objects)
            : this(pointables: objects.Select(data => new Pointable(data)).ToImmutableArray())
        { }

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
}
