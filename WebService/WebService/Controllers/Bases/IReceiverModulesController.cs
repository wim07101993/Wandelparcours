using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Models;

namespace WebService.Controllers.Bases
{
    /// <summary>
    /// An interface that extends from the <see cref="IRestController{T}"/> interface with as generic type parameter
    /// <see cref="ReceiverModule"/>.
    /// It is used to do the basic CRUD operations for the receiver modules.
    /// </summary>
    public interface IReceiverModulesController : IRestController<ReceiverModule>
    {
        /// <summary>
        /// Fetches the asked receiver module from the database.
        /// </summary>
        /// <param name="name">The name of the module</param>
        /// <param name="propertiesToInclude">The properties that should be passed with the result</param>
        /// <returns>The receiver module with the asked name</returns>
        Task<ReceiverModule> GetOneByNameAsync(string name, [FromQuery] string[] propertiesToInclude);
        
        /// <summary>
        /// Deletes the asked receiver module from the database.
        /// </summary>
        /// <param name="name">The name of the module</param>
        /// <returns></returns>
        Task DeleteByNameAsync(string name);
    }
}