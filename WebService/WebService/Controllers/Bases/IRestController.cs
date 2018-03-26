using System;
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
        #region CREATE

        /// <summary>
        /// Create is supposed to save the passed <see cref="T"/> to the database.
        /// </summary>
        /// <param name="item">is the <see cref="T"/> to save in the database</param>
        /// <exception cref="Exception">When the item could not be created</exception>
        Task<StatusCodeResult> CreateAsync([FromBody] T item);

        Task<StatusCodeResult> AddItemToListAsync(string id, string property, string jsonValue);

        #endregion CREATE


        #region READ

        /// <summary>
        /// Get is supposed to return all the Items in the database. 
        /// To limit data traffic it is possible to select only a number of properties.
        /// </summary>
        /// <param name="propertiesToInclude">are the properties of which the values should be returned</param>
        /// <returns>All <see cref="T"/>s in the database but only the given properties are filled in</returns>
        /// <exception cref="ArgumentException">When one ore more properties could not be converted to selectors</exception>
        Task<IEnumerable<T>> GetAllAsync([FromQuery] string[] propertiesToInclude);

        /// <summary>
        /// Get is supposed to return the <see cref="T"/> with the given id in the database. 
        /// To limit data traffic it is possible to select only a number of properties.
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/> to get</param>
        /// <param name="propertiesToInclude">are the properties of which the values should be returned</param>
        /// <returns>The <see cref="T"/> in the database that has the given id</returns>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="T"/> not found</exception>
        /// <exception cref="ArgumentException">When one ore more properties could not be converted to selectors</exception>
        Task<T> GetOneAsync(string id, [FromQuery] string[] propertiesToInclude);

        /// <summary>
        /// GetProperty is supposed to return the jsonValue of the asked property of the asked <see cref="T"/>.
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/></param>
        /// <param name="propertyName">is the name of the property to return</param>
        /// <returns>The jsonValue of the asked property</returns>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="T"/> not found</exception>
        /// <exception cref="ArgumentException">When the property could not be found on <see cref="T"/></exception>
        Task<object> GetPropertyAsync(string id, string propertyName);

        #endregion READ


        #region UPDATE

        /// <summary>
        /// Update updates the fields of the <see cref="T"/> that are specified in the <see cref="propertiesToUpdate"/> parameter.
        /// If the item doesn't exist, a new is created in the database.
        /// </summary>
        /// <param name="item">is the <see cref="T"/> to update</param>
        /// <param name="propertiesToUpdate">contains the properties that should be updated</param>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="T"/> not found</exception>
        /// <exception cref="ArgumentException">When one ore more properties could not be converted to selectors</exception>
        Task UpdateAsync([FromBody] T item, [FromQuery] string[] propertiesToUpdate);


        /// <summary>
        /// UpdatePropertyAsync is supposed to update the jsonValue of the asked property of the asked <see cref="T"/>.
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/></param>
        /// <param name="propertyName">is the name of the property to update</param>
        /// <param name="jsonValue">is the new jsonValue of the property</param>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="T"/> not found</exception>
        /// <exception cref="ArgumentException">When the property could not be found on <see cref="T"/> or the jsonValue could not be assigned</exception>
        Task UpdatePropertyAsync(string id, string propertyName, [FromBody] string jsonValue);

        #endregion UPDATE


        #region DELETE

        /// <summary>
        /// Delete is supposed to remove the <see cref="T"/> with the passed id from the database.
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/> to remove from the database</param>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="T"/> not found</exception>
        Task DeleteAsync(string id);

        #endregion DELETE
    }
}