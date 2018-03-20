using WebService.Models;

namespace WebService.Services.Data
{
    public interface IUsersService : IDataService<User>
    {
        bool CheckCredentials(string userName, string password);
    }
}