using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndrejKrizan.EntityFramework.PostgreSql.Conventions.GlobalCollations;
public static partial class GlobalCollationExtensions
{
    public static PropertyBuilder<string> IgnoreGlobalCollation(this PropertyBuilder<string> property)
    {
        IMutableProperty metadata = property.Metadata;
        GlobalCollationConvention.Ignore(metadata.DeclaringType.ClrType, metadata.Name);
        return property;
    }
}
