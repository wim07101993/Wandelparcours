using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Controllers.Bases;
using WebService.Helpers.Attributes;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Data;
using WebService.Services.Logging;
using ArgumentNullException = System.ArgumentNullException;

namespace WebService.Controllers
{
    [Route("api/v1/[controller]")]
    public class ReceiverModulesController : ARestControllerBase<ReceiverModule>, IReceiverModulesController
    {
        #region FIELDS

        public const string GetOneByMacTemplate = "bymac/{mac}";
        public const string DeleteByMacTemplate = "bymac/{mac}";


        #endregion FIELDS


        #region CONSTRUCTOR

        public ReceiverModulesController(IReceiverModulesService dataService, ILogger logger,
            IUsersService usersService)
            : base(dataService, logger, usersService)
        {
        }

        #endregion CONSTRUCTOR


        #region PROPERTIES

        protected override IEnumerable<Expression<Func<ReceiverModule, object>>> PropertiesToSendOnGetAll => null;

        protected override IDictionary<string, Expression<Func<ReceiverModule, object>>> PropertySelectors { get; } =
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

        [Authorize(EUserType.SysAdmin)]
        [HttpPost(CreateTemplate)]
        public override async Task<string> CreateAsync([FromBody] ReceiverModule item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (item.Position == null)
                item.Position = new Point();

            item.Position.TimeStamp = DateTime.Now;
            await base.CreateAsync(item);
            return item.Id.ToString();
        }

        #endregion post (create)

        #region get (read)

        [Authorize(EUserType.SysAdmin)]
        [HttpGet(GetAllTemplate)]
        public override Task<IEnumerable<ReceiverModule>> GetAllAsync(string[] propertiesToInclude)
            => base.GetAllAsync(propertiesToInclude);

        [Authorize(EUserType.SysAdmin)]
        [HttpGet(GetOneByMacTemplate)]
        public async Task<ReceiverModule> GetOneByMacAsync(string mac, [FromQuery] string[] propertiesToInclude)
        {
            if (mac == null)
                throw new NotFoundException<ReceiverModule>(nameof(ReceiverModule.Mac), null);

            var selectors = !EnumerableExtensions.IsNullOrEmpty(propertiesToInclude)
                ? ConvertStringsToSelectors(propertiesToInclude)
                : new Expression<Func<ReceiverModule, object>>[0];

            return await ((IReceiverModulesService) DataService).GetAsync(mac, selectors)
                   ?? throw new NotFoundException<ReceiverModule>(nameof(ReceiverModule.Mac), mac);
        }

        [Authorize(EUserType.SysAdmin)]
        [HttpGet(GetOneTemplate)]
        public override Task<ReceiverModule> GetOneAsync(string id, [FromQuery] string[] propertiesToInclude)
            => base.GetOneAsync(id, propertiesToInclude);

        #endregion get (read)

        #region delete

        [Authorize(EUserType.SysAdmin)]
        [HttpDelete(DeleteByMacTemplate)]
        public Task DeleteByMacAsync(string mac)
            => ((IReceiverModulesService) DataService).RemoveAsync(mac);

        [Authorize(EUserType.SysAdmin)]
        [HttpDelete(DeleteTemplate)]
        public override Task DeleteAsync(string id)
            => base.DeleteAsync(id);

        #endregion delete

        [Authorize(EUserType.SysAdmin)]
        [HttpPut(UpdatePropertyTemplate)]
        public override Task UpdatePropertyAsync(string id, string propertyName, [FromBody]string jsonValue)
        {
            return base.UpdatePropertyAsync(id, propertyName, jsonValue);
        }

        #endregion METHODS
    }
}