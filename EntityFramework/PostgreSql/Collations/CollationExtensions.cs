using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndrejKrizan.EntityFramework.PostgreSql.Collations
{
    public static class CollationExtensions
    {
        #region CaseAndAccentInsensitiveUnicode
        private const string CaseAndAccentInsensitiveUnicodeCollationName = "case_and_accent_insensitive_unicode";

        public static ModelBuilder CreateCaseAndAccentInsensitiveUnicodeCollation(this ModelBuilder modelBuilder)
            => modelBuilder.HasCollation(CaseAndAccentInsensitiveUnicodeCollationName, locale: "und-u-ks-level1", provider: "icu", deterministic: false);

        public static PropertiesConfigurationBuilder<string> UseCaseAndAccentInsensitiveUnicodeCollation(this PropertiesConfigurationBuilder<string> propertyConfigurator)
            => propertyConfigurator.UseCollation(CaseAndAccentInsensitiveUnicodeCollationName);
        #endregion
    }
}
