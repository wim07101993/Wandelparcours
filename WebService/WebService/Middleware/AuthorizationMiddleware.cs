using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebService.Controllers;
using WebService.Services.Authorization;
using WebService.Services.Logging;

namespace WebService.Middleware
{
    public class AuthorizationMiddleware
    {
        #region FIELDS

        private readonly RequestDelegate _next;
        private readonly ITokenService _tokenService;
        private readonly ILogger _logger;

        #endregion FIELDS


        #region CONSTRUCTOR

        public AuthorizationMiddleware(RequestDelegate next, ITokenService tokenService, ILogger logger)
        {
            _next = next;
            _tokenService = tokenService;
            _logger = logger;
        }

        #endregion CONSTRUCTOR


        #region METHODS

        public async Task Invoke(HttpContext context)
        {
            var tokenControllerName =
                nameof(TokenController)
                    .Substring(0, nameof(TokenController).Length - "Controller".Length);

            var tokenControllerUrl =
                (typeof(TokenController).GetCustomAttribute(typeof(RouteAttribute)) as RouteAttribute)
                ?.Template
                .Replace("[controller]", tokenControllerName)
                .Replace("[Controller]", tokenControllerName);

            var postUrl =
                (typeof(TokenController)
                    .GetMethod(nameof(TokenController.CreateToken))
                    .GetCustomAttribute(typeof(HttpPostAttribute)) as HttpPostAttribute)
                ?.Template
                ?? "";

            var createTokenUrl = $"/{tokenControllerUrl}/{postUrl}".ToLower();

            var requestUrl = context.Request.Path.ToString().ToLower();
            var header = context.Request.Headers;

            if (requestUrl == createTokenUrl || $"{requestUrl}/" == createTokenUrl ||
                header.ContainsKey("token") && _tokenService.ValidateToken(header["token"]))
                await _next.Invoke(context);
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                _logger.Log(this, ELogLevel.Information,
                    $"Unauthorized request from {context.Connection.RemoteIpAddress}:{context.Connection.RemotePort}");
            }
        }

        # endregion METHODS
    }
}
