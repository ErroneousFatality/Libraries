namespace AndrejKrizan.DotNet.Net.Http.Json.ResponseResults;

public sealed class ErrorResponse
{
    public required string Message { get; init; }
    public string? StackTrace { get; init; }
}
