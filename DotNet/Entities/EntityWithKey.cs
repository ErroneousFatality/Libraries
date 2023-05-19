using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using AndrejKrizan.DotNet.Utilities;

[assembly: InternalsVisibleTo("AndrejKrizan.EntityFramework.Common")]

namespace AndrejKrizan.DotNet.Entities;
public abstract class EntityWithKey<TKey, TSelf> : IEntity<TKey>
    where TKey : struct
    where TSelf : EntityWithKey<TKey, TSelf>
{
    // Properties
    public TKey Id => GetKey((TSelf)this);

    // Constructors
    protected EntityWithKey() { }

    // Protected properties
    protected internal abstract Expression<Func<TSelf, TKey>> KeyLambda { get; }

    // Private static fields
    private readonly static Func<TSelf, TKey> GetKey = Utils.GetFromDefaultInstance((TSelf e) => e.KeyLambda).Compile();
}
