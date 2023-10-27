using AndrejKrizan.DotNet.CompositeKeys;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndrejKrizan.EntityFramework.Common.Extensions;
public static class EntityTypeBuilderExtensions
{
    // Methods
    public static KeyBuilder HasKey<TEntity, TKey>(this EntityTypeBuilder<TEntity> entity)
        where TEntity : class
        where TKey: ICompositeKey<TEntity, TKey>
        => entity.HasKey(ICompositeKey<TEntity, TKey>.PropertyBindings.Select(binding => binding.EntityNavigation.Info.Name).ToArray());
}
