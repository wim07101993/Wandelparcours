using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Models;

namespace WebService.Services.Data
{
    public interface IUsersService : IDataService<User>
    {
        Task<bool> CheckCredentialsAsync(string userName, string password);
        Task TaskUpdatePasswordAsync(ObjectId id, string password);
    }
}