using AndrejKrizan.EntityFramework.PostgreSql.Collations;

using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.PostgreSql.Conventions.GlobalCollations;
public static partial class GlobalCollationExtensions
{
    public static void AddGlobalCollationConvention(this ModelConfigurationBuilder model, Collation collation)
        => model.Conventions.Add(_ => new GlobalCollationConvention(collation));
}
