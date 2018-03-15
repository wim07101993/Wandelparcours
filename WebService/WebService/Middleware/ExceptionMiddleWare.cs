using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebService.Helpers.Exceptions;
using WebService.Services.Logging;

namespace WebService.Middleware
{
    /// <summary>
    /// ExceptionMiddleware is a middleware that handles all exceptions thrown in the Controllers.
    /// </summary>
    public class ExceptionMiddleware
    {
        #region FIELDS

        /// <summary>
        /// _next is the next <see cref="RequestDelegate"/> in the pipeline.
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// _logger is an instance of the <see cref="ILogger"/> interface to log messages.
        /// </summary>
        private readonly ILogger _logger;

        #endregion FIELDS


        #region CONSTRUCTOR

        /// <summary>
        /// ExceptionMiddleware instantiates a new object of the <see cref="ExceptionMiddleware"/> class.
        /// </summary>
        /// <param name="next">is the next <see cref="RequestDelegate"/> in the pipeline</param>
        /// <param name="logger">is an instance of the <see cref="ILogger"/> interface to log messages</param>
        public ExceptionMiddleware(RequestDelegate next, ILogger logger)
        {
            // inject dependencies
            _next = next;
            _logger = logger;
        }

        #endregion CONSTRUCTOR


        #region METHODS

        /// <summary>
        /// Invoke invokes the next item in the pipeline and handles all exceptions thrown.
        /// </summary>
        /// <param name="context">is the context containing all HTTP elements (response, request,...)</param>
        /// <returns>A task to execute</returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                // invoke the next element in the pipeline
                await _next.Invoke(context);
            }
            // check for bad arguments by the client
            catch (WebArgumentException e)
            {
                // respond with a 400 bad request status code
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;

                // respond a message
                await context.Response.WriteAsync("The arguments you passed just destroyed Luxembourg...\n\n");

                // respond the error
                await context.Response.WriteAsync(e.Message);
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

        # endregion METHODS
    }
}