using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Controllers.Bases;
using WebService.Models;
using WebService.Models.Bases;
using WebService.Services.Data;
using WebService.Services.Logging;

namespace WebService.Controllers
{
    [Route("api/v1/[controller]")]
    public class ReceiverModulesController : ARestControllerBase<ReceiverModule>
    {
        #region CONSTRUCTOR

        /// <inheritdoc cref="ARestControllerBase{T}" />
        /// <summary>
        /// Residentscontroller creates an instance of the <see cref="ReceiverModulesController" /> class. 
        /// </summary>
        /// <param name="dataService">is a service to handle the database connection</param>
        /// <param name="logger">is a service to hanlde the logging of messages</param>
        public ReceiverModulesController(IDataService<ReceiverModule> dataService, ILogger logger)
            : base(dataService, logger)
        {
        }

        #endregion CONSTRUCTOR


        #region METHODS

        /// <summary>
        /// SmallDataProperties is a collection of expressions to select the properties that
        /// consume the least space (FirstName, LastName, Room Birthday and Doctor).
        /// </summary>
        public override IEnumerable<Expression<Func<ReceiverModule, object>>> PropertiesToSendOnGet { get; } = null;


        public override IEnumerable<Expression<Func<ReceiverModule, object>>> ConvertStringsToSelectors(
            IEnumerable<string> strings)
        {
            return null;
        }

        /// <inheritdoc cref="GetAsync()"/>
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
        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] string[] properties)
            => await base.GetAsync(properties);

        /// <summary>
        /// Get is the method corresponding to the GET method of the controller of the REST service.
        /// <para/>
        /// It returns the Item with the given id in the database wrapped in an <see cref="IActionResult"/>.
        /// </summary>
        /// <param name="mac">is the mac address of the <see cref="ReceiverModule"/></param>
        /// <returns>
        /// - Status ok (200) with An IEnumerable of all the Items in the database on success
        /// - Status bad request (400) when there are properties passed that do not exist in a <see cref="ReceiverModule"/>
        /// - Status not found (404) when there is no <see cref="ReceiverModule"/> with the given id found
        /// - Status internal server error (500) when an error occures
        /// </returns>
        [HttpGet("{mac}")]
        public async Task<IActionResult> GetAsync(string mac)
        {
            try
            {
                // get the value from the data service
                var item = await ((IReceiverModuleService) DataService).GetAsync(mac);
                // return the value wrapped in a 200 response 
                return Ok(item);
            }
            catch (Exception e)
            {
                // log the exception
                Logger.Log(this, ELogLevel.Error, e);
                // return a 500 error to the client
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        /// <inheritdoc cref="ARestControllerBase{T}.CreateAsync" />
        /// <summary>
        /// Create is the method corresonding to the POST method of the controller of the REST service.
        /// <para/>
        /// It saves the passed <see cref="ReceiverModule"/> to the database.
        /// </summary>
        /// <param name="item">is the <see cref="ReceiverModule"/> to save in the database</param>
        /// <returns>
        /// - Status created (201) if succes
        /// - Status internal server error (500) on error or not created
        /// </returns>
        [HttpPost]
        public override async Task<IActionResult> CreateAsync([FromBody] ReceiverModule item)
            => await base.CreateAsync(item);

        /// <inheritdoc cref="ARestControllerBase{T}.DeleteAsync" />
        /// <summary>
        /// Delete is the method corresonding to the DELETE method of the controller of the REST service.
        /// <para/>
        /// It saves the passed <see cref="ReceiverModule"/> to the database.
        /// </summary>
        /// <param name="mac">is the mac address of the <see cref="ReceiverModule"/> to remove from the database</param>
        /// <returns>
        /// - Status created (201) if succes
        /// - Status not found (40) if there was no erro but also no object to remove
        /// - Status internal server error (500) on error
        /// </returns>
        [HttpDelete("{mac}")]
        public override async Task<IActionResult> DeleteAsync(string mac)
        {
            try
            {
                // use the data service to remove the updater
                return await ((IReceiverModuleService) DataService).RemoveAsync(mac)
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
        /// It updates the fields of the <see cref="ReceiverModule"/> in the updater.
        /// If the Item doesn't exist, a new is created in the database.
        /// </summary>
        /// <param name="updater">containse the <see cref="ReceiverModule"/> to update ande the properties that should be updated</param>
        /// <returns>
        /// - Status ok (200) if the <see cref="ReceiverModule"/> was updated
        /// - Status created (201) if a new one was created
        /// - Status bad request (400) if the passed updater is null
        /// - Status internal server error (500) on error or not created
        /// </returns>
        [HttpPut]
        [Obsolete]
        public async Task<IActionResult> UpdateAsync([FromBody] AUpdater<ReceiverModule> updater)
            => await UpdateAsync(updater.Value, updater.PropertiesToUpdate);

        /// <inheritdoc cref="ARestControllerBase{T}.UpdateAsync" />
        /// <summary>
        /// Update is the method corresponding to the PUT method of the controller of the REST service.
        /// <para />
        /// It updates the fields of the <see cref="ReceiverModule" /> in the updater.
        /// If the Item doesn't exist, a new is created in the database.
        /// </summary>
        /// <param name="item">is the <see cref="ReceiverModule" /> to update</param>
        /// <param name="properties">contains the properties that should be updated</param>
        /// <returns>
        /// - Status ok (200) if the <see cref="ReceiverModule" /> was updated
        /// - Status created (201) if a new one was created
        /// - Status bad request (400) if the passed properties are not found on <see cref="ReceiverModule" />
        /// - Status internal server error (500) on error or not created
        /// </returns>
        [HttpPut]
        public override async Task<IActionResult> UpdateAsync([FromBody] ReceiverModule item,
            [FromQuery] string[] properties)
            => await base.UpdateAsync(item, properties);

        #endregion METHODS
    }
}