using System.Threading.Tasks;

namespace WebService.Services.Authorization
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(string userName, string password);
        bool ValidateToken(string strToken);
    }
}