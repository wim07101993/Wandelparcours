using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using WebService.Helpers.Attributes;
using WebService.Services.Authorization;
using WebService.Services.Data;
using WebService.Services.Exceptions;

namespace WebService.Middleware
{
    public class AuthPipelineFilter : IAsyncActionFilter
    {
        private readonly IThrow _iThrow;
        private readonly ITokenService _tokenService;
        private readonly IUsersService _usersService;

        public AuthPipelineFilter(IThrow iThrow, ITokenService tokenService, IUsersService usersService)
        {
            _iThrow = iThrow;
            _tokenService = tokenService;
            _usersService = usersService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                var allowedUserTypes = ((CanAccessAttribute) controllerActionDescriptor
                        .MethodInfo
                        .GetCustomAttributes()
                        .FirstOrDefault(x => x is CanAccessAttribute))
                    ?.AllowedUsers;

                if (allowedUserTypes != null)
                {
                    var headers = context.HttpContext.Request.Headers;
                    if (!headers.ContainsKey("token"))
                        _iThrow.Unauthorized(allowedUserTypes);

                    var strToken = headers["token"];
                    if (!_tokenService.ValidateToken(strToken))
                        _iThrow.Unauthorized(allowedUserTypes);

                    var userId = await _tokenService.GetIdFromToken(strToken);
                    var userType = await _usersService.GetPropertyAsync(userId, x => x.UserType);

                    if (!allowedUserTypes.Contains(userType))
                        _iThrow.Unauthorized(allowedUserTypes);
                }
            }

            await next();
        }
    }
}