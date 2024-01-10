using AndrejKrizan.EntityFramework.PostgreSql.Collations;
using AndrejKrizan.EntityFramework.PostgreSql.Collations.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndrejKrizan.EntityFramework.PostgreSql.Collations.Extensions;
public static partial class CollationExtensions
{
    public static IConventionPropertyBuilder? UseCollation(this IConventionPropertyBuilder propertyConvention,
        Collation collation
    )
        => propertyConvention.UseCollation(collation.Name);
}
