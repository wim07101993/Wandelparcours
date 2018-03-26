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

        public UnauthorizedException(string message, EUserType[] minAuthLevels) : base(message)
        {
            MinAuthLevels = minAuthLevels;
        }

        public EUserType[] MinAuthLevels { get; }
    }
}