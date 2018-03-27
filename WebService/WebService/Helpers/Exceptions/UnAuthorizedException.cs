using System;
using WebService.Helpers.Extensions;
using WebService.Models;

namespace WebService.Helpers.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException()
            : base("you need to have a token to access this method")
        {
        }

        public UnauthorizedException(string message)
            : base(message)
        {
        }

        public UnauthorizedException(params EUserType[] minAuthLevels)
            : base($"you need to ask access to one of the these people: {minAuthLevels.Serialize()}")
        {
            MinAuthLevels = minAuthLevels;
        }

        public EUserType[] MinAuthLevels { get; }
    }
}