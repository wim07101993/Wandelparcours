using System.Threading.Tasks;

namespace WebService.Controllers.Bases
{
    public interface ITokenController: IController
    {
        Task<string> CreateTokenAsync(string userName, string password);
    }
}