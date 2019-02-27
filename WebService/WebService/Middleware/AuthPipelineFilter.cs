using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Bson;
using WebService.Controllers;
using WebService.Controllers.Bases;
using WebService.Helpers.Attributes;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Authorization;
using WebService.Services.Data;

namespace WebService.Middleware
{
    public class AuthPipelineFilter : IAsyncActionFilter
    {
        private readonly ITokensService _tokensService;
        private readonly IUsersService _usersService;


        public AuthPipelineFilter(ITokensService tokensService, IUsersService usersService)
        {
            _tokensService = tokensService;
            _usersService = usersService;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // uncomment to disable auth
            //await next();
            //return;

            if (!(context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor))
                throw new UnauthorizedException();

            var method = controllerActionDescriptor.MethodInfo;
            if (context.Controller is TokensController && method.Name == nameof(TokensController.CreateTokenAsync))
            {
                await next();
                return;
            }

            var allowedUserTypes = method
                .GetCustomAttributes()
                .Where(x => x is AuthorizeAttribute)
                .Select(x => ((AuthorizeAttribute) x).AllowedUsers)
                .FirstOrDefault();

            if (EnumerableExtensions.IsNullOrEmpty(allowedUserTypes))
            {
                await next();
                return;
            }

            var headers = context.HttpContext.Request.Headers;
            var query = context.HttpContext.Request.Query;

            var headerContainsToken = headers.ContainsKey("token");
            if (!headerContainsToken && !query.ContainsKey("token"))
                throw new UnauthorizedException();

            string strToken = headerContainsToken
                ? headers["token"]
                : query["token"];
            
            if (!_tokensService.ValidateToken(strToken))
                throw new UnauthorizedException();

            var userId = await _tokensService.GetIdFromToken(strToken);
            if (userId == ObjectId.Empty)
                throw new UnauthorizedException(allowedUserTypes);

            var userType = await _usersService.GetPropertyAsync(userId, x => x.UserType);

            // ReSharper disable once AssignNullToNotNullAttribute
            if (userType != EUserType.SysAdmin && !allowedUserTypes.Contains(userType))
                throw new UnauthorizedException(allowedUserTypes);

            if (context.Controller is IController controller)
                controller.CurrentUserId = userId;

            await next();
        }
    }
}