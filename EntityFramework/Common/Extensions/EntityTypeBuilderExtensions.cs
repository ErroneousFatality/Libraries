using System.Linq.Expressions;

using AndrejKrizan.DotNet.Entities;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndrejKrizan.EntityFramework.Common.Extensions;
public static class EntityTypeBuilderExtensions
{
    public static KeyBuilder HasKey<TEntity, TKey>(this EntityTypeBuilder<TEntity> entity)
        where TEntity : class
        where TKey: IKey<TEntity, TKey>
    {
        Expression<Func<TEntity, TKey>> keyLambda = TKey.Lambda;
        Expression<Func<TEntity, object?>> objectLambda = Expression.Lambda<Func<TEntity, object?>>(
            Expression.Convert(keyLambda.Body, typeof(object)),
            keyLambda.Parameters
        );
        return entity.HasKey(objectLambda);
    }
}
