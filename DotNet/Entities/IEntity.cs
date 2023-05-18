namespace AndrejKrizan.DotNet.Entities;

public interface IEntity<TId>
{
    TId Id { get; }
}