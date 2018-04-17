using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebService.Helpers.Exceptions;
using WebService.Services.Logging;

namespace WebService.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Helpers.Exceptions.ArgumentException e)
            {
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;

                await context.Response.WriteAsync("The arguments you passed just destroyed Luxembourg...\n\n");
                await context.Response.WriteAsync(e.Message);
            }
            catch (NotFoundException e)
            {
                context.Response.StatusCode = (int) HttpStatusCode.NotFound;

                await context.Response.WriteAsync("These are not the droids you're looking for.\n\n");
                await context.Response.WriteAsync(e.Message);
            }
            catch (UnauthorizedException e)
            {
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;

                await context.Response.WriteAsync("You shall not pass!!\n\n");
                await context.Response.WriteAsync(e.Message);

                _logger.Log(this, ELogLevel.Information, e);
            }
            catch (Exception e)
            {
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                await context.Response.WriteAsync("<img src=\"http://gph.is/1s201Ez\">");

                _logger.Log(this, ELogLevel.Error, e);
            }
        }
    }
}