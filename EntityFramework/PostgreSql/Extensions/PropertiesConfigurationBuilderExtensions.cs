using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndrejKrizan.EntityFramework.PostgreSql.Extensions;
public static class PropertiesConfigurationBuilderExtensions
{
    public static PropertiesConfigurationBuilder<TProperty> HaveJsonConversion<TProperty>(this PropertiesConfigurationBuilder<TProperty> property)
        => Common.Extensions.PropertiesConfigurationBuilderExtensions.HaveJsonConversion(property).HaveColumnType("jsonb");
}
