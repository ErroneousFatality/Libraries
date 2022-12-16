using Microsoft.Extensions.Configuration;

namespace AndrejKrizan.Common.Extensions
{
    public static class IConfigurationExtensions
    {
        public static string GetRequiredValue(this IConfiguration configuration, string key)
            => configuration[key]
                ?? throw new ArgumentException($"The \"{key}\" value was not found inside the configuration.", nameof(configuration));
        public static string GetRequiredValue(this IConfigurationSection configurationSection, string key)
            => configurationSection[key]
                ?? throw new ArgumentException($"The \"{key}\" value was not found inside the configuration section at path \"{configurationSection.Path}\".", nameof(configurationSection));

        public static TSettings GetRequiredSettings<TSettings>(this IConfiguration configuration, string key)
            => configuration.GetRequiredSection(key).Get<TSettings>()
                ?? throw new ArgumentException($"The \"{key}\" settings were not found or are invalid inside the configuration.", nameof(configuration));
        public static TSettings GetRequiredSettings<TSettings>(this IConfigurationSection configurationSection, string key)
            => configurationSection.GetRequiredSection(key).Get<TSettings>()
                ?? throw new ArgumentException($"The \"{key}\" settings were not found or are invalid inside the configuration section at path \"{configurationSection.Path}\".", nameof(configurationSection));
    }
}
