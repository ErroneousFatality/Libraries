using AndrejKrizan.EntityFramework.PostgreSql.Collations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndrejKrizan.EntityFramework.PostgreSql.Collations;

public static partial class CollationExtensions
{
    public static PropertiesConfigurationBuilder UseCollation(this PropertiesConfigurationBuilder property,
        Collation collation
    )
        => property.UseCollation(collation.Name);
}
