using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Controllers.Bases;
using WebService.Helpers.Attributes;
using WebService.Models;
using WebService.Services.Authorization;
using WebService.Services.Data;

namespace WebService.Controllers
{
    [Route(Routes.RestBase.ControllerRoute)]
    public class TokensController : AControllerBase, ITokenController
    {
        private readonly ITokensService _tokensService;
        private readonly IUsersService _usersService;


        public TokensController(ITokensService tokensService, IUsersService usersService) : base(usersService)
        {
            _tokensService = tokensService;
            _usersService = usersService;
        }


        [Authorize]
        [HttpPost(Routes.Tokens.CreateTokenTemplate)]
        public async Task<object> CreateTokenAsync([FromHeader] string userName, [FromHeader] string password)
        {
            var user = await _usersService.GetByNameAsync(
                userName, new Expression<Func<User, object>>[]
                {
                    x => x.Email,
                    x => x.Group,
                    x => x.UserName,
                    x => x.UserType,
                    x => x.Id
                });

            var token = await _tokensService.CreateTokenAsync(user.Id, password)
                        ?? throw new UnauthorizedAccessException();

            return new
            {
                Token = token,
                User = user
            };
        }
    }
}