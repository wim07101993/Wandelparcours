using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Models;
using WebService.Services.Data;

namespace WebService.Controllers.Bases
{
    public interface IReceiverModulesController : IRestController<ReceiverModule>
    {
        Task<IActionResult> GetAsync(string mac);
    }
}
