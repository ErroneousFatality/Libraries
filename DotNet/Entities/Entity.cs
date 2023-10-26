namespace AndrejKrizan.DotNet.Entities;
public abstract class Entity<TId>
{
    // Properties
    public TId Id { get; private set; }

    // Constructors
    protected Entity(TId id)
    {
        Id = id;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>Used for EntityFrameworkCore deserialization.</summary>
    protected Entity() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
