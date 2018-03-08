using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Helpers.Exceptions;
using WebService.Models.Bases;

namespace WebService.Controllers.Bases
{
    /// <summary>
    /// IRestController defines the methods a REST controller should have
    /// </summary>
    /// <typeparam name="T">is the type of the data to handle. It should be assignable to an <see cref="IModelWithID"/></typeparam>
    public interface IRestController<T> where T : IModelWithID
    {
        /// <summary>
        /// Get is supposed to return the <see cref="T"/> with the given id in the database. 
        /// To limit data traffic it is possible to select only a number of properties
        /// </summary>
        /// <returns>The <see cref="T"/> in the database that has the given id</returns>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="T"/> not found</exception>
        /// <exception cref="WebArgumentException">When the properties could not be converted to selectors</exception>
        Task<T> GetAsync(string id, [FromQuery] string[] properties);

        /// <summary>
        /// Get is supposed to return all the Items in the database wrapped in an <see cref="IActionResult"/>. 
        /// To limit data traffic it is possible to select only a number of properties by default.
        /// </summary>
        /// <returns>All <see cref="T"/>s in the database but only the given properties are filled in</returns>
        /// <exception cref="WebArgumentException">When the properties could not be converted to selectors</exception>
        Task<IEnumerable<T>> GetAsync([FromQuery] string[] properties);

        /// <summary>
        /// Create is supposed to save the passed <see cref="T"/> to the database.
        /// </summary>
        /// <param name="item">is the <see cref="T"/> to save in the database</param>
        Task CreateAsync([FromBody] T item);

        /// <summary>
        /// Delete is supposed to remove the <see cref="T"/> with the passed id from the database.
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/> to remove from the database</param>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="T"/> not found</exception>
        Task DeleteAsync(string id);

        /// <summary>
        /// Update updates the fields of the <see cref="T"/> that are specified in the <see cref="properties"/> parameter.
        /// If the item doesn't exist, a new is created in the database.
        /// </summary>
        /// <param name="item">is the <see cref="T"/> to update</param>
        /// <param name="properties">contains the properties that should be updated</param>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="T"/> not found</exception>
        /// <exception cref="WebArgumentException">When the properties could not be converted to selectors</exception>
        Task UpdateAsync([FromBody] T item, [FromQuery] string[] properties);
    }
}