namespace AndrejKrizan.DotNet.Entities;

/// <summary>
///     An entity with a <see cref="TId"/> Id property.
/// </summary>
public abstract class Entity<TId> : IEntity<TId>
    where TId : struct
{
    // Properties
    public TId Id { get; private set; }

    // Constructors
    protected Entity(TId id)
    {
        Id = id;
    }

    /// <summary>Constructor for EntityFrameworkCore.</summary>
    /// <remarks>EntityFrameworkCore chooses the constructor with the least amount of parameters (including 0) where all of them correspond to a property.</remarks>
    protected Entity() { }
}