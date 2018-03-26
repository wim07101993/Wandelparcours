using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebService.Services.Authorization;
using WebService.Services.Data;

namespace WebService.Controllers
{
    [Route("api/v1/[controller]")]
    public class TokensController : Controller
    {
        public const string CreateTokenTemplate = "";

        private readonly ITokenService _tokenService;
        private readonly IUsersService _usersService;

        public TokensController(ITokenService tokenService, IUsersService usersService)
        {
            _tokenService = tokenService;
            _usersService = usersService;
        }

        [Authorize]
        [HttpPost(CreateTokenTemplate)]
        public async Task<string> CreateTokenAsync([FromHeader] string userName, [FromHeader] string password)
        {
            var id = (ObjectId) await _usersService.GetPropertyByNameAsync(userName, x => x.Id);
            var token = await _tokenService.CreateTokenAsync(id, password);
            return token ?? throw new UnauthorizedAccessException();
        }
    }
}