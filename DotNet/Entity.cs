namespace AndrejKrizan.DotNet;

/// <summary>
///     An entity with a <see cref="TKey"/> Id property.
/// </summary>
public abstract class Entity<TKey>
    where TKey : struct
{
    // Properties
    public TKey Id { get; protected set; }

    // Constructors
    protected Entity(TKey id)
    {
        Id = id;
    }

    /// <summary>Constructor for EntityFrameworkCore.</summary>
    /// <remarks>EntityFrameworkCore chooses the constructor with the least amount of parameters (including 0) where all of them correspond to a property.</remarks>
    protected Entity() { }
}

/// <summary>
///     An entity with a <see cref="Guid"/> Id property.
/// </summary>
public abstract class Entity : Entity<Guid>
{
    // Constructors
    protected Entity(Guid id)
        : base(id) { }

    /// <summary>Constructor for EntityFrameworkCore.</summary>
    /// <remarks>EntityFrameworkCore chooses the constructor with the least amount of parameters (including 0) where all of them correspond to a property.</remarks>
    protected Entity() { }
}
