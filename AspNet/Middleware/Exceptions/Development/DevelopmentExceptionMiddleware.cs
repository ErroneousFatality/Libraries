using System.Net;
using System.Net.Mime;

using AndrejKrizan.Common.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace AndrejKrizan.AspNet.Middleware.Exceptions.Development
{
    public class DevelopmentExceptionMiddleware
    {
        // Fields
        private readonly RequestDelegate NextAsync;
        private readonly ILogger<DevelopmentExceptionMiddleware> Logger;

        // Constructors
        public DevelopmentExceptionMiddleware(RequestDelegate nextAsync, ILogger<DevelopmentExceptionMiddleware> logger)
        {
            NextAsync = nextAsync;
            Logger = logger;
        }

        // Methods
        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (IsContentHumanReadable(httpContext.Request.ContentType))
            {
                httpContext.Request.EnableBuffering();
            }
            try
            {
                await NextAsync(httpContext);
            }
            catch (Exception exception)
            {
                if (httpContext.RequestAborted.IsCancellationRequested)
                {
                    return;
                }
                HttpStatusCode statusCode = Utils.ExceptionToStatusCode(exception);
                await LogRequestAndExceptionAsync(httpContext.Request, exception, statusCode);
                await Utils.WriteJsonToHttpResponseAsync(httpContext.Response, statusCode, new DevelopmentExceptionResponse(exception));
            }
        }

        // Private methods
        private async Task LogRequestAndExceptionAsync(HttpRequest request, Exception exception, HttpStatusCode statusCode)
        {
            string content;
            if (IsContentHumanReadable(request.ContentType))
            {
                request.Body.Position = 0;
                content = await request.Body.ReadAsync();
            }
            else if (request.HasFormContentType)
            {
                content = request.Form.Files.Select(formFile => formFile.FileName).StringJoin();
            }
            else
            {
                content = request.ContentType ?? "\\";
            }

            Logger.Log(
                statusCode == HttpStatusCode.InternalServerError ? LogLevel.Error : LogLevel.Information,
                exception,
                "A {requestMethod} request to {requestPathAndQuery} resulted in response {responseStatusCode} ({responseStatus}).\nContent: {content}",
                request.Method, request.GetEncodedPathAndQuery(), (int)statusCode, statusCode, content
            );
        }

        private static bool IsContentHumanReadable(string? contentType)
            => !string.IsNullOrWhiteSpace(contentType) 
            && contentType.ContainsAny(StringComparison.InvariantCultureIgnoreCase,
                MediaTypeNames.Text.Plain,
                MediaTypeNames.Text.RichText,
                MediaTypeNames.Text.Xml,
                MediaTypeNames.Application.Json,
                MediaTypeNames.Application.Xml,
                MediaTypeNames.Application.Soap
            );
    }
}
