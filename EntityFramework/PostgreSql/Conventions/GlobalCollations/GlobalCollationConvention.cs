using AndrejKrizan.EntityFramework.PostgreSql.Collations;
using AndrejKrizan.EntityFramework.PostgreSql.Collations.Extensions;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace AndrejKrizan.EntityFramework.PostgreSql.Conventions.GlobalCollations;
public class GlobalCollationConvention : IModelFinalizingConvention
{
    // Static fields
    private static readonly Dictionary<Type, HashSet<string>> IgnoredEntityProperties = [];

    // Static methods
    public static void Ignore(Type entity, string property)
    {
        if (IgnoredEntityProperties.TryGetValue(entity, out HashSet<string>? ignoredProperties))
            ignoredProperties.Add(property);
        else
            IgnoredEntityProperties.Add(entity, [property]);
    }

    // Fields
    private readonly Collation Collation;

    // Constructors
    public GlobalCollationConvention(Collation collation)
    {
        Collation = collation;
    }

    // Methods
    public void ProcessModelFinalizing(IConventionModelBuilder model, IConventionContext<IConventionModelBuilder> context)
    {
        IEnumerable<IConventionEntityType> entities = model.Metadata.GetEntityTypes();
        IEnumerable<IConventionProperty> stringPropertiesToCollate = entities
            .SelectMany(entity => entity.GetDeclaredProperties()
                .Where(property
                    => property.ClrType == typeof(string)
                    && !(
                        IgnoredEntityProperties.TryGetValue(entity.ClrType, out HashSet<string>? ignoredProperties)
                        && ignoredProperties.Contains(property.Name)
                    )
                )
            );
        foreach (IConventionProperty property in stringPropertiesToCollate)
            property.Builder.UseCollation(Collation);
    }
}
