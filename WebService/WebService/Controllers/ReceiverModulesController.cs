using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        public override Expression<Func<ReceiverModule, object>>[] PropertiesToSendOnGet { get; } = null;


        public override IEnumerable<Expression<Func<ReceiverModule, object>>> ConvertStringsToSelectors(
            IEnumerable<string> strings)
        {
            return null;
        }

        [HttpGet]
        public override async Task<IActionResult> GetAsync()
            => await base.GetAsync();

        [HttpPost]
        public override async Task<IActionResult> CreateAsync([FromBody] ReceiverModule item)
            => await base.CreateAsync(item);

        [HttpDelete("{id}")]
        public override async Task<IActionResult> DeleteAsync(string id)
            => await base.DeleteAsync(id);

        [HttpPut]
        public override async Task<IActionResult> UpdateAsync([FromBody] AUpdater<ReceiverModule> updater)
            => await base.UpdateAsync(updater);

        #endregion METHODS
    }
}