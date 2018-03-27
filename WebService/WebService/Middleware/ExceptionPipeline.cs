using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using WebService.Helpers.Exceptions;
using WebService.Services.Logging;

namespace WebService.Middleware
{
    public class ExceptionPipeline : IAsyncActionFilter
    {
        #region FIELDS

        /// <summary>
        /// _logger is an instance of the <see cref="ILogger"/> interface to log messages.
        /// </summary>
        private readonly ILogger _logger;

        #endregion FIELDS


        #region CONSTRUCTOR

        /// <summary>
        /// ExceptionMiddleware instantiates a new object of the <see cref="ExceptionPipeline"/> class.
        /// </summary>
        /// <param name="logger">is an instance of the <see cref="ILogger"/> interface to log messages</param>
        public ExceptionPipeline(ILogger logger)
        {
            _logger = logger;
        }

        #endregion CONSTRUCTOR


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                // invoke the next element in the pipeline
                await next();

                if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    var attributs = controllerActionDescriptor.MethodInfo.GetCustomAttributes();
                    if (attributs.Any(x => x is HttpPostAttribute))
                        context.HttpContext.Response.StatusCode = (int) HttpStatusCode.Created;
                }
            }
            // check for bad arguments by the client
            catch (Helpers.Exceptions.ArgumentException e)
            {
                // respond with a 400 bad request status code
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;

                // respond a message
                await context.HttpContext.Response.WriteAsync(
                    "The arguments you passed just destroyed Luxembourg...\n\n");

                // respond the error
                await context.HttpContext.Response.WriteAsync((string) e.Message);
            }
            // check if some recourse is not found
            catch (NotFoundException e)
            {
                // respond with a 404 not found status code
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;

                // respond a message
                await context.HttpContext.Response.WriteAsync("These are not the droids you're looking for.\n\n");

                // respond the error
                await context.HttpContext.Response.WriteAsync(e.Message);
            }
            // check for unauthorized requests
            catch (UnauthorizedException e)
            {
                // respond with a 404 not found status code
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;

                // respond a message
                await context.HttpContext.Response.WriteAsync("You shall not pass!!\n\n");

                // respond the error
                await context.HttpContext.Response.WriteAsync(e.Message);

                _logger.Log(this, ELogLevel.Information, e);
            }
            // catch all other exceptions
            catch (Exception e)
            {
                // respond with  500 internal server error status code
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                // log the exception
                _logger.Log(this, ELogLevel.Error, e);

                // respond with fancy gif
                await context.HttpContext.Response.WriteAsync("http://gph.is/1s201Ez");
            }
        }
    }
}