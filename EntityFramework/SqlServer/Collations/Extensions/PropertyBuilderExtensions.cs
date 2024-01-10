using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndrejKrizan.EntityFramework.SqlServer.Collations;

public static partial class CollationExtensions
{
    public static PropertyBuilder IsCaseSensitive(this PropertyBuilder property)
        => property.UseCollation(Predefined.CaseSensitive);

    public static PropertyBuilder IsCaseInsensitive(this PropertyBuilder property)
        => property.UseCollation(Predefined.CaseInsensitive);
}
