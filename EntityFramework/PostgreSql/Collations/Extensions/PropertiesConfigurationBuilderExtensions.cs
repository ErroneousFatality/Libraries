using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndrejKrizan.EntityFramework.PostgreSql.Collations;

public static partial class CollationExtensions
{
    public static PropertiesConfigurationBuilder UseCollation(this PropertiesConfigurationBuilder propertyConfigurator, Collation collation)
        => propertyConfigurator.UseCollation(collation.Name);
}
