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
        Type type = CreateAnonymousKeyType<TEntity, TKey>();
        ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes)!;
        IEnumerable<Expression> parameters = IKey<TEntity, TKey>.PropertyBindings.Select(property => property.EntityProperty.Expression);
        NewExpression construction = Expression.New(constructor, parameters);
        Expression<Func<TEntity, object?>> constructionLambda = Expression.Lambda<Func<TEntity, object?>>(construction, IKey<TEntity, TKey>.EntityParameter);
        return entity.HasKey(constructionLambda);
    }

    // Private methods
    private static Type CreateAnonymousKeyType<TEntity, TKey>()
        where TEntity: class
        where TKey : struct, IKey<TEntity, TKey>
    {
        TypeBuilder builder = AssemblyBuilder
            .DefineDynamicAssembly(new AssemblyName("DynamicAnonymousTypeCreator"), AssemblyBuilderAccess.RunAndCollect)
            .DefineDynamicModule("DynamicAnonymousTypeCreator")
            .DefineType(typeof(TKey).Name, TypeAttributes.Public);
        foreach (IPropertyBinding<TEntity, TKey> propertyBinding in IKey<TEntity, TKey>.PropertyBindings)
        {
            PropertyInfo property = propertyBinding.EntityProperty.PropertyInfo;
            builder.DefineField(property.Name, property.PropertyType, FieldAttributes.Public);
        }
        Type type = builder.CreateType();
        return type;
    }
}
