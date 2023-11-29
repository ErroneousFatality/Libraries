using Microsoft.Extensions.Configuration;

namespace AndrejKrizan.DotNet.Configurations;

public static class IConfigurationExtensions
{
    /// <exception cref="ArgumentException">The value was not found inside the configuration.</exception>
    public static string GetRequiredValue(this IConfiguration configuration, string key)
        => configuration[key]
            ?? throw new ArgumentException($"The \"{key}\" value was not found inside the configuration.", nameof(configuration));

    /// <exception cref="ArgumentException">The value was not found inside the configuration section.</exception>
    public static string GetRequiredValue(this IConfigurationSection configurationSection, string key)
        => configurationSection[key]
            ?? throw new ArgumentException($"The \"{key}\" value was not found inside the configuration section at path \"{configurationSection.Path}\".", nameof(configurationSection));


    /// <exception cref="ArgumentException">The settings were not found or are invalid inside the configuration.</exception>
    public static TSettings GetRequiredSettings<TSettings>(this IConfiguration configuration, string key)
        => configuration.GetRequiredSection(key).Get<TSettings>()
            ?? throw new ArgumentException($"The \"{key}\" settings were not found or are invalid inside the configuration.", nameof(configuration));

    /// <exception cref="ArgumentException">The settings were not found or are invalid inside the configuration section.</exception>
    public static TSettings GetRequiredSettings<TSettings>(this IConfigurationSection configurationSection, string key)
        => configurationSection.GetRequiredSection(key).Get<TSettings>()
            ?? throw new ArgumentException($"The \"{key}\" settings were not found or are invalid inside the configuration section at path \"{configurationSection.Path}\".", nameof(configurationSection));
}
