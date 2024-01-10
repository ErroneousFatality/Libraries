using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndrejKrizan.EntityFramework.PostgreSql.Conversions;
public static class PropertiesConfigurationBuilderExtensions
{
    public static PropertiesConfigurationBuilder<TProperty> HaveJsonConversion<TProperty>(this PropertiesConfigurationBuilder<TProperty> property)
        => Common.Conversions.Json.PropertiesConfigurationBuilderExtensions.HaveJsonConversion(property).HaveColumnType("jsonb");
}
