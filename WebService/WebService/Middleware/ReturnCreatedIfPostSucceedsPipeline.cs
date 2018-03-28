using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using WebService.Services.Logging;

namespace WebService.Middleware
{
    public class ReturnCreatedIfPostSucceedsPipeline : IAsyncActionFilter
    {
        #region FIELDS

        /// <summary>
        /// _logger is an instance of the <see cref="ILogger"/> interface to log messages.
        /// </summary>
        private readonly ILogger _logger;

        #endregion FIELDS


        #region CONSTRUCTOR

        /// <summary>
        /// ExceptionMiddleware instantiates a new object of the <see cref="ReturnCreatedIfPostSucceedsPipeline"/> class.
        /// </summary>
        /// <param name="logger">is an instance of the <see cref="ILogger"/> interface to log messages</param>
        public ReturnCreatedIfPostSucceedsPipeline(ILogger logger)
        {
            _logger = logger;
        }

        #endregion CONSTRUCTOR


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
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
    }
}