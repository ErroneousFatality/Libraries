using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace AndrejKrizan.DotNet.Net.Http.Json.ResponseResults;

public sealed class ResponseResult<TContent> : ResponseResult
{
    // Properties
    public TContent? Content { get; }

    [MemberNotNullWhen(true, nameof(Content))]
    public override bool IsSuccessful 
        => base.IsSuccessful;

    // Constructors
    private ResponseResult(HttpStatusCode statusCode, bool isSuccessful, TContent? content, ErrorResponse? error) 
        : base(statusCode, isSuccessful, error)
    {
        Content = content;
    }

    // Static factory methods
    internal static ResponseResult<TContent> CreateSuccessful(TContent? content, HttpStatusCode statusCode)
    => new(statusCode, isSuccessful: true, content, error: null);

    internal static new ResponseResult<TContent> CreateErroneous(ErrorResponse error, HttpStatusCode statusCode)
        => new(statusCode, isSuccessful: false, content: default, error);

    [Obsolete("For a response result with content to be successful, the content must be provided.", error: true)]
    internal static new ResponseResult<TContent> CreateSuccessful(HttpStatusCode statusCode)
        => throw new NotSupportedException();
}
