using System.Text.Json;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AndrejKrizan.EntityFramework.Common.Conversions.Json;
public class JsonConverter<T> : ValueConverter<T?, string?>
{
    // Constructors
    public JsonConverter()
        : base(
            value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
            json => json == null
                ? default
                : JsonSerializer.Deserialize<T>(json, (JsonSerializerOptions?)null)
        )
    { }
}
