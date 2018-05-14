using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
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
using ArgumentException = WebService.Helpers.Exceptions.ArgumentException;
using ArgumentNullException = WebService.Helpers.Exceptions.ArgumentNullException;

namespace WebService.Controllers
{
    [Route(Routes.RestBase.ControllerRoute)]
    public class UsersController : ARestControllerBase<User>, IUsersController
    {
        #region FIELDS

        private readonly IUsersService _usersService;

        #endregion FIELDS


        #region CONSTRUCTORS

        public UsersController(IUsersService dataService, ILogger logger, IUsersService usersService)
            : base(dataService, logger, usersService)
        {
            _usersService = usersService;
        }

        #endregion CONSTRUCTORS


        #region PROPERTIES

        protected override IDictionary<string, Expression<Func<User, object>>> PropertySelectors { get; } =
            new Dictionary<string, Expression<Func<User, object>>>
            {
                {nameof(Models.User.Id), x => x.Id},
                {nameof(Models.User.UserName), x => x.UserName},
                {nameof(Models.User.Email), x => x.Email},
                {nameof(Models.User.UserType), x => x.UserType},
                {nameof(Models.User.Residents), x => x.Residents},
            };

        #endregion PROPERTIES


        #region METHDOS

        #region create

        [Authorize(EUserType.SysAdmin)]
        [HttpPost(Routes.RestBase.Create)]
        public override async Task<string> CreateAsync([FromBody] User item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            item.Group = item.Group?.ToUpper();
            return await base.CreateAsync(item);
        }

        [Authorize(EUserType.SysAdmin)]
        [HttpPost(Routes.RestBase.AddItemToList)]
        public override async Task<StatusCodeResult> AddItemToListAsync(string id, string propertyName,
            [FromBody] string jsonValue)
        {
            if (propertyName?.EqualsWithCamelCasing(nameof(Models.User.Residents)) != true)
                return await base.AddItemToListAsync(id, propertyName, jsonValue);

            var residentId = jsonValue.ToObjectId();
            var userId = id.ToObjectId();

            await DataService.AddItemToListProperty(userId, x => x.Residents, residentId);
            return StatusCode((int) HttpStatusCode.Created);
        }

        #endregion create

        #region read

        [Authorize(EUserType.SysAdmin)]
        [HttpGet(Routes.RestBase.GetAll)]
        public override Task<IEnumerable<User>> GetAllAsync(string[] propertiesToInclude)
            => base.GetAllAsync(propertiesToInclude);

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User)]
        [HttpGet(Routes.RestBase.GetOne)]
        public override async Task<User> GetOneAsync(string id, string[] propertiesToInclude)
        {
            switch (await GetPropertyOfCurrentUser(x => x.UserType))
            {
                case EUserType.SysAdmin:
                    break;
                default:
                    if (id.ToObjectId() != CurrentUserId)
                        throw new NotFoundException<User>(nameof(Models.User.Id), id);
                    break;
            }

            return await base.GetOneAsync(id, propertiesToInclude);
        }

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User)]
        [HttpGet(Routes.RestBase.GetProperty)]
        public override async Task<object> GetPropertyAsync(string id, string propertyName)
        {
            switch (await GetPropertyOfCurrentUser(x => x.UserType))
            {
                case EUserType.SysAdmin:
                    break;
                default:
                    if (id.ToObjectId() != CurrentUserId)
                        throw new NotFoundException<User>(nameof(Models.User.Id), id);
                    break;
            }

            return await base.GetPropertyAsync(id, propertyName);
        }

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User, EUserType.Module)]
        [HttpGet(Routes.Users.GetByName)]
        public async Task<User> GetByNameAsync(string userName, string[] propertiesToInclude)
        {
            var user = await GetCurrentUser(
                new Expression<Func<User, object>>[]
                {
                    x => x.UserName,
                    x => x.UserType
                });

            switch (user.UserType)
            {
                case EUserType.SysAdmin:
                    break;
                default:
                    if (user.UserName != userName)
                        throw new NotFoundException<User>(nameof(Models.User.UserName), userName);
                    break;
            }

            var selectors = !EnumerableExtensions.IsNullOrEmpty(propertiesToInclude)
                ? ConvertStringsToSelectors(propertiesToInclude)
                : null;

            return await _usersService.GetByNameAsync(userName, selectors);
        }

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User, EUserType.Module)]
        [HttpGet(Routes.Users.GetPropertyByName)]
        public async Task<object> GetPropertyByNameAsync(string userName, string propertyName)
        {
            var user = await GetCurrentUser();

            switch (user.UserType)
            {
                case EUserType.SysAdmin:
                    break;
                default:
                    if (user.UserName != userName)
                        throw new NotFoundException<User>(nameof(user.UserName), userName);
                    break;
            }

            if (!typeof(User).GetProperties().Any(x => x.Name.EqualsWithCamelCasing(propertyName)))
                throw new PropertyNotFoundException<User>(propertyName);

            return await _usersService.GetPropertyByNameAsync(
                userName, PropertySelectors[propertyName.ToUpperCamelCase()]);
        }

        #endregion read

        #region update

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpPut(Routes.RestBase.Update)]
        public override async Task UpdateAsync([FromBody] User item, [FromQuery] string[] properties)
        {
            if (item == null)
                throw new ArgumentException("the item to update cannot be null");
            
            switch (await GetPropertyOfCurrentUser(x => x.UserType))
            {
                case EUserType.SysAdmin:
                    break;
                default:
                    if (CurrentUserId != item.Id)
                        throw new NotFoundException<User>(nameof(Models.User.Id), item.Id.ToString());
                    if (properties.Any(x => x.EqualsWithCamelCasing(nameof(Models.User.UserType))))
                        throw new UnauthorizedException(EUserType.SysAdmin);
                    break;
            }

            if (properties.Any(x => x.EqualsWithCamelCasing(nameof(Models.User.Group))))
                item.Group = item.Group?.ToUpper();

            if (properties.All(x => !x.EqualsWithCamelCasing(nameof(Models.User.Password))))
            {
                await base.UpdateAsync(item, properties);
                return;
            }

            if (properties.Length > 1)
            {
                var propertyList = properties.ToList();
                propertyList.Remove(x => x.EqualsWithCamelCasing(nameof(Models.User.Password)));

                await base.UpdateAsync(item, propertyList.ToArray());
            }

            var user = await DataService.GetOneAsync(item.Id);
            user.Password = item.Password.Hash(user.Id);
            await DataService.UpdatePropertyAsync(user.Id, x => x.Password, user.Password);
        }

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpPut(Routes.RestBase.UpdateProperty)]
        public override async Task UpdatePropertyAsync(string id, string propertyName, [FromQuery] string jsonValue)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                throw new NotFoundException<User>(nameof(IModelWithID.Id), id);

            switch (await GetPropertyOfCurrentUser(x => x.UserType))
            {
                case EUserType.SysAdmin:
                    break;
                default:
                    if (CurrentUserId != objectId)
                        throw new NotFoundException<User>(nameof(Models.User.Id), objectId.ToString());
                    if (propertyName.EqualsWithCamelCasing(nameof(Models.User.UserType)))
                        throw new UnauthorizedException(EUserType.SysAdmin);
                    break;
            }

            if (!propertyName.EqualsWithCamelCasing(nameof(Models.User.Password)))
                await base.UpdatePropertyAsync(id, propertyName, jsonValue);
            else
                await ((IUsersService) DataService).UpdatePasswordAsync(objectId, jsonValue);
        }

        #endregion update

        #region delete

        [Authorize(EUserType.SysAdmin)]
        [HttpDelete(Routes.RestBase.Delete)]
        public override async Task DeleteAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                throw new NotFoundException<User>(nameof(IModelWithID.Id), id);

            if (objectId == CurrentUserId)
                throw new ArgumentException("You cannot delete yourself", nameof(id));

            await base.DeleteAsync(id);
        }

        [Authorize(EUserType.SysAdmin)]
        [HttpDelete(Routes.Users.RemoveResident)]
        public async Task RemoveResident(string id, string residentId)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                throw new NotFoundException<User>(nameof(IModelWithID.Id), id);
            if (!ObjectId.TryParse(residentId, out var residentobjectId))
                throw new NotFoundException<Resident>(nameof(IModelWithID.Id), residentId);

            await DataService.RemoveItemFromList(objectId, x => x.Residents, residentobjectId);
        }

        [Authorize(EUserType.SysAdmin)]
        [HttpDelete(Routes.Users.ClearResidents)]
        public async Task ClearResidents(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                throw new NotFoundException<User>(nameof(IModelWithID.Id), id);

            await DataService.UpdatePropertyAsync(objectId, x => x.Residents, new List<ObjectId>());
        }

        #endregion delete

        #endregion METHODS
    }
}