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
using WebService.Services.Authorization;
using WebService.Services.Data;

namespace WebService.Middleware
{
    public class AuthPipelineFilter : IAsyncActionFilter
    {
        private readonly ITokenService _tokenService;
        private readonly IUsersService _usersService;

        public AuthPipelineFilter(ITokenService tokenService, IUsersService usersService)
        {
            _tokenService = tokenService;
            _usersService = usersService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!(context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor))
                throw new UnauthorizedException();

            var method = controllerActionDescriptor.MethodInfo;
            if (context.Controller is TokensController && method.Name == nameof(TokensController.CreateTokenAsync))
            {
                await next();
                return;
            }

            var authorizeAttribute = (AuthorizeAttribute) method
                .GetCustomAttributes()
                .FirstOrDefault(x => x is AuthorizeAttribute);

            if (authorizeAttribute == null)
                throw new NotFoundException();

            var allowedUserTypes = authorizeAttribute.AllowedUsers;

            if (EnumerableExtensions.IsNullOrEmpty(allowedUserTypes))
            {
                await next();
                return;
            }

            var headers = context.HttpContext.Request.Headers;
            if (!headers.ContainsKey("token"))
                throw new UnauthorizedException();

            var strToken = headers["token"];
            if (!_tokenService.ValidateToken(strToken))
                throw new UnauthorizedException();

            var userId = await _tokenService.GetIdFromToken(strToken);
            if (userId == ObjectId.Empty)
                throw new UnauthorizedException(allowedUserTypes);

            var userType = await _usersService.GetPropertyAsync(userId, x => x.UserType);

            // ReSharper disable once AssignNullToNotNullAttribute
            if (!allowedUserTypes.Contains(userType))
                throw new UnauthorizedException(allowedUserTypes);

            if (context.Controller is IController controller)
                controller.UserId = userId;

            await next();
        }
    }
}