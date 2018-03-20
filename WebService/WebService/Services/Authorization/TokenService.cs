using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebService.Models;
using WebService.Services.Data;
using WebService.Services.Exceptions;
using WebService.Services.Logging;

namespace WebService.Services.Authorization
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IUsersService _usersService;
        private readonly IThrow _iThrow;
        private readonly ILogger _logger;

        public TokenService(IConfiguration configuration, IUsersService usersService, IThrow iThrow,
            ILogger logger)
        {
            _configuration = configuration;
            _usersService = usersService;
            _iThrow = iThrow;
            _logger = logger;
        }


        public async Task<string> CreateTokenAsync(string userName, string password)
        {
            if (!(await _usersService.CheckCredentialsAsync(userName, password)))
                _iThrow.NotFound<User>(nameof(userName));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            ));

            _logger.Log(this, ELogLevel.Information, $"Issued new token: {token}");
            return token;
        }

        public bool ValidateToken(string strToken)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var tokenParams = new TokenValidationParameters
            {
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Issuer"],
                IssuerSigningKey = key,
            };

            try
            {
                new JwtSecurityTokenHandler().ValidateToken(strToken, tokenParams, out var token);
                return token.ValidTo < DateTime.Now;
            }
            catch
            {
                // IGNORED
            }

            return false;
        }
    }
}