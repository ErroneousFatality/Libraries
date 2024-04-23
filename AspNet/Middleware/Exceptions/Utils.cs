using System.Net;
using System.Net.Mime;

using AndrejKrizan.DotNet.Exceptions;

using Microsoft.AspNetCore.Http;

namespace AndrejKrizan.AspNet.Middleware.Exceptions;

internal static class Utils
{
    internal static HttpStatusCode ExceptionToStatusCode(Exception exception)
        => exception switch
        {
            NotFoundException => HttpStatusCode.NotFound,
            ArgumentException => HttpStatusCode.BadRequest,
            AuthorizationException => HttpStatusCode.Unauthorized,
            InvalidOperationException => HttpStatusCode.Forbidden,
            _ => HttpStatusCode.InternalServerError
        };

    internal static async Task WriteJsonToHttpResponseAsync<TResponse>(HttpResponse httpResponse, HttpStatusCode statusCode, TResponse response)
    {
        httpResponse.ContentType = MediaTypeNames.Application.Json;
        httpResponse.StatusCode = (int)statusCode;
        await httpResponse.WriteAsJsonAsync(response);
    }
}
