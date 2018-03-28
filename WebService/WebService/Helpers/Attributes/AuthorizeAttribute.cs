using System;
using WebService.Models;

namespace WebService.Helpers.Attributes
{
    public class AuthorizeAttribute : Attribute
    {
        public EUserType[] AllowedUsers { get; }

        public AuthorizeAttribute(params EUserType[] userTypes)
        {
            AllowedUsers = userTypes;
        }
    }
}