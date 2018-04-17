using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Controllers.Bases;
using WebService.Helpers.Attributes;
using WebService.Services.Authorization;
using WebService.Services.Data;

namespace WebService.Controllers
{
    [Route("api/v1/[controller]")]
    public class TokensController : AControllerBase, ITokenController
    {
        private const string CreateTokenTemplate = "";

        private readonly ITokenService _tokenService;
        private readonly IUsersService _usersService;


        public TokensController(ITokenService tokenService, IUsersService usersService) : base(usersService)
        {
            _tokenService = tokenService;
            _usersService = usersService;
        }


        [Authorize]
        [HttpPost(CreateTokenTemplate)]
        public async Task<string> CreateTokenAsync([FromHeader] string userName, [FromHeader] string password)
        {
            var id = await _usersService.GetPropertyByNameAsync(userName, x => x.Id);
            var token = await _tokenService.CreateTokenAsync(id, password);
            return token ?? throw new UnauthorizedAccessException();
        }
    }
}