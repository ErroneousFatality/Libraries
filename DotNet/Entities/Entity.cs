using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using AndrejKrizan.DotNet.Utilities;

[assembly: InternalsVisibleTo("AndrejKrizan.EntityFramework.Common")]
namespace AndrejKrizan.DotNet.Entities;
public abstract class Entity<TKey, TSelf>
    where TKey : struct
    where TSelf : Entity<TKey, TSelf>
{
    // Constructors
    protected Entity() { }

    // Methods
    public TKey ToKey()
        => CreateKey((TSelf)this);

    // Protected properties
    protected internal abstract Expression<Func<TSelf, TKey>> KeyLambda { get; }

    // Static protected properties
    public static readonly Func<TSelf, TKey> CreateKey = Utils.GetFromDefaultInstance((TSelf entity) => entity.KeyLambda).Compile();
}
