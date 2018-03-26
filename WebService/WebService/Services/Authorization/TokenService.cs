using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using WebService.Helpers.Exceptions;
using WebService.Services.Data;
using WebService.Services.Logging;

namespace WebService.Services.Authorization
{
    public class TokenService : ITokenService
    {
        private const int TokenLifeTime = 30;

        private readonly IConfiguration _configuration;
        private readonly IUsersService _usersService;
        private readonly ILogger _logger;
        private readonly IDictionary<string, ObjectId> _issuedTokens = new Dictionary<string, ObjectId>();


        public TokenService(IConfiguration configuration, IUsersService usersService, ILogger logger)
        {
            _configuration = configuration;
            _usersService = usersService;
            _logger = logger;

            RunTokenGarbageCollectorAsync();
        }

        public IReadOnlyList<string> IssuedTokens => _issuedTokens.Keys.ToList();

        public bool IsTokenGarbageCollectorRunning { get; set; }


        public async Task<string> CreateTokenAsync(ObjectId id, string password)
        {
            if (!await _usersService.CheckCredentialsAsync(id, password))
                throw new WrongCredentialsException();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                expires: DateTime.Now.AddMinutes(TokenLifeTime),
                signingCredentials: creds
            ));

            _issuedTokens.Add(token, id);
            _logger.Log(this, ELogLevel.Information, $"Issued new token: {token}");
            return token;
        }

        public Task<ObjectId> GetIdFromToken(string token)
            => Task.FromResult(_issuedTokens[token]);

        public bool ValidateToken(string strToken)
        {
            if (!_issuedTokens.ContainsKey(strToken))
                return false;

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
                return false;
            }
        }

        private async void RunTokenGarbageCollectorAsync()
        {
            IsTokenGarbageCollectorRunning = true;

            while (IsTokenGarbageCollectorRunning)
            {
                await Task.Delay(TimeSpan.FromMinutes(TokenLifeTime));

                if (!_issuedTokens.Any())
                    continue;

                for (var i = IssuedTokens.Count - 1; i >= 0; i--)
                    if (!ValidateToken(IssuedTokens[i]))
                        _issuedTokens.Remove(IssuedTokens[i]);
            }

            IsTokenGarbageCollectorRunning = false;
        }
    }
}