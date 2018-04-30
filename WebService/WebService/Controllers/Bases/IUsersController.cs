using System.Threading.Tasks;
using WebService.Models;

namespace WebService.Controllers.Bases
{
    public interface IUsersController : IRestController<User>
    {
        Task<User> GetByNameAsync(string userName, string[] propertiesToInclude);

        Task<object> GetPropertyByNameAsync(string userName, string propertyName);
    }
}