using System.Linq.Expressions;

namespace AndrejKrizan.EntityFramework.Common.Extensions.IQueryables;

public class PropertySelection<TEntity>
        where TEntity : class
{
    // Properties
    public Expression<Func<TEntity, object>> PropertySelector { get; set; }
    public Expression<Func<TEntity, object>>[] AdditionalPropertySelectors { get; set; }

    // Constructors
    public PropertySelection(
        Expression<Func<TEntity, object>> firstPropertySelector,
        params Expression<Func<TEntity, object>>[] additionalPropertySelectors
    )
    {
        PropertySelector = firstPropertySelector;
        AdditionalPropertySelectors = additionalPropertySelectors;
    }

    public static implicit operator PropertySelection<TEntity>(Expression<Func<TEntity, object>> propertySelector) => new(propertySelector);
}
