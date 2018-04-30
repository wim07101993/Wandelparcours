using System.Threading.Tasks;

namespace WebService.Controllers.Bases
{
    public interface ITokenController : IController
    {
        Task<object> CreateTokenAsync(string userName, string password);
    }
}