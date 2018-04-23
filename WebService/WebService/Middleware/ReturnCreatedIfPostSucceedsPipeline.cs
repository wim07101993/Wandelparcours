using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebService.Middleware
{
    public class ReturnCreatedIfPostSucceedsPipeline : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
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