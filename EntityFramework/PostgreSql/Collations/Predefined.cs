namespace AndrejKrizan.EntityFramework.PostgreSql.Collations;
public static class Predefined
{
    public static readonly Collation UnicodeAccentInsensitiveCaseInsensitive 
        = new("unicode_accent_insensitive_case_insensitive", locale: "und-u-ks-level1", provider: "icu");

    public static readonly Collation UnicodeAccentInsensitiveCaseInsensitiveUndeterministic
        = new("unicode_accent_insensitive_case_insensitive_deterministic", locale: "und-u-ks-level1", provider: "icu", deterministic: true);

    public static readonly Collation UnicodeAccentInsensitiveCaseInsensitiveDeterministic
        = new("unicode_accent_insensitive_case_insensitive_undeterministic", locale: "und-u-ks-level1", provider: "icu", deterministic: false);
}
