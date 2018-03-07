using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Models;
using WebService.Models.Bases;

namespace WebService.Controllers.Bases
{
    // ReSharper disable once TypeParameterCanBeVariant
    public interface IRestController<T> where T : IModelWithID
    {
        /// <summary>
        /// SmallDataProperties are supposed to be a collection of expressions to select the properties that
        /// consume the least space.
        /// </summary>
        IEnumerable<Expression<Func<T, object>>> PropertiesToSendOnGet { get; }

        /// <summary>
        /// Get is supposed to correspond to the GET method of the controller of the REST service.
        /// <para/>
        /// It returns the Item with the given id in the database wrapped in an <see cref="IActionResult"/>. To limit data traffic it is possible to
        /// select only a number of properties by default. These properties are selected with the <see cref="properties"/> property.
        /// </summary>
        /// <returns>
        /// - Status ok (200) with An IEnumerable of all the Items in the database on success
        /// - Status bad request (400) when there are properties passed that do not exist in a <see cref="T"/>
        /// - Status not found (404) when there is no <see cref="T"/> with the given id found
        /// - Status internal server error (500) when an error occures
        /// </returns>
        Task<IActionResult> GetAsync(string id, [FromQuery] string[] properties);

        /// <summary>
        /// Get is supposed to correspond to the GET method of the controller of the REST service.
        /// <para/>
        /// It returns all the Items in the database wrapped in an <see cref="IActionResult"/>. To limit data traffic it is possible to
        /// select only a number of properties by default. These properties are selected with the <see cref="PropertiesToSendOnGet"/> property.
        /// </summary>
        /// <param name="properties">are the proeprties to include in the collection to return</param>
        /// <returns>
        /// - Status ok (200) with An IEnumerable of all the Items in the database on success
        /// - Status internal server (500) error when an error occures
        /// </returns>
        Task<IActionResult> GetAsync([FromQuery] string[] properties);

        /// <summary>
        /// Create is supposed to correspond to the POST method of the controller of the REST service.
        /// <para/>
        /// It saves the passed <see cref="T"/> to the database.
        /// </summary>
        /// <param name="item">is the <see cref="T"/> to save in the database</param>
        /// <returns>
        /// - Status created (201) if succes
        /// - Status internal server error (500) on error or not created
        /// </returns>
        Task<IActionResult> CreateAsync([FromBody] T item);

        /// <summary>
        /// Delete is supposed to correspond to the DELETE method of the controller of the REST service.
        /// <para/>
        /// It saves the passed <see cref="T"/> to the database.
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/> to remove from the database</param>
        /// <returns>
        /// - Status created (201) if succes
        /// - Status not found (40) if there was no erro but also no object to remove
        /// - Status internal server error (500) on error
        /// </returns>
        Task<IActionResult> DeleteAsync(string id);

        /// <summary>
        /// Update is supposed to correspond to the PUT method of the controller of the REST service.
        /// <para/>
        /// It updates the fields of the <see cref="T"/> in the updater.
        /// If the Item doesn't exist, a new is created in the database.
        /// </summary>
        /// <param name="item">is the <see cref="T"/> to update</param>
        /// <param name="properties">contains the properties that should be updated</param>
        /// <returns>
        /// - Status ok (200) if the <see cref="T"/> was updated
        /// - Status created (201) if a new one was created
        /// - Status bad request (400) if the passed properties are not found on <see cref="T"/>
        /// - Status internal server error (500) on error or not created
        /// </returns>
        Task<IActionResult> UpdateAsync([FromBody] T item, [FromQuery] string[] properties);
    }
}