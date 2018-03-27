using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace WebService.Helpers.Extensions
{
    public static class ControllerExtensions
    {
        public static string GetControllerUrl<T>() where T : Controller
        {
            var completeControllerName = typeof(T).Name;

            const string controllerString = "Controller";
            var normalNameLength = completeControllerName.Length - controllerString.Length;

            var controllerName = completeControllerName.Substring(normalNameLength) == controllerString
                ? completeControllerName.Substring(0, normalNameLength)
                : completeControllerName;

            var attribute = typeof(T)
                .GetCustomAttributes()
                .FirstOrDefault(x => x is RouteAttribute) as RouteAttribute;

            var ip = IPAddressHelper.GetIPAddress();
            if (string.IsNullOrWhiteSpace(attribute?.Template))
                return $"{ip}/{controllerName}";

            var urlSuffix = attribute.Template
                .Replace("[Controller]", controllerName, StringComparison.InvariantCultureIgnoreCase);

            return $"{ip}/{urlSuffix}";
        }

        public static string GetUrl<T>(this Controller This) where T : Controller
            => GetControllerUrl<T>();

        public static string GetUrl<TParent>(this MethodInfo This, IDictionary<string, string> parameters = null)
            where TParent : Controller
        {
            if (!(This
                .GetCustomAttributes()
                .FirstOrDefault(x => x is HttpMethodAttribute) is HttpMethodAttribute attribute))
                return $"{GetControllerUrl<TParent>()}/{This.Name}";

            var urlSuffix = attribute.Template;

            if (parameters != null)
                urlSuffix = parameters.Aggregate(
                    urlSuffix,
                    (current, parameter) => current
                        .Replace("{" + parameter.Key + "}", parameter.Value));

            return $"{GetControllerUrl<TParent>()}/{urlSuffix}";
        }
    }
}