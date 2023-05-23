namespace AndrejKrizan.DotNet.Entities;
public abstract class Entity<TId>
    where TId: struct
{
    // Properties
    public TId Id { get; private set; }

    // Constructors
    protected Entity() { }

    protected Entity(TId id)
    {
        Id = id;
    }
}
