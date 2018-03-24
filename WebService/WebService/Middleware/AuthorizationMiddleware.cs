using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebService.Controllers;
using WebService.Services.Authorization;
using WebService.Services.Data;
using WebService.Services.Exceptions;
using WebService.Services.Logging;

namespace WebService.Middleware
{
    public class AuthorizationMiddleware
    {
        #region FIELDS

        private readonly RequestDelegate _next;
        private readonly ITokenService _tokenService;
        private readonly ILogger _logger;
        private readonly IUsersService _usersService;
        private readonly IThrow _iThrow;

        #endregion FIELDS


        #region CONSTRUCTOR

        public AuthorizationMiddleware(RequestDelegate next, ITokenService tokenService, ILogger logger,
            IUsersService usersService, IThrow iThrow)
        {
            _next = next;
            _tokenService = tokenService;
            _logger = logger;
            _usersService = usersService;
            _iThrow = iThrow;
        }

        #endregion CONSTRUCTOR


        #region METHODS

        public async Task Invoke(HttpContext context)
        {
            var tokenControllerUrl = GetControllerUrl(typeof(TokensController));

            var createTokenUrl = $"/{tokenControllerUrl}/{TokensController.CreateTokenTemplate}".ToLower();

            var requestUrl = context.Request.Path.ToString().ToLower();
            var header = context.Request.Headers;

            if (requestUrl == createTokenUrl || $"{requestUrl}/" == createTokenUrl)
                await _next.Invoke(context);
            else if (header.ContainsKey("token") && _tokenService.ValidateToken(header["token"]))
                await _next.Invoke(context);
            else
                _iThrow.Unauthorized();
        }

        private static string GetControllerUrl(Type controllerType)
        {
            var completeControllerName = controllerType.Name;

            const string controllerString = "Controller";
            var normalNameLength = completeControllerName.Length - controllerString.Length;

            var controllerName = completeControllerName.Substring(normalNameLength) == controllerString
                ? completeControllerName.Substring(0, normalNameLength)
                : completeControllerName;

            return (controllerType
                    .GetCustomAttributes()
                    .FirstOrDefault(x => x is RouteAttribute) as RouteAttribute)?
                .Template
                .Replace("[Controller]", controllerName, StringComparison.InvariantCultureIgnoreCase);
        }
        
        # endregion METHODS
    }
}