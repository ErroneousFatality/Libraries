using AndrejKrizan.Hdf.Entities.AttributableObjects;
using AndrejKrizan.Hdf.Entities.AttributableObjects.Dtos;
using AndrejKrizan.Hdf.Entities.Objects;
using AndrejKrizan.Hdf.Entities.Types;

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

    #region DateTime Datasets

    public HdfDataset<DateTime> CreateDateTimeDataset(string pathName, DateTime value, bool dispose = true, params HdfAttributeDto[] attributes)
        => CreateDataset(
            pathName,
            CreateDateTimeConstructor(attributes),
            value,
            dispose
        );
    public HdfDataset<DateTime> CreateDateTimeDataset(string pathName, DateTime value, params HdfAttributeDto[] attributes)
        => CreateDateTimeDataset(pathName, value, dispose: true, attributes);

    public HdfDataset<DateTime> CreateDateTimeDataset(string pathName, IEnumerable<DateTime> collection, bool dispose = true, params HdfAttributeDto[] attributes)
        => CreateDataset(
            pathName,
            CreateDateTimeConstructor(attributes),
            collection,
            dispose
        );
    public HdfDataset<DateTime> CreateDateTimeDataset(string pathName, IEnumerable<DateTime> collection, params HdfAttributeDto[] attributes)
        => CreateDateTimeDataset(pathName, collection, dispose: true, attributes);

    public HdfDataset<DateTime> CreateDateTimeDataset<TRow>(string pathName, IEnumerable<TRow> matrix, bool dispose = true, params HdfAttributeDto[] attributes)
        where TRow : IEnumerable<DateTime>
        => CreateDataset(
            pathName,
            CreateDateTimeConstructor(attributes),
            matrix,
            dispose
        );
    public HdfDataset<DateTime> CreateDateTimeDataset<TRow>(string pathName, IEnumerable<TRow> matrix, params HdfAttributeDto[] attributes)
        where TRow : IEnumerable<DateTime>
        => CreateDateTimeDataset(pathName, matrix, dispose: true, attributes);

    private static Func<ulong[], Func<HdfContainer, string, HdfDataset<DateTime>>> CreateDateTimeConstructor(HdfAttributeDto[] attributes)
        => (dimensions)
            => (container, name)
                => new HdfDataset<DateTime>(container, name, new HdfDateTimeType(), dimensions, attributes);

    #endregion

    #region String Datasets

    public HdfDataset<string> CreateStringDataset(string pathName, string value, bool dispose = true, params HdfAttributeDto[] attributes)
        => CreateDataset(
            pathName,
            CreateStringConstructor(attributes),
            value,
            dispose
        );
    public HdfDataset<string> CreateStringDataset(string pathName, string value, params HdfAttributeDto[] attributes)
        => CreateStringDataset(pathName, value, dispose: true, attributes);

    public HdfDataset<string> CreateStringDataset(string pathName, IEnumerable<string> collection, bool dispose = true, params HdfAttributeDto[] attributes)
        => CreateDataset(
            pathName,
            CreateStringConstructor(attributes),
            collection,
            dispose
        );
    public HdfDataset<string> CreateStringDataset(string pathName, IEnumerable<string> collection, params HdfAttributeDto[] attributes)
        => CreateStringDataset(pathName, collection, dispose: true, attributes);

    public HdfDataset<string> CreateStringDataset<TRow>(string pathName, IEnumerable<TRow> matrix, bool dispose = true, params HdfAttributeDto[] attributes)
        where TRow : IEnumerable<string>
        => CreateDataset(
            pathName,
            CreateStringConstructor(attributes),
            matrix,
            dispose
        );
    public HdfDataset<string> CreateStringDataset<TRow>(string pathName, IEnumerable<TRow> matrix, params HdfAttributeDto[] attributes)
        where TRow : IEnumerable<string>
        => CreateStringDataset(pathName, matrix, dispose: true, attributes);

    private static Func<ulong[], Func<HdfContainer, string, HdfDataset<string>>> CreateStringConstructor(HdfAttributeDto[] attributes)
        => (dimensions)
            => (container, name)
                => new HdfDataset<string>(container, name, new HdfStringType(), dimensions, attributes);

    #endregion

    #region Generic Datasets

    public HdfDataset<T> CreateDataset<T>(string pathName, T value, bool dispose = true, params HdfAttributeDto[] attributes)
        where T : notnull
        => CreateDataset(
            pathName,
            CreateGenericConstructor<T>(attributes),
            value,
            dispose
        );
    public HdfDataset<T> CreateDataset<T>(string pathName, T value, params HdfAttributeDto[] attributes)
        where T : notnull
        => CreateDataset(pathName, value, dispose: true, attributes);


    public HdfDataset<T> CreateDataset<T>(string pathName, IEnumerable<T> collection, bool dispose = true, params HdfAttributeDto[] attributes)
        where T : notnull
        => CreateDataset(
            pathName,
            CreateGenericConstructor<T>(attributes),
            collection,
            dispose
        );
    public HdfDataset<T> CreateDataset<T>(string pathName, IEnumerable<T> collection, params HdfAttributeDto[] attributes)
        where T : notnull
        => CreateDataset(pathName, collection, dispose: true, attributes);


    public HdfDataset<T> CreateDataset<T, TRow>(string pathName, IEnumerable<TRow> matrix, bool dispose = true, params HdfAttributeDto[] attributes)
        where T : notnull
        where TRow : IEnumerable<T>
        => CreateDataset(
            pathName,
            CreateGenericConstructor<T>(attributes),
            matrix,
            dispose
        );
    public HdfDataset<T> CreateDataset<T, TRow>(string pathName, IEnumerable<TRow> matrix, params HdfAttributeDto[] attributes)
        where T : notnull
        where TRow : IEnumerable<T>
        => CreateDataset<T, TRow>(pathName, matrix, dispose: true, attributes);

    private static Func<ulong[], Func<HdfContainer, string, HdfDataset<T>>> CreateGenericConstructor<T>(HdfAttributeDto[] attributes)
        where T : notnull
        => (dimensions)
            => (container, name)
                => new HdfDataset<T>(container, name, dimensions, attributes);

    #endregion

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
    private HdfDataset<T> CreateDataset<T>(
        string pathName,
        Func<ulong[], Func<HdfContainer, string, HdfDataset<T>>> constructorConstructor,
        T value,
        bool dispose = true
    )
        where T : notnull
    {
        ulong[] dimensions = Array.Empty<ulong>();
        HdfDataset<T> dataset = CreateChild(pathName, constructorConstructor(dimensions));
        dataset.Write(value);
        if (dispose)
        {
            dataset.Dispose();
        }
        return dataset;
    }

    private HdfDataset<T> CreateDataset<T>(
        string pathName,
        Func<ulong[], Func<HdfContainer, string, HdfDataset<T>>> constructorConstructor,
        IEnumerable<T> collection,
        bool dispose = true
    )
        where T : notnull
    {
        ulong[] dimensions = [(ulong)collection.Count()];
        HdfDataset<T> dataset = CreateChild(pathName, constructorConstructor(dimensions));
        dataset.Write(collection);
        if (dispose)
        {
            dataset.Dispose();
        }
        return dataset;
    }

    private HdfDataset<T> CreateDataset<T, TRow>(
        string pathName,
        Func<ulong[], Func<HdfContainer, string, HdfDataset<T>>> constructorConstructor,
        IEnumerable<TRow> matrix,
        bool dispose = true
    )
        where T : notnull
        where TRow : IEnumerable<T>
    {
        ulong[] dimensions = [(ulong)matrix.Count(), (ulong)(matrix.FirstOrDefault()?.Count() ?? 0)];
        HdfDataset<T> dataset = CreateChild(pathName, constructorConstructor(dimensions));
        dataset.Write(matrix);
        if (dispose)
        {
            dataset.Dispose();
        }
        return dataset;
    }

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
