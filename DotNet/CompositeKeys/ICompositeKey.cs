using System.Linq.Expressions;

namespace AndrejKrizan.DotNet.CompositeKeys;
public interface ICompositeKey<TEntity, TSelf>
    where TEntity : notnull
    where TSelf : CompositeKey<TEntity,TSelf>, ICompositeKey<TEntity, TSelf>
{
    // Static properties

    /// <summary>
    ///     Must use an object initializer.<br/>
    ///     Example:
    ///     <code>
    ///         (Entity entity) => new EntityKey { A = entity.A, B = entity.B }
    ///     </code>
    /// </summary>
    static abstract Expression<Func<TEntity, TSelf>> Selector { get; }
}
