using AndrejKrizan.EntityFramework.SqlServer;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndrejKrizan.EntityFramework.SqlServer.Extensions;

public static class PropertyBuilderExtensions
{
    public static PropertyBuilder IsCaseSensitive(this PropertyBuilder propertyBuilder)
        => propertyBuilder.UseCollation(Collations.CaseSensitive);

    public static PropertyBuilder IsCaseInsensitive(this PropertyBuilder propertyBuilder)
        => propertyBuilder.UseCollation(Collations.CaseInsensitive);
}
