using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Models;

namespace WebService.Controllers.Bases
{
    /// <inheritdoc cref="IRestController{T}" />
    /// <summary>
    /// IReceiverModulesController defines the methods for the <see cref="ReceiverModule"/> REST controller.
    /// </summary>
    public interface IReceiverModulesController : IRestController<ReceiverModule>
    {
        Task<ReceiverModule> GetOneByMacAsync(string mac, [FromQuery] string[] propertiesToInclude);
        Task DeleteByMacAsync(string mac);
    }
}