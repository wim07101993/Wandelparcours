using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebService.Controllers.Bases;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Data;
using WebService.Services.Exceptions;
using WebService.Services.Logging;

namespace WebService.Controllers
{
    [Route("api/v1/[controller]")]
    public class UsersController : ARestControllerBase<User>
    {
        public UsersController(IThrow iThrow, IUsersService dataService, ILogger logger)
            : base(iThrow, dataService, logger)
        {
        }

        public override IEnumerable<Expression<Func<User, object>>> PropertiesToSendOnGetAll { get; } =
            new Expression<Func<User, object>>[]
            {
                x => x.Id,
                x => x.UserName,
                x => x.Email,
                x => x.AuthLevel
            };

        public override IDictionary<string, Expression<Func<User, object>>> PropertySelectors { get; } =
            new Dictionary<string, Expression<Func<User, object>>>
            {
                {nameof(Models.User.Id), x => x.Id},
                {nameof(Models.User.UserName), x => x.UserName},
                {nameof(Models.User.Email), x => x.Email},
                {nameof(Models.User.Password), x => x.Password},
                {nameof(Models.User.AuthLevel), x => x.AuthLevel},
                {nameof(Models.User.Residents), x => x.Residents},
            };

        [HttpPut]
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

            var user = await DataService.GetAsync(item.Id, PropertiesToSendOnGetAll);
            user.Password = item.Password;
            await DataService.UpdatePropertyAsync(user.Id, x => x.Password, user.Password);
        }

        [HttpPut("{id}/{propertyName}")]
        public override async Task UpdatePropertyAsync(string id, string propertyName, [FromQuery] string jsonValue)
        {
            if (!propertyName.EqualsWithCamelCasing(nameof(Models.User.Password)))
            {
                await base.UpdatePropertyAsync(id, propertyName, jsonValue);
                return;
            }

            if (!ObjectId.TryParse(id, out var objectId))
                // if it fails, throw not found exception
                throw new NotFoundException($"The {typeof(User).Name} with id {id} could not be found");

            await ((IUsersService) DataService).TaskUpdatePasswordAsync(objectId, jsonValue);
        }
    }
}