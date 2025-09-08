using AndrejKrizan.Hdf.Entities.AttributableObjects;

using HDF.PInvoke;

namespace AndrejKrizan.Hdf.Entities.Files;

public class HdfFile : HdfContainer
{
    // Properties
    public string FilePath { get; }
    public FileAccessType Access { get; }

    // Computed properties
    public override string? PathName => string.Empty;

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

    // Static factory methods
    public static HdfFile Create(string filePath, bool dispose = false, params HdfAttributeDto[] attributes)
    {
        HdfFile hdfFile = new(filePath, FileAccessType.Create, attributes);
        IDisposable disposable = hdfFile.Create();
        if (dispose)
        {
            disposable.Dispose();
        }
        return hdfFile;
    }
    public static HdfFile Create(string filePath, params HdfAttributeDto[] attributes)
        => Create(filePath, dispose: false, attributes);

    public static HdfFile OpenWrite(string filePath, bool dispose = false, params HdfAttributeDto[] attributes)
    {
        HdfFile hdfFile = new(filePath, FileAccessType.Write, attributes);
        IDisposable disposable = hdfFile.Open();
        if (dispose)
        {
            disposable.Dispose();
        }
        return hdfFile;
    }
    public static HdfFile OpenWrite(string filePath, params HdfAttributeDto[] attributes)
        => OpenWrite(filePath, dispose: false, attributes);

    public static HdfFile OpenRead(string filePath, bool dispose = false)
    {
        HdfFile hdfFile = new(filePath, FileAccessType.Read);
        IDisposable disposable = hdfFile.Open();
        if (dispose)
        {
            disposable.Dispose();
        }
        return hdfFile;
    }
    public static HdfFile OpenRead(string filePath)
        => OpenRead(filePath, dispose: false);
}
