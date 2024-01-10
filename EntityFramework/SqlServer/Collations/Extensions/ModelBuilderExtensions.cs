using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.SqlServer.Collations;

public static partial class CollationExtensions
{
    public static ModelBuilder UseCaseSensitiveCollation(this ModelBuilder model)
        => model.UseCollation(Predefined.CaseSensitive);

    public static ModelBuilder UseCaseInsensitiveCollation(this ModelBuilder model)
        => model.UseCollation(Predefined.CaseInsensitive);
}
