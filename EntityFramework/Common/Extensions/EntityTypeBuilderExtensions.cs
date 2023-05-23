using System.Linq.Expressions;

using AndrejKrizan.DotNet.Entities;
using AndrejKrizan.DotNet.Extensions;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndrejKrizan.EntityFramework.Common.Extensions;
public static class EntityTypeBuilderExtensions
{
    public static KeyBuilder HasKey<TEntity, TKey>(this EntityTypeBuilder<TEntity> entity)
        where TEntity : class
        where TKey: IKey<TEntity>
    {
        Expression<Func<TEntity, object?>> _lambda = TKey.Lambda;
        Expression<Func<TEntity, object?>> lambda = Expression.Lambda<Func<TEntity, object?>>(
            _lambda.Body.UnwrapConversion<object?, TKey>(),
            _lambda.Parameters
        );
        return entity.HasKey(lambda);
    }
        
}
