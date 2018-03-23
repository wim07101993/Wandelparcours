using System;
using WebService.Models;

namespace WebService.Helpers.Attributes
{
    public class AuthLevelAttribute : Attribute
    {
        public const EAuthLevel Default = EAuthLevel.User;

        public EAuthLevel AuthLevel { get; set; }
        
        public AuthLevelAttribute(EAuthLevel authLevel)
        {
            AuthLevel = authLevel;
        }
    }
}