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
        /// <summary>
        /// GetAsync is supposed to return the <see cref="ReceiverModule"/> that has the given MAC-address, wrapped in an <see cref="IActionResult"/>.
        /// </summary>
        /// <param name="mac">is the MAC-address of the <see cref="ReceiverModule"/> to return</param>
        /// <returns>
        /// The <see cref="ReceiverModule"/> that has the given MAC-address wrapped in:
        /// - Status ok (200) with An IEnumerable of all the Items in the database on success
        /// - Status bad request (400) when there are properties passed that do not exist in a <see cref="ReceiverModule"/>
        /// - Status not found (404) when there is no <see cref="ReceiverModule"/> with the given id found
        /// - Status internal server error (500) when an error occures
        /// </returns>
        Task<IActionResult> GetAsync(string mac);
    }
}