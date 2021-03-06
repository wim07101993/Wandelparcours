﻿using System;
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
    [Route(Routes.RestBase.ControllerRoute)]
    public class ReceiverModulesController : ARestControllerBase<ReceiverModule>, IReceiverModulesController
    {
        #region CONSTRUCTOR

        public ReceiverModulesController(IReceiverModulesService dataService, ILogger logger,
            IUsersService usersService)
            : base(dataService, logger, usersService)
        {
        }

        #endregion CONSTRUCTOR


        #region PROPERTIES

        protected override IDictionary<string, Expression<Func<ReceiverModule, object>>> PropertySelectors { get; } =
            new Dictionary<string, Expression<Func<ReceiverModule, object>>>
            {
                {nameof(ReceiverModule.Mac), x => x.Mac},
                {nameof(ReceiverModule.Id), x => x.Id},
                {nameof(ReceiverModule.IsActive), x => x.IsActive},
                {nameof(ReceiverModule.Position), x => x.Position},
                {nameof(ReceiverModule.Name), x => x.Name}
            };

        #endregion PROPERTIES


        #region METHODS    

        #region post (create)

        [Authorize(EUserType.SysAdmin)]
        [HttpPost(Routes.RestBase.Create)]
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

        [Authorize(EUserType.Module, EUserType.Nurse,EUserType.User)]
        [HttpGet(Routes.RestBase.GetAll)]
        public override Task<IEnumerable<ReceiverModule>> GetAllAsync(string[] propertiesToInclude)
            => base.GetAllAsync(propertiesToInclude);

        [Authorize(EUserType.SysAdmin)]
        [HttpGet(Routes.ReceiverModules.GetOneByName)]
        public async Task<ReceiverModule> GetOneByNameAsync(string name, [FromQuery] string[] propertiesToInclude)
        {
            if (name == null)
                throw new NotFoundException<ReceiverModule>(nameof(ReceiverModule.Mac), null);

            var selectors = !EnumerableExtensions.IsNullOrEmpty(propertiesToInclude)
                ? ConvertStringsToSelectors(propertiesToInclude)
                : null;

            return await ((IReceiverModulesService) DataService).GetOneAsync(name, selectors)
                   ?? throw new NotFoundException<ReceiverModule>(nameof(ReceiverModule.Mac), name);
        }

        [Authorize(EUserType.SysAdmin)]
        [HttpGet(Routes.RestBase.GetOne)]
        public override Task<ReceiverModule> GetOneAsync(string id, [FromQuery] string[] propertiesToInclude)
            => base.GetOneAsync(id, propertiesToInclude);

        #endregion get (read)

        
        #region update
        
        [Authorize(EUserType.SysAdmin)]
        [HttpPut(Routes.RestBase.UpdateProperty)]
        public override Task UpdatePropertyAsync(string id, string propertyName, [FromBody] string jsonValue)
        {
            return base.UpdatePropertyAsync(id, propertyName, jsonValue);
        }

        #endregion update
        
        
        #region delete

        [Authorize(EUserType.SysAdmin)]
        [HttpDelete(Routes.ReceiverModules.DeleteByName)]
        public Task DeleteByNameAsync(string name)
            => ((IReceiverModulesService) DataService).RemoveAsync(name);

        [Authorize(EUserType.SysAdmin)]
        [HttpDelete(Routes.RestBase.Delete)]
        public override Task DeleteAsync(string id)
            => base.DeleteAsync(id);

        #endregion delete

        #endregion METHODS
    }
}