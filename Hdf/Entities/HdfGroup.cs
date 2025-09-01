using AndrejKrizan.Hdf.Entities.AttributableObjects;

using HDF.PInvoke;

namespace AndrejKrizan.Hdf.Entities;

public class HdfGroup : HdfContainer
{
    // Constructors
    public HdfGroup(HdfContainer parent, string name, params HdfAttributeDto[] attributes)
        : base(parent, name, attributes)
    {
    }

    // Methods
    public override string Describe()
        => "group";

    // Protected methods
    protected override long CreateInternal()
        => H5G.create(Parent!.Id, Name!);

    protected override long OpenInternal()
        => H5G.open(Parent!.Id, Name!);

    protected override int CloseInternal()
        => H5G.close(Id);
}
