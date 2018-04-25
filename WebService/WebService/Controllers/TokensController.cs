﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Controllers.Bases;
using WebService.Helpers.Attributes;
using WebService.Services.Authorization;
using WebService.Services.Data;

namespace WebService.Controllers
{
    [Route(Routes.RestBase.ControllerRoute)]
    public class TokensController : AControllerBase, ITokenController
    {
        private readonly ITokenService _tokenService;
        private readonly IUsersService _usersService;


        public TokensController(ITokenService tokenService, IUsersService usersService) : base(usersService)
        {
            _tokenService = tokenService;
            _usersService = usersService;
        }


        [Authorize]
        [HttpPost(Routes.Tokens.CreateTokenTemplate)]
        public async Task<string> CreateTokenAsync([FromHeader] string userName, [FromHeader] string password)
        {
            var id = await _usersService.GetPropertyByNameAsync(userName, x => x.Id);
            return await _tokenService.CreateTokenAsync(id, password)
                   ?? throw new UnauthorizedAccessException();
        }
    }
}