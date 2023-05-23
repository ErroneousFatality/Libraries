using System.Linq.Expressions;
using AndrejKrizan.DotNet.Entities;
using AndrejKrizan.DotNet.Utilities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndrejKrizan.EntityFramework.Common.Configurations;

/// <summary>Sets the key from entity.</summary>
public abstract class EntityConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity<TKey, TEntity>
    where TKey : struct
{
    // Methods
    public void Configure(EntityTypeBuilder<TEntity> entity)
    {
        Expression<Func<TEntity, TKey>> keyLambda = Utils.GetFromDefaultInstance((TEntity entity) => entity.KeyLambda);
        Expression<Func<TEntity, object?>> keyObjectLambda = Expression.Lambda<Func<TEntity, object?>>(keyLambda.Body, keyLambda.Parameters.Single());
        entity.HasKey(keyObjectLambda);
        ConfigureEntity(entity);
    }

    // Protected methods
    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> entity);
}
