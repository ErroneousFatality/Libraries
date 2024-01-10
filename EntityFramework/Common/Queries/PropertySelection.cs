using System.Collections.Immutable;
using System.Linq.Expressions;

namespace AndrejKrizan.EntityFramework.Common.Queries;

public sealed class PropertySelection<TEntity>
{
    // Properties
    public Expression<Func<TEntity, object?>> Property { get; }
    public Expression<Func<TEntity, object?>>[] AdditionalProperties { get; }

    // Constructors
    public PropertySelection(
        Expression<Func<TEntity, object?>> property,
        params Expression<Func<TEntity, object?>>[] additionalProperties
    )
    {
        Property = property;
        AdditionalProperties = additionalProperties;
    }

    public PropertySelection(IEnumerable<Expression<Func<TEntity, object?>>> properties)
    {
        Property = properties.First();
        AdditionalProperties = properties.Skip(1).ToArray();
    }

    // Conversions
    public static implicit operator PropertySelection<TEntity>(Expression<Func<TEntity, object?>> property) => new(property);
    public static implicit operator PropertySelection<TEntity>(Expression<Func<TEntity, object?>>[] properties) => new(properties);
    public static implicit operator PropertySelection<TEntity>(ImmutableArray<Expression<Func<TEntity, object?>>> properties) => new(properties);
    public static implicit operator PropertySelection<TEntity>(List<Expression<Func<TEntity, object?>>> properties) => new(properties);
}
