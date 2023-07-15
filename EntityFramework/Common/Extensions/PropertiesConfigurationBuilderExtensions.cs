using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AndrejKrizan.EntityFramework.Common.Converters;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndrejKrizan.EntityFramework.Common.Extensions;
public static class PropertiesConfigurationBuilderExtensions
{
    public static PropertiesConfigurationBuilder<TProperty> HaveJsonConversion<TProperty>(this PropertiesConfigurationBuilder<TProperty> property)
        => property.HaveConversion<JsonConverter<TProperty>>();
}
