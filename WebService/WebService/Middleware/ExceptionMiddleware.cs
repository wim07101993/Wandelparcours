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
            // check for bad arguments by the client
            catch (Helpers.Exceptions.ArgumentException e)
            {
                // respond with a 400 bad request status code
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;

                // respond a message
                await context.Response.WriteAsync(
                    "The arguments you passed just destroyed Luxembourg...\n\n");

                // respond the error
                await context.Response.WriteAsync((string) e.Message);
            }
            // check if some recourse is not found
            catch (NotFoundException e)
            {
                // respond with a 404 not found status code
                context.Response.StatusCode = (int) HttpStatusCode.NotFound;

                // respond a message
                await context.Response.WriteAsync("These are not the droids you're looking for.\n\n");

                // respond the error
                await context.Response.WriteAsync(e.Message);
            }
            // check for unauthorized requests
            catch (UnauthorizedException e)
            {
                // respond with a 404 not found status code
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;

                // respond a message
                await context.Response.WriteAsync("You shall not pass!!\n\n");

                // respond the error
                await context.Response.WriteAsync(e.Message);

                _logger.Log(this, ELogLevel.Information, e);
            }
            // catch all other exceptions
            catch (Exception e)
            {
                // respond with  500 internal server error status code
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                // log the exception
                _logger.Log(this, ELogLevel.Error, e);

                // respond with fancy gif
                await context.Response.WriteAsync("http://gph.is/1s201Ez");
            }
        }
    }
}