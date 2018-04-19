using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Models;

namespace WebService.Controllers.Bases
{
    public interface IReceiverModulesController : IRestController<ReceiverModule>
    {
        Task<ReceiverModule> GetOneByMacAsync(string mac, [FromQuery] string[] propertiesToInclude);
        Task DeleteByMacAsync(string mac);
    }
}