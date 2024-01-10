using System.Diagnostics.CodeAnalysis;

namespace AndrejKrizan.ElasticSearch.Configuration;

public class ElasticSearchSettings
{
    // Properties
    public required string Uri { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }

    // Constructors
    public ElasticSearchSettings() { }

    [SetsRequiredMembers]
    public ElasticSearchSettings(string uri, string username, string password)
    {
        Uri = uri;
        Username = username;
        Password = password;
    }
}
