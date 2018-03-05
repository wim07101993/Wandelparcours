using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebService.Controllers.Bases;
using WebService.Models;
using WebService.Models.Bases;
using WebService.Services.Data;
using WebService.Services.Logging;
using ILogger = WebService.Services.Logging.ILogger;

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
        public override Expression<Func<ReceiverModule, object>>[] PropertiesToSendOnGet { get; } = null;


        public override IEnumerable<Expression<Func<ReceiverModule, object>>> ConvertStringsToSelectors(
            IEnumerable<string> strings)
        {
            return null;
        }

        [HttpGet]
        public override async Task<IActionResult> GetAsync()
            => await base.GetAsync();

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

        [HttpPost]
        public override async Task<IActionResult> CreateAsync([FromBody] ReceiverModule item)
            => await base.CreateAsync(item);

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

        [HttpPut]
        public override async Task<IActionResult> UpdateAsync([FromBody] AUpdater<ReceiverModule> updater)
            => await base.UpdateAsync(updater);

        #endregion METHODS
    }
}