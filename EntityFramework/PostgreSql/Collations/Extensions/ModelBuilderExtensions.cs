using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.PostgreSql.Collations;

public static partial class CollationExtensions
{
    public static ModelBuilder HasCollation(this ModelBuilder modelBuilder, Collation collation)
        => modelBuilder.HasCollation(collation.Name, collation.Locale, collation.Provider, collation.Deterministic);

    public static ModelBuilder UseCollation(this ModelBuilder modelBuilder, Collation collation)
        => collation.Deterministic
            ? throw new ArgumentException("PostgreSql doesn't support setting non-deterministic collations on the database level. See: https://www.npgsql.org/efcore/misc/collations-and-case-sensitivity.html?tabs=data-annotations.", nameof(collation))
            : modelBuilder.HasCollation(collation).UseCollation(collation.Name);
}
