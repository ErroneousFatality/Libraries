using System.Linq.Expressions;

namespace AndrejKrizan.DotNet.Entities;
public interface IKey<TEntity, TSelf>
    where TEntity : class
    where TSelf: IKey<TEntity, TSelf>
{
    static abstract Expression<Func<TEntity, TSelf>> Lambda { get; }
}
