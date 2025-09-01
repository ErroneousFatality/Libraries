namespace AndrejKrizan.Hdf.Entities.Objects;

public interface IHdfObject : IDisposable
{
    // Properties
    IHdfObject? Parent { get; }
    string? Name { get; }
    long Id { get; }
    bool IsCreated { get; }
    bool IsOpen { get; }
    string? PathName { get; }
    string DescriptionWithPathName { get; }

    // Methods
    string Describe();
    IDisposable Create();
    IDisposable OpenOrCreate();
    IDisposable Open();
}