using System.Diagnostics.CodeAnalysis;

namespace AndrejKrizan.DotNet.Entities;
public abstract class Entity<TId>
{
    // Properties
    public required TId Id { get; init; }

    // Constructors

    [SetsRequiredMembers]
    protected Entity(TId id)
    {
        Id = id;
    }

    /// <summary>Used for EntityFrameworkCore deserialization.</summary>
    protected Entity() { }
}
