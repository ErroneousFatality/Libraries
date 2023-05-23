using System.Linq.Expressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AndrejKrizan.EntityFramework.Common")]
namespace AndrejKrizan.DotNet.Entities;
public abstract class EntityWithId<TKey, TSelf> : Entity<TKey, TSelf>
    where TKey : struct
    where TSelf: EntityWithId<TKey, TSelf>
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
    protected internal override Expression<Func<TSelf, TKey>> KeyLambda
        => entity => entity.Id;
}
