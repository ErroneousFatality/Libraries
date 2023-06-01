namespace AndrejKrizan.EntityFramework.PostgreSql.Collations;
public static class Predefined
{
    public static readonly Collation UnicodeAccentInsensitiveCaseInsensitive
        = new("unicode_accent_insensitive_case_insensitive_deterministic", locale: "und-u-ks-level1", provider: "icu", deterministic: false);
}
