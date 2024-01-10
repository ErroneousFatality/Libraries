using System.Diagnostics.CodeAnalysis;

namespace AndrejKrizan.EntityFramework.PostgreSql.Collations;

public class Collation
{
    // Properties
    public required string Name { get; init; }
    public required string Locale { get; init; }
    public string? Provider { get; init; }
    public bool Deterministic { get; }

    // Constructors
    public Collation() { }

    [SetsRequiredMembers]
    public Collation(string name, string locale, string? provider = null, bool deterministic = false)
    {
        Name = name;
        Locale = locale;
        Provider = provider;
        Deterministic = deterministic;
    }
}
