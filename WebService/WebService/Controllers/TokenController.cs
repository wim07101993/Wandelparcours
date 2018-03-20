using System;
using Microsoft.AspNetCore.Mvc;
using WebService.Services.Authorization;

namespace WebService.Controllers
{
    public class TokenController : Controller
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        public string CreateToken([FromHeader] string userName, [FromHeader] string password)
        {
            var token = _tokenService.CreateToken(userName, password);
            return token ?? throw new UnauthorizedAccessException();
        }
    }
}