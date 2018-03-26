using Microsoft.AspNetCore.Builder;
using WebService.Middleware;

namespace WebService.Helpers.Extensions
{
    /// <summary>
    /// MiddlewareExtensions is a static class that holds extension methods to add middleware to an <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// UseExceptionMiddleware adds the <see cref="ExceptionMiddleware"/> to the pipeline of the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="This">is the <see cref="IApplicationBuilder"/> to add the middleware to</param>
        /// <returns>The modified <see cref="IApplicationBuilder"/></returns>
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder This)
            => This.UseMiddleware<ExceptionMiddleware>();
    }
}