using WebService.Models;
using WebService.Services.Randomizer;

namespace WebService.Helpers.Extensions
{
    public static class UserExtensions
    {
        public static User HashPassword(this User This, bool useSalt = true, bool usePepper = true)
        {
            string str;

            if (useSalt && usePepper)
                str = This.Password + This.UserName + This.Email + Randomizer.Instance.NextChar();
            else if (useSalt)
                str = This.Password + This.UserName + This.Email;
            else if (usePepper)
                str = This.Password + Randomizer.Instance.NextChar();
            else
                str = This.Password;

            var hash = BCrypt.Net.BCrypt.HashString(str);

            This.Password = hash;

            return This;
        }
    }
}