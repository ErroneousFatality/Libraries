using System.Net;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AndrejKrizan.AspNet.Middleware.Exceptions
{
    public class ExceptionMiddleware
    {
        // Fields
        private readonly RequestDelegate NextAsync;
        private readonly ILogger<ExceptionMiddleware> Logger;

        // Constructors
        public ExceptionMiddleware(RequestDelegate nextAsync, ILogger<ExceptionMiddleware> logger)
        {
            NextAsync = nextAsync;
            Logger = logger;
        }

        // Methods
        public async Task InvokeAsync(HttpContext httpContext)
        {
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
                if (statusCode == HttpStatusCode.InternalServerError)
                {
                    Logger.LogError(exception, "A {requestMethod} request to {requestPath} resulted in an internal server error.",
                        httpContext.Request.Method, httpContext.Request.Path
                    );
                }
                await Utils.WriteJsonToHttpResponseAsync(httpContext.Response, statusCode, new ExceptionResponse(exception));
            }
        }
    }
}
