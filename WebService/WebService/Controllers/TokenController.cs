using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Services.Authorization;

namespace WebService.Controllers
{
    public class TokenController : Controller
    {
        public const string CreateTokenTemplate = "";

        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost(CreateTokenTemplate)]
        public async Task<string> CreateTokenAsync([FromHeader] string userName, [FromHeader] string password)
        {
            var token = await _tokenService.CreateTokenAsync(userName, password);
            return token ?? throw new UnauthorizedAccessException();
        }
    }
}