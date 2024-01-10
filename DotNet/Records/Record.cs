using System.Diagnostics.CodeAnalysis;

namespace AndrejKrizan.DotNet.Records;

public abstract class Record<TId>
    where TId : notnull
{
    // Properties
    public required TId Id { get; init; }

    // Constructors
    [SetsRequiredMembers]
    protected Record(TId id)
    {
        Id = id;
    }

    protected Record() { }
}
