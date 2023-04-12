using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndrejKrizan.EntityFramework.PostgreSql
{
    public static class Collations
    {
        #region UnicodeAccentInsensitiveCaseInsensitive
        public const string UnicodeAccentInsensitiveCaseInsensitive = "unicode_accent_insensitive_case_insensitive";

        public static ModelBuilder UseUnicodeAccentInsensitiveCaseInsensitive(this ModelBuilder modelBuilder)
            => modelBuilder
                .HasCollation(UnicodeAccentInsensitiveCaseInsensitive, locale: "und-u-ks-level1", provider: "icu", deterministic: false)
                .UseCollation(UnicodeAccentInsensitiveCaseInsensitive);

        public static PropertiesConfigurationBuilder<string> UseUnicodeAccentInsensitiveCaseInsensitive(this PropertiesConfigurationBuilder<string> propertyConfigurator)
            => propertyConfigurator.UseCollation(UnicodeAccentInsensitiveCaseInsensitive);
        #endregion
    }
}
