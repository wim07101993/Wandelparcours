using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Models.Bases;

namespace WebService.Controllers.Bases
{
    /// <summary>
    /// IRestController defines the methods a REST controller should have
    /// </summary>
    /// <typeparam name="T"></typeparam>
    // ReSharper disable once TypeParameterCanBeVariant
    public interface IRestController<T> where T : IModelWithID
    {
        /// <summary>
        /// Get is supposed to return the Item with the given id in the database wrapped in an <see cref="IActionResult"/>. 
        /// To limit data traffic it is possible to select only a number of properties.
        /// </summary>
        /// <param name="id">is the id of the item to get</param>
        /// <param name="properties">are the properties to get from that item</param>
        /// <returns>
        /// The <see cref="T"/> that has the given id wrapped in:
        /// - Status ok (200) with An IEnumerable of all the Items in the database on success
        /// - Status bad request (400) when there are properties passed that do not exist in a <see cref="T"/>
        /// - Status not found (404) when there is no <see cref="T"/> with the given id found
        /// - Status internal server error (500) when an error occures
        /// </returns>
        Task<IActionResult> GetAsync(string id, [FromQuery] string[] properties);

        /// <summary>
        /// Get is supposed to return all the Items in the database wrapped in an <see cref="IActionResult"/>.
        /// To limit data traffic only the small properties should be returned by default but can be asked for. 
        /// </summary>
        /// <param name="properties">are the proeprties to include in the collection to return</param>
        /// <returns>
        /// - Status ok (200) with An IEnumerable of all the Items in the database on success
        /// - Status bad request (400) when there are properties passed that do not exist in a <see cref="T"/>
        /// - Status internal server (500) error when an error occures
        /// </returns>
        Task<IActionResult> GetAsync([FromQuery] string[] properties);

        /// <summary>
        /// Create is supposed to create a new ide for the item and save it to the database.
        /// </summary>
        /// <param name="item">is the <see cref="T"/> to save in the database</param>
        /// <returns>
        /// - Status created (201) if success
        /// - Status internal server error (500) on error or not created
        /// </returns>
        Task<IActionResult> CreateAsync([FromBody] T item);

        /// <summary>
        /// Delete is supposed to delete the passed <see cref="T"/> from the database.
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/> to remove from the database</param>
        /// <returns>
        /// - Status ok (200) if success
        /// - Status not found (404) if there is no item with the given id
        /// - Status internal server error (500) on error
        /// </returns>
        Task<IActionResult> DeleteAsync(string id);

        /// <summary>
        /// Update is supposed to update a specified set of properties of the <see cref="T"/>.
        /// If the item doesn't exist, a new is created in the database.
        /// </summary>
        /// <param name="item">is the <see cref="T"/> to update</param>
        /// <param name="properties">are the properties that should be updated</param>
        /// <returns>
        /// - Status ok (200) if the <see cref="T"/> was updated
        /// - Status created (201) if a new one was created
        /// - Status bad request (400) if the passed properties are not found on <see cref="T"/>
        /// - Status internal server error (500) on error or not created
        /// </returns>
        Task<IActionResult> UpdateAsync([FromBody] T item, [FromQuery] string[] properties);
    }
}