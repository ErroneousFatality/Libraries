using System.Linq.Expressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AndrejKrizan.EntityFramework.Common")]
namespace AndrejKrizan.DotNet;
public abstract class Entity<TKey, TSelf>
    where TKey : struct
    where TSelf : Entity<TKey, TSelf>
{
    // Constructors
    protected Entity() { }

    // Protected properties
    protected internal abstract Expression<Func<TSelf, TKey>> KeyLambda { get; }
}
