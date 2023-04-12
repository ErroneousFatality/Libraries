namespace AndrejKrizan.EntityFramework.PostgreSql.Collations;

public readonly struct Collation
{
    // Fields
    public readonly string Name;
    public readonly string Locale;
    public readonly string Provider;
    public readonly bool? Deterministic;

    // Constructors
    public Collation(string name, string locale, string provider, bool? deterministic = null)
    {
        Name = name;
        Locale = locale;
        Provider = provider;
        Deterministic = deterministic;
    }
}
