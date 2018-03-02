using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebService.Helpers;
using WebService.Models;
using WebService.Services.Data;
using WebService.Services.Logging;

namespace WebService.Controllers
{
    [Route("api/v1/[controller]")]
    public class ReceiverModulesController : Controller
    {
        #region FIELDS

        /// <summary>
        /// _dataService is used to handle the data traffice to and from the database.
        /// </summary>
        private readonly IDataService _dataService;

        /// <summary>
        /// _logger is used to handle the logging of messages.
        /// </summary>
        private readonly ILogger _logger;

        #endregion FIELDS


        #region CONSTRUCTORS

        /// <summary>
        /// ReceiverModulescontroller creates an instance of the <see cref="ReceiverModulesController"/> class. 
        /// </summary>
        /// <param name="dataService">is a service to handle the database connection</param>
        /// <param name="logger">is a service to hanlde the logging of messages</param>
        public ReceiverModulesController(IDataService dataService, ILogger logger)
        {
            // initiate the services
            _dataService = dataService;
            _logger = logger;
        }

        #endregion CONSTRUCTORS


        #region METHODS

        /// <summary>
        /// Get is the method corresponding to the GET method of the /ReceiverModules controller of the REST service. (GET 0.0.0.0:5000/api/v1/receiverModules)
        /// <para/>
        /// It returns all the ReceiverModules in the database wrapped in an <see cref="IActionResult"/>.
        /// </summary>
        /// <returns>
        /// - Status ok (200) with An IEnumerable of all the ReceiverModules in the database on success
        /// - Status internal server (500) error when an error occures
        /// </returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                // return the values that come from the data service wrapped in a 200 response 
                return Ok(_dataService.GetReceiverModules());
            }
            catch (Exception e)
            {
                // log the exception
                _logger.Log(this, ELogLevel.Error, e);
                // return a 500 error to the client
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Create is the method corresonding to the POST method of the /ReceiverModules controller of the REST service. (POST 0.0.0.0:5000/api/v1/receiverModules)
        /// <para/>
        /// It saves the passed <see cref="ReceiverModule"/> to the database.
        /// </summary>
        /// <param name="receiverModule">is the <see cref="ReceiverModule"/> to save in the database</param>
        /// <returns>
        /// - Status created (201) if succes
        /// - Status internal server error (500) on error or not created
        /// </returns>
        [HttpPost]
        public IActionResult Create([FromBody] ReceiverModule receiverModule)
        {
            try
            {
                // use the data service to create a new receiverModule
                return _dataService.CreateReceiverModule(receiverModule) != null
                    // if the receiverModule was created return satus created
                    ? StatusCode((int) HttpStatusCode.Created)
                    // if the receiverModule was not created return status not modified
                    : StatusCode((int) HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                // log the error
                _logger.Log(this, ELogLevel.Error, e);
                // return a 500 internal server error code
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Delete is the method corresonding to the DELETE method of the /ReceiverModules controller of the REST service. (DELETE 0.0.0.0:5000/api/v1/receiverModules/{id})
        /// <para/>
        /// It saves the passed <see cref="ReceiverModule"/> to the database.
        /// </summary>
        /// <param name="id">is the id of the <see cref="ReceiverModule"/> to remove from the database</param>
        /// <returns>
        /// - Status created (201) if succes
        /// - Status not found (40) if there was no erro but also no object to remove
        /// - Status internal server error (500) on error
        /// </returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                // use the data service to remove the receiverModule
                return _dataService.RemoveReceiverModule(new ObjectId(id))
                    // if the receiverModule was deleted return status ok
                    ? StatusCode((int) HttpStatusCode.OK)
                    // if the receiverModule was not deleted return status no content
                    : StatusCode((int) HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                // log the error
                _logger.Log(this, ELogLevel.Error, e);
                // return a 500 internal server error code
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Update is the method corresponding to the PUT method of the /ReceiverModules controller of the REST service. (PUT 0.0.0.0:5000/api/v1/receiverModules)
        /// <para/>
        /// It updates the fields of the <see cref="ReceiverModule"/> in the receiverModule.
        /// If the ReceiverModule doesn't exist, a new is created in the database.
        /// </summary>
        /// <param name="receiverModule">containse the <see cref="ReceiverModule"/> to update ande the properties that should be updated</param>
        /// <returns>
        /// - Status ok (200) if the <see cref="ReceiverModule"/> was updated
        /// - Status created (201) if a new one was created
        /// - Status bad request (400) if the passed receiverModule is null
        /// - Status internal server error (500) on error or not created
        /// </returns>
        [HttpPut]
        public IActionResult Update([FromBody] ReceiverModule receiverModule)
        {
            try
            {
                // check if the receiverModule to update exists
                if (receiverModule == null)
                    return new StatusCodeResult((int) HttpStatusCode.BadRequest);
                
                return _dataService.UpdateReceiverModule(receiverModule) == null
                    // if the update failed, try creating a new receiverModule
                    ? Create(receiverModule)
                    // if the update was a succes, reutrn 200
                    : StatusCode((int) HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                // log the error
                _logger.Log(this, ELogLevel.Error, e);
                // return a 500 internal server error code
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        #endregion METHOD
    }
}