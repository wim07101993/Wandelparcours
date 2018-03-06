using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebService.Helpers.Extensions;
using WebService.Models.Bases;
using WebService.Services.Data;
using WebService.Services.Logging;

namespace WebService.Controllers.Bases
{
    public abstract class ARestControllerBase<T> : Controller where T : IModelWithID
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

        public abstract Expression<Func<T, object>>[] PropertiesToSendOnGet { get; }

        #endregion PROPERTIES


        #region METHODS

        /// <summary>
        /// ConvertStringsToSelectors should convert a collection of property names to their selector in the form of 
        /// <see cref="Expression{TDelegate}"/> of type <see cref="Func{TInput,TResult}"/>
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        public abstract IEnumerable<Expression<Func<T, object>>> ConvertStringsToSelectors(IEnumerable<string> strings);

        /// <summary>
        /// Get is the method corresponding to the GET method of the controller of the REST service.
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
        public virtual async Task<IActionResult> GetAsync(string id, string[] properties)
        {
            // parse the id
            if (!ObjectId.TryParse(id, out var objectId))
                // if it fails, return a 404
                return StatusCode((int) HttpStatusCode.NotFound);

            //create selectors
            IEnumerable<Expression<Func<T, object>>> selectors = null;
            // if there are no properties, they don't need to be converted
            if (!EnumerableExtensions.IsNullOrEmpty(properties))
                try
                {
                    // try converting the propertie names to selectors
                    selectors = ConvertStringsToSelectors(properties);
                }
                catch (ArgumentException)
                {
                    // if it fails because of a bad argument (properties cannot be found)
                    // return a 400 error
                    return StatusCode((int) HttpStatusCode.BadRequest);
                }

            try
            {
                // get the value from the data service
                var item = await DataService.GetAsync(objectId, selectors);

                return item == null
                    // if the item is null, return a 404
                    ? StatusCode((int) HttpStatusCode.NotFound)
                    // else return the values wrapped in a 200 response 
                    : (IActionResult) Ok(item);
            }
            catch (Exception e)
            {
                // log the exception
                Logger.Log(this, ELogLevel.Error, e);
                // return a 500 error to the client
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Get is the method corresponding to the GET method of the controller of the REST service.
        /// <para/>
        /// It returns all the Items in the database wrapped in an <see cref="IActionResult"/>. To limit data traffic it is possible to
        /// select only a number of properties by default. These properties are selected with the <see cref="PropertiesToSendOnGet"/> property.
        /// </summary>
        /// <returns>
        /// - Status ok (200) with An IEnumerable of all the Items in the database on success
        /// - Status internal server (500) error when an error occures
        /// </returns>
        public virtual async Task<IActionResult> GetAsync()
        {
            try
            {
                // get the values from the data service
                var items = await DataService.GetAsync(PropertiesToSendOnGet);
                // return the values wrapped in a 200 response 
                return Ok(items);
            }
            catch (Exception e)
            {
                // log the exception
                Logger.Log(this, ELogLevel.Error, e);
                // return a 500 error to the client
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Create is the method corresonding to the POST method of the controller of the REST service.
        /// <para/>
        /// It saves the passed <see cref="T"/> to the database.
        /// </summary>
        /// <param name="item">is the <see cref="T"/> to save in the database</param>
        /// <returns>
        /// - Status created (201) if succes
        /// - Status internal server error (500) on error or not created
        /// </returns>
        public virtual async Task<IActionResult> CreateAsync([FromBody] T item)
        {
            try
            {
                // use the data service to create a new updater
                return await DataService.CreateAsync(item) != null
                    // if the updater was created return satus created
                    ? StatusCode((int) HttpStatusCode.Created)
                    // if the updater was not created return status not modified
                    : StatusCode((int) HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                // log the error
                Logger.Log(this, ELogLevel.Error, e);
                // return a 500 internal server error code
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Delete is the method corresonding to the DELETE method of the controller of the REST service.
        /// <para/>
        /// It saves the passed <see cref="T"/> to the database.
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/> to remove from the database</param>
        /// <returns>
        /// - Status created (201) if succes
        /// - Status not found (40) if there was no erro but also no object to remove
        /// - Status internal server error (500) on error
        /// </returns>
        public virtual async Task<IActionResult> DeleteAsync(string id)
        {
            // try to parse the id
            if (!ObjectId.TryParse(id, out var objectId))
                // if it fails, return a 404 error
                return StatusCode((int) HttpStatusCode.NotFound);

            try
            {
                // use the data service to remove the updater
                return await DataService.RemoveAsync(objectId)
                    // if the updater was deleted return status ok
                    ? StatusCode((int) HttpStatusCode.OK)
                    // if the updater was not deleted return status no content
                    : StatusCode((int) HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                // log the error
                Logger.Log(this, ELogLevel.Error, e);
                // return a 500 internal server error code
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Update is the method corresponding to the PUT method of the controller of the REST service.
        /// <para/>
        /// It updates the fields of the <see cref="T"/> in the updater.
        /// If the Item doesn't exist, a new is created in the database.
        /// </summary>
        /// <param name="updater">containse the <see cref="T"/> to update ande the properties that should be updated</param>
        /// <returns>
        /// - Status ok (200) if the <see cref="T"/> was updated
        /// - Status created (201) if a new one was created
        /// - Status bad request (400) if the passed updater is null
        /// - Status internal server error (500) on error or not created
        /// </returns>
        public virtual async Task<IActionResult> UpdateAsync([FromBody] AUpdater<T> updater)
        {
            try
            {
                // check if the resident to update exists
                if (updater.Value == null)
                    return new StatusCodeResult((int) HttpStatusCode.BadRequest);

                T updatedResident;
                if (EnumerableExtensions.IsNullOrEmpty(updater.PropertiesToUpdate))
                    // if there are no properties to update, pass none to the data service
                    updatedResident = await DataService.UpdateAsync(updater.Value);
                else
                {
                    // create a new list of selectors
                    var selectors = ConvertStringsToSelectors(updater.PropertiesToUpdate);

                    // update the resident in the data service
                    updatedResident = await DataService.UpdateAsync(updater.Value, selectors);
                }

                return Equals(updatedResident, default(T))
                    // if the update failed, try creating a new resident
                    ? await CreateAsync(updater.Value)
                    // if the update was a succes, reutrn 200
                    : StatusCode((int) HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                // log the error
                Logger.Log(this, ELogLevel.Error, e);
                // return a 500 internal server error code
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        #endregion METHOD
    }
}