using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using AndrejKrizan.DotNet.Entities;
using AndrejKrizan.DotNet.ValueObjects.PropertyBindings;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndrejKrizan.EntityFramework.Common.Extensions;
public static class EntityTypeBuilderExtensions
{
    // Methods
    public static KeyBuilder HasKey<TEntity, TKey>(this EntityTypeBuilder<TEntity> entity)
        where TEntity : class
        where TKey: struct, IKey<TEntity, TKey>
    {
        ImmutableArray<IPropertyBinding<TEntity, TKey>> propertyBindings = IKey<TEntity, TKey>.PropertyBindings;
        Type type = CreateAnonymousKeyType(propertyBindings);
        ConstructorInfo constructor = type.GetConstructor(propertyBindings.Select(property => property.Type).ToArray())!;
        IEnumerable<MemberExpression> arguments = propertyBindings.Select(property => (MemberExpression)property.EntityProperty.Expression);
        Expression<Func<TEntity, object?>> construction = Expression.Lambda<Func<TEntity, object?>>(
            Expression.New(constructor, arguments), 
            IKey<TEntity, TKey>.EntityParameter
        );
        return entity.HasKey(construction);
    }

    // Private methods
    private static Type CreateAnonymousKeyType<TEntity, TKey>(IEnumerable<IPropertyBinding<TEntity, TKey>> propertyBindings)
        where TEntity: class
        where TKey : struct, IKey<TEntity, TKey>
    {
        TypeBuilder builder = AssemblyBuilder
            .DefineDynamicAssembly(new AssemblyName("DynamicAnonymousTypeCreator"), AssemblyBuilderAccess.RunAndCollect)
            .DefineDynamicModule("DynamicAnonymousTypeCreator")
            .DefineType(typeof(TKey).Name, TypeAttributes.Public);
        foreach (IPropertyBinding<TEntity, TKey> propertyBinding in propertyBindings)
        {
            PropertyInfo property = propertyBinding.EntityProperty.PropertyInfo;
            builder.DefineField(property.Name, property.PropertyType, FieldAttributes.Public);
        }
        Type type = builder.CreateType();
        return type;
    }
}
