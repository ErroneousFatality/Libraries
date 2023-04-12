using AndrejKrizan.AspNet.Middleware.Exceptions;
using AndrejKrizan.AspNet.Middleware.Exceptions.Development;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AndrejKrizan.AspNet.Extensions;

public static class IApplicationBuilderExtensions
{
    /// <summary>Registers the <see cref="DevelopmentExceptionMiddleware"/> when the environment is development, otherwise registers the <see cref="ExceptionMiddleware"/>.</summary>
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder application, bool isDevelopment)
    {
        if (isDevelopment)
        {
            application.UseMiddleware<DevelopmentExceptionMiddleware>();
        }
        else
        {
            application.UseMiddleware<ExceptionMiddleware>();
        }
        return application;
    }

    /// <summary>Registers the <see cref="DevelopmentExceptionMiddleware"/> when the environment is development, otherwise registers the <see cref="ExceptionMiddleware"/>.</summary>
    /// <param name="hostEnvironment">Used to check if it's development.</param>
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder application, IHostEnvironment hostEnvironment)
        => application.UseExceptionMiddleware(isDevelopment: hostEnvironment.IsDevelopment());

    /// <summary>Registers the <see cref="DevelopmentExceptionMiddleware"/> when the environment is development, otherwise registers the <see cref="ExceptionMiddleware"/>.</summary>
    /// <param name="webHostBuilderContext">Used to access its <see cref="IHostEnvironment"/> to check if it's development.</param>
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder application, WebHostBuilderContext webHostBuilderContext)
        => application.UseExceptionMiddleware(hostEnvironment: webHostBuilderContext.HostingEnvironment);
}
