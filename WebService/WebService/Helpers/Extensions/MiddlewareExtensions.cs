using Microsoft.AspNetCore.Builder;
using WebService.Middleware;

namespace WebService.Helpers.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddelware(this IApplicationBuilder builder)
            => builder.UseMiddleware<ExceptionMiddleware>();
    }
}