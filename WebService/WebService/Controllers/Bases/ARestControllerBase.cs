using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models.Bases;
using WebService.Services.Data;
using WebService.Services.Logging;

namespace WebService.Controllers.Bases
{
    /// <inheritdoc cref="Controller" />
    /// <inheritdoc cref="IRestController{T}" />
    /// <summary>
    /// ARestControllerBase is an abstract class that holds the methods to Get, Create, Delete and Update data to a database.
    /// </summary>
    /// <typeparam name="T">is the type of the data to handle</typeparam>
    public abstract class ARestControllerBase<T> : Controller, IRestController<T> where T : IModelWithID
    {
        #region FIELDS

        /// <summary>
        /// _dataService is used to handle the data traffice to and from the database.
        /// </summary>
        protected readonly IDataService<T> DataService;

        /// <summary>
        /// _logger is used to handle the logging of messages.
        /// </summary>
        protected readonly ILogger Logger;

        #endregion FIELDS


        #region CONSTRUCTORS

        /// <summary>
        /// ARestControllerBase creates an instance of the <see cref="ARestControllerBase{T}"/> class. 
        /// </summary>
        /// <param name="dataService">is a service to handle the database connection</param>
        /// <param name="logger">is a service to hanlde the logging of messages</param>
        protected ARestControllerBase(IDataService<T> dataService, ILogger logger)
        {
            // initiate the services
            DataService = dataService;
            Logger = logger;
        }

        #endregion CONSTRUCTORS


        #region PROPERTIES

        /// <summary>
        /// PropertiesToSendOnGetAll are the selectors of the properties to send when all the <see cref="T"/>s are asked from the database.
        /// </summary>
        public abstract IEnumerable<Expression<Func<T, object>>> PropertiesToSendOnGetAll { get; }

        #endregion PROPERTIES


        #region METHODS

        /// <summary>
        /// ConvertStringsToSelectors should convert a collection of property names to their selector in the form of 
        /// <see cref="Expression{TDelegate}"/> of type <see cref="Func{TInput,TResult}"/>
        /// </summary>
        /// <param name="strings">are the property names to convert to selectors</param>
        /// <returns>An <see cref="IEnumerable{TDelegate}"/> that contains the converted selectors</returns>
        public abstract IEnumerable<Expression<Func<T, object>>> ConvertStringsToSelectors(IEnumerable<string> strings);

        /// <inheritdoc cref="IRestController{T}.GetAsync(string,string[])" />
        /// <summary>
        /// Get fetches the item with the given id in the database. 
        /// To limit data traffic it is possible to select only a number of properties
        /// </summary>
        /// <returns>The <see cref="T"/> in the database that has the given id</returns>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="T"/> not found</exception>
        /// <exception cref="WebArgumentException">When the properties could not be converted to selectors</exception>
        public virtual async Task<T> GetAsync(string id, [FromQuery] string[] properties)
        {
            // parse the id
            if (!ObjectId.TryParse(id, out var objectId))
                // if it fails, throw not found exception
                throw new NotFoundException($"The {typeof(T).Name} with id {id} could not be found");

            //create selectors
            IEnumerable<Expression<Func<T, object>>> selectors = null;
            // if there are no properties, they don't need to be converted
            if (!EnumerableExtensions.IsNullOrEmpty(properties))
                // convert the property names to selectors
                selectors = ConvertStringsToSelectors(properties);

            // get the value from the data service
            var item = await DataService.GetAsync(objectId, selectors);

            return item == null
                // if the item is null, throw a not found exception
                ? throw new NotFoundException($"The {typeof(T).Name} with id {id} could not be found")
                // else return the values
                : item;
        }

        /// <inheritdoc cref="IRestController{T}.GetAsync(string[])" />
        /// <summary>
        /// Get returns all the Items in the database wrapped in an <see cref="IActionResult"/>. 
        /// To limit data traffic it is possible to select only a number of properties by default. 
        /// By default the properties in the <see cref="PropertiesToSendOnGetAll"/> are the only ones sent.
        /// </summary>
        /// <returns>
        /// All <see cref="T"/>s in the database but only the given properties are filled in
        /// </returns>
        /// <exception cref="WebArgumentException">When the properties could not be converted to selectors</exception>
        public virtual async Task<IEnumerable<T>> GetAsync([FromQuery] string[] properties)
        {
            // the default selectors ar in the PropertiesToSendOnGetAll property
            var selectors = PropertiesToSendOnGetAll;
            if (!EnumerableExtensions.IsNullOrEmpty(properties))
                // convert the property names to selectors
                selectors = ConvertStringsToSelectors(properties);

            // return the items got from the data service
            return await DataService.GetAsync(selectors);
        }

        /// <inheritdoc cref="IRestController{T}.CreateAsync" />
        /// <summary>
        /// Create saves the passed <see cref="T"/> to the database.
        /// </summary>
        /// <param name="item">is the <see cref="T"/> to save in the database</param>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="T"/> not found</exception>
        public virtual async Task CreateAsync([FromBody] T item)
        {
            // use the data service to create a new item
            var result = await DataService.CreateAsync(item);

            // if the item was not created return throw exception
            if (!result)
                throw new Exception($"Could not create {typeof(T).Name} in the database");
        }

        /// <inheritdoc cref="IRestController{T}.DeleteAsync" />
        /// <summary>
        /// Delete removes the <see cref="T"/> with the passed id from the database.
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/> to remove from the database</param>
        /// <returns>Status created (201) if succes</returns>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="T"/> not found</exception>
        public virtual async Task DeleteAsync(string id)
        {
            // parse the id
            if (!ObjectId.TryParse(id, out var objectId))
                // if it fails, throw not found exception
                throw new NotFoundException($"The {typeof(T).Name} with id {id} could not be found");

            // use the data service to remove the item
            var result = await DataService.RemoveAsync(objectId);

            // if the item could not be deleted, throw exception
            if (!result)
                throw new NotFoundException($"The {typeof(T).Name} with id {id} could not be found");
        }

        /// <inheritdoc cref="IRestController{T}.UpdateAsync" />
        /// <summary>
        /// Update updates the fields of the <see cref="T"/> that are specified in the <see cref="properties"/> parameter.
        /// If the item doesn't exist, a new is created in the database.
        /// </summary>
        /// <param name="item">is the <see cref="T"/> to update</param>
        /// <param name="properties">contains the properties that should be updated</param>
        /// <returns>Status ok (200) if the <see cref="T"/> was updated</returns>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="T"/> not found</exception>
        /// <exception cref="WebArgumentException">When the properties could not be converted to selectors</exception>
        public virtual async Task UpdateAsync([FromBody] T item, [FromQuery] string[] properties)
        {
            //create selectors
            IEnumerable<Expression<Func<T, object>>> selectors = null;
            // if there are no properties, they don't need to be converted
            if (!EnumerableExtensions.IsNullOrEmpty(properties))
                // convert the property names to selectors
                selectors = ConvertStringsToSelectors(properties);

            // boolean to indicate whether the item is updated
            var itemUpdated = EnumerableExtensions.IsNullOrEmpty(properties)
                // if there are no properties to update, pass none to the data service
                ? await DataService.UpdateAsync(item)
                // update the item in the data service
                : await DataService.UpdateAsync(item, selectors);

            // if the update was a succes, reutrn 200
            if (!itemUpdated)
                await CreateAsync(item);
        }

        #endregion METHOD
    }
}