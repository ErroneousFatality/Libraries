using System.Collections.ObjectModel;

using AndrejKrizan.Hdf.Entities.AttributableObjects.Dtos;
using AndrejKrizan.Hdf.Entities.Objects;

namespace AndrejKrizan.Hdf.Entities.AttributableObjects
{
    public abstract class HdfAttributableObject : HdfObject
    {
        // Properties
        public IReadOnlyCollection<HdfAttribute>? Attributes => _attributes?.AsReadOnly();
        private List<HdfAttribute>? _attributes;
        public IReadOnlyCollection<HdfAttributeDto>? AttributeDtos { get; private set; }

        // Constructors
        public HdfAttributableObject(HdfObject? parent = null, string? name = null, params HdfAttributeDto[] attributes)
            : base(parent, name)
        {
            _attributes = new(attributes.Length);
            AttributeDtos = new ReadOnlyCollection<HdfAttributeDto>(attributes);
        }

        // Methods
        public override IDisposable Create()
        {
            IDisposable disposable = base.Create();
            _attributes = AttributeDtos!
                .Select(attributeDTO => attributeDTO.CreateAndWriteToAttribute(this))
                .ToList();
            AttributeDtos = null;
            return disposable;
        }

        public void CreateAttribute<T>(HDFAttributeDTO<T> dto)
            where T : notnull
        {
            HdfAttribute attribute = dto.CreateAndWriteToAttribute(this);
            _attributes!.Add(attribute);
        }
        public void CreateAttribute<T>(string name, T value)
            where T : notnull
            => CreateAttribute(new HDFAttributeDTO<T>(name, value));
    }
}
