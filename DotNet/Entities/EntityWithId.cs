using System.Linq.Expressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AndrejKrizan.EntityFramework.Common")]
namespace AndrejKrizan.DotNet.Entities;
public abstract class EntityWithId<TKey> : Entity<TKey, EntityWithId<TKey>>
    where TKey : struct
{
    // Properties
    public TKey Id { get; private set; }


    // Constructors
    protected EntityWithId(TKey id)
    {
        Id = id;
    }

    protected EntityWithId() { }

    // Protected properties
    protected internal override Expression<Func<EntityWithId<TKey>, TKey>> KeyLambda
        => entity => entity.Id;
}
