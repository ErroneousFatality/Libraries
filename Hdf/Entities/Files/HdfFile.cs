using AndrejKrizan.Hdf.Entities.AttributableObjects.Dtos;

using HDF.PInvoke;

namespace AndrejKrizan.Hdf.Entities.Files;

public class HdfFile : HdfContainer
{
    // Properties
    public string FilePath { get; }
    public FileAccessType Access { get; }

    // Computed properties
    public override string? PathName => null;

    // Constructors
    public HdfFile(string filePath, FileAccessType access, params HdfAttributeDto[] attributes)
        : base(parent: null, name: Path.GetFileName(filePath)!, attributes)
    {
        FilePath = filePath;

        if (!Enum.IsDefined(access))
        {
            throw new ArgumentOutOfRangeException(nameof(access));
        }
        Access = access;
    }

    // Methods
    public override string Describe()
        => $"file at \"{FilePath}\" with {Access} access";

    // Protected methods
    protected override long CreateInternal()
        => Access switch
        {
            FileAccessType.Read => H5F.open(FilePath, flags: H5F.ACC_RDONLY),
            FileAccessType.Write => H5F.open(FilePath, flags: H5F.ACC_RDWR),
            FileAccessType.Create => H5F.create(FilePath, flags: H5F.ACC_TRUNC),
            _ => throw new ArgumentOutOfRangeException(nameof(Access))
        };

    protected override long OpenInternal()
        => H5F.open(FilePath, Access switch
        {
            FileAccessType.Read => H5F.ACC_RDONLY,
            FileAccessType.Write => H5F.ACC_RDWR,
            FileAccessType.Create => H5F.ACC_RDWR,
            _ => throw new ArgumentOutOfRangeException(nameof(Access))
        });

    protected override int CloseInternal()
        => H5F.close(Id);
}
