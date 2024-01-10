using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndrejKrizan.EntityFramework.Common.Conversions.Json;
public static class PropertiesConfigurationBuilderExtensions
{
    public static PropertiesConfigurationBuilder<TProperty> HaveJsonConversion<TProperty>(this PropertiesConfigurationBuilder<TProperty> property)
        => property.HaveConversion<JsonConverter<TProperty>>();
}
