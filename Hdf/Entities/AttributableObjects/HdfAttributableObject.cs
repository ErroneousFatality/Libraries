using System.Data;

using AndrejKrizan.Hdf.Entities.Objects;

namespace AndrejKrizan.Hdf.Entities.AttributableObjects;

public abstract class HdfAttributableObject : HdfObject
{
    // Properties
    public IReadOnlyCollection<HdfAttribute>? Attributes => _attributes?.AsReadOnly();
    private List<HdfAttribute>? _attributes;

    public IReadOnlyCollection<HdfAttributeDto>? AttributesToCreate => _attributesToCreate?.AsReadOnly();
    private List<HdfAttributeDto>? _attributesToCreate;

    // Constructors
    public HdfAttributableObject(HdfObject? parent = null, string? name = null, params HdfAttributeDto[] attributes)
        : base(parent, name)
    {
        _attributes = null;
        _attributesToCreate = [.. attributes];
    }

    // Methods
    public void QueueAttributeForCreation<T>(string name, T value)
        where T: notnull
    {
        if (IsCreated)
        {
            throw new InvalidOperationException($"{DescriptionWithPathName}: Queueing attributes for creation is allowed only before the attributable object is created.");
        }
        HdfAttributeDto<T> dto = new(name, value);
        _attributesToCreate!.Add(dto);
    }

    public override IDisposable Create()
    {
        IDisposable disposable = base.Create();
        _attributes = _attributesToCreate!
            .Select(dto => dto.CreateAndWriteToAttribute(parent: this, dispose: true))
            .ToList();
        _attributesToCreate = null;
        return disposable;
    }

    public HdfAttribute<T> CreateAttribute<T>(string name, T value, bool dispose = true)
        where T : notnull
    {
        if (!IsOpen)
        {
            throw new InvalidOperationException($"{DescriptionWithPathName}: The attributable object needs to be open in order to create an attribute inside it.");
        }
        HdfAttribute<T> attribute = HdfAttribute<T>.CreateAndWriteTo(parent: this, name, value, dispose);
        _attributes!.Add(attribute);
        return attribute;
    }
}
