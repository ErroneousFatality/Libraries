using System.Collections.Immutable;

using AndrejKrizan.Common.Extensions;

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
            IntPtr[] pointers = Pointables.Select(pointer => pointer.Pointer).ToArray(Pointables.Length);
            SetHandle(pointers);
        }

        public PointableArray(IReadOnlyCollection<object> objects)
            : this(pointables: objects.Convert(data => new Pointable(data)))
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
