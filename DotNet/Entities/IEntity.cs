namespace AndrejKrizan.DotNet.Entities;

public interface IEntity<TId>
{
    public TId Id { get; }
}