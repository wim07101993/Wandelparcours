namespace WebService.Services.Authorization
{
    public interface ITokenService
    {
        string CreateToken(string userName, string password);
        bool ValidateToken(string strToken);
    }
}