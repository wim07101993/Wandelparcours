using WebService.Models;
using WebService.Services.Data;

namespace WebService.Controllers.Bases
{
    public interface IUsersController : IDataService<User>
    {
    }
}