using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace AndrejKrizan.DotNet.Net.Http.Json.ResponseResults;

public class ResponseResult
{
    // Properties
    [MemberNotNullWhen(false, nameof(Error))]
    public virtual bool IsSuccessful { get; }

    public HttpStatusCode StatusCode { get; }

    public ErrorResponse? Error { get; }

    // Constructors
    protected ResponseResult(HttpStatusCode statusCode, bool isSuccessful, ErrorResponse? error)
    {
        StatusCode = statusCode;
        IsSuccessful = isSuccessful;
        Error = error;
    }

    // Static factory methods
    internal static ResponseResult CreateSuccessful(HttpStatusCode statusCode)
        => new(statusCode, isSuccessful: true, error: null);
    internal static ResponseResult CreateErroneous(ErrorResponse error, HttpStatusCode statusCode) 
        => new(statusCode, isSuccessful: false, error);

    // Methods

    /// <exception cref="HttpRequestException"></exception>
    public void EnsureIsSuccessful()
    {
        if (!IsSuccessful)
        {
            throw new HttpRequestException(Error.Message, inner: null, StatusCode);
        }
    }
}
