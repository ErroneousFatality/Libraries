using AndrejKrizan.Hdf.Entities.AttributableObjects;
using AndrejKrizan.Hdf.Entities.Objects;

namespace AndrejKrizan.Hdf.Entities;

public abstract class HdfContainer : HdfAttributableObject
{
    // Properties
    public IReadOnlyDictionary<string, HdfObject> Children => _children.AsReadOnly();
    private readonly Dictionary<string, HdfObject> _children;

    // Constructors
    protected HdfContainer(HdfContainer? parent, string name, params HdfAttributeDto[] attributes)
        : base(parent, name, attributes)
    {
        _children = [];
    }

    // Methods

    #region Datasets

    public HdfDataset<T> CreateDataset<T>(string pathName, T value, bool dispose = true, params HdfAttributeDto[] attributes)
        where T : notnull
    {
        ulong[] dimensions = [];
        HdfDataset<T> dataset = CreateChild(pathName, CreateConstructor<T>(dimensions, attributes));
        dataset.Write(value: value);
        if (dispose)
        {
            dataset.Dispose();
        }
        return dataset;
    }
    public HdfDataset<T> CreateDataset<T>(string pathName, T value, params HdfAttributeDto[] attributes)
        where T : notnull
        => CreateDataset(pathName, value: value, dispose: true, attributes);

    public HdfDataset<T> CreateDataset<T>(string pathName, IEnumerable<T> collection, bool dispose = true, params HdfAttributeDto[] attributes)
        where T : notnull
    {
        ulong[] dimensions = [(ulong)collection.Count()];
        HdfDataset<T> dataset = CreateChild(pathName, CreateConstructor<T>(dimensions, attributes));
        dataset.Write(collection: collection);
        if (dispose)
        {
            dataset.Dispose();
        }
        return dataset;
    }
    public HdfDataset<T> CreateDataset<T>(string pathName, IEnumerable<T> collection, params HdfAttributeDto[] attributes)
    where T : notnull
        => CreateDataset(pathName, collection: collection, dispose: true, attributes);


    public HdfDataset<T> CreateDataset<T>(string pathName, IEnumerable<IEnumerable<T>> matrix, bool dispose = true, params HdfAttributeDto[] attributes)
        where T : notnull
    {
        ulong[] dimensions = [(ulong)matrix.Count(), (ulong)(matrix.FirstOrDefault()?.Count() ?? 0)];
        HdfDataset<T> dataset = CreateChild(pathName, CreateConstructor<T>(dimensions, attributes));
        dataset.Write(matrix: matrix);
        if (dispose)
        {
            dataset.Dispose();
        }
        return dataset;
    }
    public HdfDataset<T> CreateDataset<T>(string pathName, IEnumerable<IEnumerable<T>> matrix, params HdfAttributeDto[] attributes)
        where T : notnull
        => CreateDataset(pathName, matrix: matrix, dispose: true, attributes);

    public HdfDataset<T> CreateDataset<T, TRow>(string pathName, IEnumerable<TRow> matrix, bool dispose = true, params HdfAttributeDto[] attributes)
        where T : notnull
        where TRow : IEnumerable<T>
        => CreateDataset(pathName, matrix: matrix.Cast<IEnumerable<T>>(), dispose, attributes);
    public HdfDataset<T> CreateDataset<T, TRow>(string pathName, IEnumerable<TRow> matrix, params HdfAttributeDto[] attributes)
        where T : notnull
        where TRow : IEnumerable<T>
        => CreateDataset<T, TRow>(pathName, matrix: matrix, dispose: true, attributes);


    private static Func<HdfContainer, string, HdfDataset<T>> CreateConstructor<T>(ulong[] dimensions, HdfAttributeDto[] attributes)
        where T : notnull
        => (container, name)
            => new HdfDataset<T>(container, name, dimensions, attributes);

    #endregion

    public HdfGroup CreateGroup(string pathName, bool dispose = false, params HdfAttributeDto[] attributes)
    {
        HdfGroup group = CreateChild(pathName,
            (container, name) => new HdfGroup(container, name, attributes)
        );
        if (dispose)
        {
            group.Dispose();
        }
        return group;
    }
    public HdfGroup CreateGroup(string pathName, params HdfAttributeDto[] attributes)
        => CreateGroup(pathName, dispose: false, attributes);

    // Private methods
    private HdfGroup GetOrConstructGroup(string name)
    {
        HdfGroup group;
        if (Children.TryGetValue(name, out HdfObject? hdfObject))
        {
            if (hdfObject is not HdfGroup)
            {
                throw new ArgumentException($"A non-group already exists at {hdfObject.PathName}.", nameof(name));
            }
            group = (HdfGroup)hdfObject;
        }
        else
        {
            group = new HdfGroup(this, name);
            _children.Add(name, group);
        }
        return group;
    }

    private T CreateChild<T>(string pathName, Func<HdfContainer, string, T> constructor)
        where T : HdfObject
    {

        string[] names = pathName.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (names.Length == 0)
        {
            throw new ArgumentException("The path name is empty.", nameof(pathName));
        }

        HdfContainer container = this;
        foreach (string containerName in names.SkipLast(1))
        {
            container = container.GetOrConstructGroup(containerName);
        }

        string childName = names.Last();
        T child = constructor(container, childName);
        if (!child.IsCreated)
        {
            child.Create();
        }
        container._children.Add(childName, child);

        return child;
    }
}
