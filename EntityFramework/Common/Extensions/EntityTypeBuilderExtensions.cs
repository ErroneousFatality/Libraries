using AndrejKrizan.DotNet.Entities;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndrejKrizan.EntityFramework.Common.Extensions;
public static class EntityTypeBuilderExtensions
{
    // Methods
    public static KeyBuilder HasKey<TEntity, TKey>(this EntityTypeBuilder<TEntity> entity)
        where TEntity : class
        where TKey: struct, IKey<TEntity, TKey>
        => entity.HasKey(IKey<TEntity, TKey>.PropertyBindings.Select(binding => binding.EntityProperty.PropertyInfo.Name).ToArray());
}
