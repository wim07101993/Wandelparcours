using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using WebService.Controllers;
using WebService.Helpers.Attributes;
using WebService.Helpers.Extensions;
using WebService.Models;
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
            {
                var autLevels = GetAuthLevel(requestUrl);

                var userId = await _tokenService.GetIdFromToken(header["token"]);
                var user = await _usersService.GetOneAsync(userId,
                    new Expression<Func<User, object>>[] {x => x.AuthLevel});

                if (autLevels.Any(x => x == user.AuthLevel))
                    await _next.Invoke(context);
                else
                _iThrow.Unauthorized(autLevels);
            }
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

        private static EAuthLevel[] GetAuthLevel(string url)
        {
            var controllerType = GetControllerCorrespondingToUrl(url);

            return (GetMethodCorrespondingToUrl(controllerType, url)
                    ?.GetCustomAttributes()
                    .Where(x => x is AuthLevelAttribute) as AuthLevelAttribute)
                ?.AuthLevels;
        }

        private static Type GetControllerCorrespondingToUrl(string url)
            => Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .FirstOrDefault(x =>
                {
                    if (!typeof(Controller).IsAssignableFrom(x) ||
                        !x.IsPublic ||
                        x.IsAbstract ||
                        x.IsInterface)
                        return false;

                    var controllerUrl = GetControllerUrl(x);

                    if (controllerUrl == null || controllerUrl.Length >= url.Length)
                        return false;

                    var urlPart = url
                        .Substring(1, controllerUrl.Length);

                    return urlPart
                        .Equals(controllerUrl, StringComparison.InvariantCultureIgnoreCase);
                });

        private static MethodInfo GetMethodCorrespondingToUrl(Type controllerType, string url)
        {
            var controllerUrl = GetControllerUrl(controllerType);
            return controllerType.GetMethods().FirstOrDefault(method =>
            {
                if (!(method
                    .GetCustomAttributes()
                    .FirstOrDefault(attr => attr is HttpMethodAttribute) is HttpMethodAttribute httpMethod))
                    return false;

                // controller/method/{param1}/text/{param2}
                // { "controller/method/" , "param1", "/text/", "param2" }
                var templateSplit = httpMethod.Template.Split('{', '}');

                // pattern = controller/method/
                var pattern = new StringBuilder($"^/{templateSplit[0]}");

                for (var i = 1; i < templateSplit.Length; i += 2)
                {
                    pattern.Append(@"\w*");
                    pattern.Append(templateSplit[i + 1]);
                }

                // add potential query parameters
                pattern.Append(@"[?](\w*=\w*&)*\w*=\w*$");

                var regex = new Regex(pattern.ToString());

                return regex.IsMatch(url);
            });
        }

        # endregion METHODS
    }
}