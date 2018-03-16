using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Controllers.Bases;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Data;
using WebService.Services.Exceptions;
using WebService.Services.Logging;

namespace WebService.Controllers
{
    /// <inheritdoc cref="ARestControllerBase{T}"/>
    /// <summary>
    /// ReceiverModulesController handles the reading and writing of receiver module data to the database.
    /// </summary>
    [Route("api/v1/[controller]")]
    public class ReceiverModulesController : ARestControllerBase<ReceiverModule>, IReceiverModulesController
    {
        #region CONSTRUCTOR

        public ReceiverModulesController(IThrow iThrow, IDataService<ReceiverModule> dataService, ILogger logger)
            : base(iThrow, dataService, logger)
        {
        }

        #endregion CONSTRUCTOR


        #region PROPERTIES

        public override IEnumerable<Expression<Func<ReceiverModule, object>>> PropertiesToSendOnGetAll { get; } = null;

        public override IDictionary<string, Expression<Func<ReceiverModule, object>>> PropertySelectors { get; } =
            new Dictionary<string, Expression<Func<ReceiverModule, object>>>
            {
                {nameof(ReceiverModule.Mac), x => x.Mac},
                {nameof(ReceiverModule.Id), x => x.Id},
                {nameof(ReceiverModule.IsActive), x => x.IsActive},
                {nameof(ReceiverModule.Position), x => x.Position},
            };

        #endregion PROPERTIES


        #region METHODS

        #region post (create)

        [HttpPost]
        public override async Task CreateAsync([FromBody] ReceiverModule item)
        {
            item.Position.TimeStamp = DateTime.Now;
            await base.CreateAsync(item);
        }

        #endregion post (create)

        #region get (read)

        [HttpGet("{id}/{propertyName}")]
        public override Task<object> GetPropertyAsync(string id, string propertyName)
            => base.GetPropertyAsync(id, propertyName);

        [HttpGet("{mac}")]
        public override async Task<ReceiverModule> GetAsync(string mac, [FromQuery] string[] propertiesToInclude)
        {
            // convert the property names to selectors, if there are any
            var selectors = !EnumerableExtensions.IsNullOrEmpty(propertiesToInclude)
                ? ConvertStringsToSelectors(propertiesToInclude)
                : null;

            // get the value from the data service
            return await ((IReceiverModulesService) DataService).GetAsync(mac, selectors)
                   ?? throw new NotFoundException(
                       $"The {typeof(ReceiverModule).Name} with id {mac} could not be found");
        }

        [HttpGet]
        public override Task<IEnumerable<ReceiverModule>> GetAsync([FromQuery] string[] propertiesToInclude)
            => base.GetAsync(propertiesToInclude);

        #endregion get (read)

        #region put (update)

        [HttpPut]
        public override Task UpdateAsync([FromBody] ReceiverModule item, [FromQuery] string[] propertiesToUpdate)
            => base.UpdateAsync(item, propertiesToUpdate);

        [HttpPut("{id}/{propertyName}")]
        public override Task UpdatePropertyAsync(string id, string propertyName, [FromBody] string jsonValue)
            => base.UpdatePropertyAsync(id, propertyName, jsonValue);

        #endregion put (update)

        #region delete

        [HttpDelete("{mac}")]
        public override Task DeleteAsync(string mac)
            // use the data service to remove the item
            => ((IReceiverModulesService) DataService).RemoveAsync(mac);

        #endregion delete

        #endregion METHODS
    }
}