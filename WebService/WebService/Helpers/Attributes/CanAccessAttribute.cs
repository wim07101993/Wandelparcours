using System;
using WebService.Models;

namespace WebService.Helpers.Attributes
{
    public class CanAccessAttribute : Attribute
    {
        public EUserType[] AllowedUsers { get; }

        public CanAccessAttribute(params EUserType[] userTypes)
        {
            AllowedUsers = userTypes;
        }
    }
}