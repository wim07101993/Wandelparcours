using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
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
        #region FIELDS

        public new const string GetOneTemplate = "{mac}";
        public new const string DeleteTemplate = "{mac}";

        #endregion FIELDS


        #region CONSTRUCTOR

        public ReceiverModulesController(IThrow iThrow, IReceiverModulesService dataService, ILogger logger)
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

        [HttpPost(CreateTemplate)]
        public override async Task<StatusCodeResult> CreateAsync([FromBody] ReceiverModule item)
        {
            if (item == null)
            {
                Throw.NullArgument(nameof(item));
                return null;
            }

            if (item.Position == null)
                item.Position = new Point();

            item.Position.TimeStamp = DateTime.Now;
            await base.CreateAsync(item);
            return StatusCode((int) HttpStatusCode.Created);
        }

        #endregion post (create)

        #region get (read)

        [HttpGet(GetOneTemplate)]
        public override async Task<ReceiverModule> GetOneAsync(string mac, [FromQuery] string[] propertiesToInclude)
        {
            if (mac == null)
            {
                Throw.NotFound<ReceiverModule>(null);
                return null;
            }

            // convert the property names to selectors, if there are any
            var selectors = !EnumerableExtensions.IsNullOrEmpty(propertiesToInclude)
                ? ConvertStringsToSelectors(propertiesToInclude)
                : new Expression<Func<ReceiverModule, object>>[0];

            // get the value from the data service
            return await ((IReceiverModulesService) DataService).GetAsync(mac, selectors)
                   ?? throw new NotFoundException(
                       $"The {typeof(ReceiverModule).Name} with id {mac} could not be found");
        }

        #endregion get (read)

        #region delete

        [HttpDelete(DeleteTemplate)]
        public override Task DeleteAsync(string mac)
            // use the data service to remove the item
            => ((IReceiverModulesService) DataService).RemoveAsync(mac);

        #endregion delete

        #endregion METHODS
    }
}