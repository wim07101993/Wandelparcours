using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebService.Controllers.Bases;
using WebService.Helpers.Attributes;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Models.Bases;
using WebService.Services.Data;
using WebService.Services.Logging;

namespace WebService.Controllers
{
    [Route("api/v1/[controller]")]
    public class UsersController : ARestControllerBase<User>
    {
        public UsersController(IUsersService dataService, ILogger logger, IUsersService usersService)
            : base(dataService, logger, usersService)
        {
        }


        public override IEnumerable<Expression<Func<User, object>>> PropertiesToSendOnGetAll { get; } =
            new Expression<Func<User, object>>[]
            {
                x => x.Id,
                x => x.UserName,
                x => x.Email,
                x => x.UserType
            };

        public override IDictionary<string, Expression<Func<User, object>>> PropertySelectors { get; } =
            new Dictionary<string, Expression<Func<User, object>>>
            {
                {nameof(Models.User.Id), x => x.Id},
                {nameof(Models.User.UserName), x => x.UserName},
                {nameof(Models.User.Email), x => x.Email},
                {nameof(Models.User.Password), x => x.Password},
                {nameof(Models.User.UserType), x => x.UserType},
                {nameof(Models.User.Residents), x => x.Residents},
            };


        #region METHDOS

        [Authorize(EUserType.SysAdmin, EUserType.Nurse)]
        [HttpPost(CreateTemplate)]
        public override Task<string> CreateAsync([FromBody] User item)
            => base.CreateAsync(item);


        [Authorize(EUserType.SysAdmin, EUserType.Nurse)]
        [HttpGet(GetAllTemplate)]
        public override Task<IEnumerable<User>> GetAllAsync(string[] propertiesToInclude)
            => base.GetAllAsync(propertiesToInclude);

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User)]
        [HttpGet(GetOneTemplate)]
        public override Task<User> GetOneAsync(string id, string[] propertiesToInclude) =>
            base.GetOneAsync(id, propertiesToInclude);

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User)]
        [HttpGet(GetPropertyTemplate)]
        public override Task<object> GetPropertyAsync(string id, string propertyName) =>
            base.GetPropertyAsync(id, propertyName);


        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User)]
        [HttpPut(UpdateTemplate)]
        public override async Task UpdateAsync([FromBody] User item, [FromQuery] string[] properties)
        {
            if (properties.All(x => !x.EqualsWithCamelCasing(nameof(Models.User.Password))))
            {
                await base.UpdateAsync(item, properties);
                return;
            }

            if (properties.Length > 1)
            {
                var propertyList = properties.ToList();
                propertyList.Remove(x => x.EqualsWithCamelCasing(nameof(Models.User.Password)));

                await base.UpdateAsync(item, properties);
            }

            var user = await DataService.GetOneAsync(item.Id, PropertiesToSendOnGetAll);
            user.Password = item.Password;
            await DataService.UpdatePropertyAsync(user.Id, x => x.Password, user.Password);
        }

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User)]
        [HttpPut(UpdatePropertyTemplate)]
        public override async Task UpdatePropertyAsync(string id, string propertyName, [FromQuery] string jsonValue)
        {
            if (!propertyName.EqualsWithCamelCasing(nameof(Models.User.Password)))
            {
                await base.UpdatePropertyAsync(id, propertyName, jsonValue);
                return;
            }

            if (!ObjectId.TryParse(id, out var objectId))
                throw new NotFoundException<User>(nameof(IModelWithID.Id), id);

            await ((IUsersService) DataService).UpdatePasswordAsync(objectId, jsonValue);
        }


        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User)]
        [HttpDelete(DeleteTemplate)]
        public override Task DeleteAsync(string id) => base.DeleteAsync(id);

        #endregion METHODS
    }
}