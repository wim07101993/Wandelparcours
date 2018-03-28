using Microsoft.AspNetCore.Builder;
using WebService.Middleware;

namespace WebService.Helpers.Extensions
{
    /// <summary>
    /// MiddlewareExtensions is a static class that holds extension methods to add middleware to an <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddelware(this IApplicationBuilder builder)
            => builder.UseMiddleware<ExceptionMiddleware>();
    }
}