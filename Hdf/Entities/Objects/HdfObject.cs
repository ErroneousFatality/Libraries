using AndrejKrizan.Hdf.Extensions;

namespace AndrejKrizan.Hdf.Entities.Objects
{
    public abstract class HdfObject : IHdfObject
    {
        // Properties
        public HdfObject? Parent { get; }
        public string? Name { get; }

        public long Id => _id ?? throw new InvalidOperationException($"The {DescriptionWithPathName} is closed.");
        private long? _id = null;

        public bool IsOpen => _openedScopeCount > 0;
        private ushort _openedScopeCount;

        public bool IsCreated { get; private set; }

        // Computed properties
        public virtual string? PathName => $"{(Parent == null ? string.Empty : $"{Parent.PathName}/")}{Name}";
        public string DescriptionWithPathName => $"{Describe()}{(string.IsNullOrEmpty(PathName) ? string.Empty : $" at \"{PathName}\"")}";

        // Constructors
        protected HdfObject(HdfObject? parent = null, string? name = null)
        {
            Parent = parent;

            if (name != null && string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"A {nameof(HdfObject)}'s name must be either null or nonempty.", nameof(name));
            }
            Name = name;
        }

        // Finalizers
        ~HdfObject()
            => Close();

        // Methods
        public abstract string Describe();

        public virtual IDisposable Create()
        {
            if (IsCreated)
            {
                throw new InvalidOperationException($"The {DescriptionWithPathName} was already created.");
            }
            Parent?.OpenOrCreate();
            SetId(CreateInternal().ValidateHDFId(() => $"create {DescriptionWithPathName}"));
            IsCreated = true;
            return this;
        }

        public IDisposable OpenOrCreate()
            => IsCreated ? Open() : Create();

        public IDisposable Open()
        {
            if (IsOpen)
            {
                _openedScopeCount++;
            }
            else
            {
                Parent?.OpenOrCreate();
                SetId(OpenInternal().ValidateHDFId(() => $"open {DescriptionWithPathName}"));
                GC.ReRegisterForFinalize(this);
            }
            return this;
        }

        public void Dispose()
        {
            if (_openedScopeCount > 1)
            {
                _openedScopeCount--;
            }
            else
            {
                Close();
                GC.SuppressFinalize(this);
            }
        }

        public IDisposable CreateAndDo(Action action, bool dispose = true)
        {
            IDisposable disposable = Create();
            action();
            if (dispose)
            {
                disposable.Dispose();
            }
            return disposable;
        }

        // Protected methods
        protected abstract long CreateInternal();
        protected abstract long OpenInternal();
        protected abstract int CloseInternal();

        // Private methods
        private void SetId(long id)
        {
            if (IsOpen)
            {
                throw new InvalidOperationException($"{DescriptionWithPathName} is already open.");
            }
            _id = id.ValidateHDFId(() => $"set id of {DescriptionWithPathName}");
            _openedScopeCount = 1;
        }

        private void Close()
        {
            if (!IsOpen)
            {
                throw new InvalidOperationException($"The {DescriptionWithPathName} is not open.");
            }
            CloseInternal().ValidateHDFResponse(() => $"close {DescriptionWithPathName}");
            _id = null;
            _openedScopeCount = 0;
            Parent?.Dispose();
        }
    }
}
