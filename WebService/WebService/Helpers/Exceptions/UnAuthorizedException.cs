using System;
using WebService.Models;

namespace WebService.Helpers.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException()
        {
        }

        public UnauthorizedException(string message) : base(message)
        {
        }

        public UnauthorizedException(string message, EAuthLevel minAuthLevel) : base(message)
        {
            MinAuthLevel = minAuthLevel;
        }

        public EAuthLevel MinAuthLevel { get; }
    }
}