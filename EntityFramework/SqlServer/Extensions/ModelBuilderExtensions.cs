using AndrejKrizan.EntityFramework.SqlServer;

using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.SqlServer.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder UseCaseSensitiveCollation(this ModelBuilder modelBuilder)
        => modelBuilder.UseCollation(Collations.CaseSensitive);

    public static ModelBuilder UseCaseInsensitiveCollation(this ModelBuilder modelBuilder)
        => modelBuilder.UseCollation(Collations.CaseInsensitive);
}
