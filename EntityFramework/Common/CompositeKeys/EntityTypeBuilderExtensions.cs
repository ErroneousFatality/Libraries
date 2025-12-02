using AndrejKrizan.DotNet.CompositeKeys;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndrejKrizan.EntityFramework.Common.CompositeKeys;
public static class EntityTypeBuilderExtensions
{
    // Methods
    public static KeyBuilder HasKey<TEntity, TKey>(this EntityTypeBuilder<TEntity> entity)
        where TEntity : class
        where TKey : CompositeKey<TEntity, TKey>, ICompositeKey<TEntity, TKey>
        => entity.HasKey(CompositeKey<TEntity, TKey>.GetEntityPropertyNames());
}
