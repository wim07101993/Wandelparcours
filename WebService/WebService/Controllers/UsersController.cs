﻿using System;
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

        protected override IEnumerable<Expression<Func<User, object>>> PropertiesToSendOnGetAll { get; } =
            new Expression<Func<User, object>>[]
            {
                x => x.Id,
                x => x.UserName,
                x => x.Email,
                x => x.UserType
            };

        protected override IDictionary<string, Expression<Func<User, object>>> PropertySelectors { get; } =
            new Dictionary<string, Expression<Func<User, object>>>
            {
                {nameof(Models.User.Id), x => x.Id},
                {nameof(Models.User.UserName), x => x.UserName},
                {nameof(Models.User.Email), x => x.Email},
                {nameof(Models.User.Password), x => x.Password},
                {nameof(Models.User.UserType), x => x.UserType},
                {nameof(Models.User.Residents), x => x.Residents},
            };

        #endregion PROPERTIES


        #region METHDOS

        #region create

        [Authorize(EUserType.SysAdmin, EUserType.Nurse)]
        [HttpPost(Routes.RestBase.Create)]
        public override async Task<string> CreateAsync([FromBody] User item)
        {
            return await base.CreateAsync(item);
        }

        [Authorize(EUserType.SysAdmin, EUserType.Nurse)]
        [HttpPost(Routes.RestBase.AddItemToList)]
        public override async Task<StatusCodeResult> AddItemToListAsync(string id, string propertyName,
            [FromBody] string jsonValue)
        {
            if (propertyName?.EqualsWithCamelCasing(nameof(Models.User.Residents)) == true)
            {
                var residentId = jsonValue.ToObjectId();
                var userId = id.ToObjectId();

                await DataService.AddItemToListProperty(userId, x => x.Residents, residentId);
                return StatusCode((int) HttpStatusCode.Created);
            }
            else
                return await base.AddItemToListAsync(id, propertyName, jsonValue);
        }

        #endregion create

        #region read

        [Authorize(EUserType.SysAdmin)]
        [HttpGet(Routes.RestBase.GetAll)]
        public override Task<IEnumerable<User>> GetAllAsync(string[] propertiesToInclude)
            => base.GetAllAsync(propertiesToInclude);

        [Authorize(EUserType.SysAdmin, EUserType.User)]
        [HttpGet(Routes.RestBase.GetOne)]
        public override Task<User> GetOneAsync(string id, string[] propertiesToInclude)
            => base.GetOneAsync(id, propertiesToInclude);

        [Authorize(EUserType.SysAdmin, EUserType.User)]
        [HttpGet(Routes.RestBase.GetProperty)]
        public override Task<object> GetPropertyAsync(string id, string propertyName)
            => base.GetPropertyAsync(id, propertyName);

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User, EUserType.Module)]
        [HttpGet(Routes.Users.GetByName)]
        public async Task<User> GetByNameAsync(string userName, string[] propertiesToInclude)
        {
            var user = await GetCurrentUser();

            if (user.UserType != EUserType.SysAdmin && user.UserName != userName)
                throw new NotFoundException<User>(nameof(user.UserName), userName);

            var selectors = !EnumerableExtensions.IsNullOrEmpty(propertiesToInclude)
                ? ConvertStringsToSelectors(propertiesToInclude)
                : PropertiesToSendOnGetAll;

            return await _usersService.GetByNameAsync(userName, selectors);
        }

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User, EUserType.Module)]
        [HttpGet(Routes.Users.GetPropertyByName)]
        public async Task<object> GetPropertyByNameAsync(string userName, string propertyName)
        {
            var user = await GetCurrentUser();

            if (user.UserType != EUserType.SysAdmin && user.UserName != userName)
                throw new NotFoundException<User>(nameof(user.UserName), userName);

            if (!typeof(User).GetProperties().Any(x => x.Name.EqualsWithCamelCasing(propertyName)))
                throw new PropertyNotFoundException<User>(propertyName);

            return await _usersService.GetPropertyByNameAsync(
                userName, PropertySelectors[propertyName.ToUpperCamelCase()]);
        }

        #endregion read

        #region update

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User)]
        [HttpPut(Routes.RestBase.Update)]
        public override async Task UpdateAsync([FromBody] User item, [FromQuery] string[] properties)
        {
            var userTypeToEdit = await _usersService.GetPropertyAsync(item.Id, x => x.UserType);
            var currentUserType = await _usersService.GetPropertyAsync(UserId, x => x.UserType);

            if (userTypeToEdit > currentUserType
                || properties.Any(x => x.EqualsWithCamelCasing(nameof(Models.User.UserType)))
                && item.UserType > currentUserType)
                throw new UnauthorizedException(item.UserType);

            if (properties.All(x => !x.EqualsWithCamelCasing(nameof(Models.User.Password))))
            {
                await base.UpdateAsync(item, properties);
                return;
            }

            if (item.Id != UserId && currentUserType == item.UserType)
                throw new UnauthorizedException("You cannot edit the password of another user with the same level.");

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
        [HttpPut(Routes.RestBase.UpdateProperty)]
        public override async Task UpdatePropertyAsync(string id, string propertyName, [FromQuery] string jsonValue)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                throw new NotFoundException<User>(nameof(IModelWithID.Id), id);

            var currentUserType = await GetPropertyOfCurrentUser(x => x.UserType);
            var userTypeToEdit = await _usersService.GetPropertyAsync(objectId, x => x.UserType);

            if (userTypeToEdit > currentUserType
                || propertyName.EqualsWithCamelCasing(nameof(Models.User.UserType))
                && userTypeToEdit > currentUserType)
                throw new UnauthorizedException(userTypeToEdit);

            if (!propertyName.EqualsWithCamelCasing(nameof(Models.User.Password)))
                await base.UpdatePropertyAsync(id, propertyName, jsonValue);
            else
            {
                if (objectId != UserId && currentUserType == userTypeToEdit)
                    throw new UnauthorizedException(
                        "You cannot edit the password of another user with the same level.");

                await ((IUsersService) DataService).UpdatePasswordAsync(objectId, jsonValue);
            }
        }

        #endregion update

        #region delete

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User)]
        [HttpDelete(Routes.RestBase.Delete)]
        public override async Task DeleteAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                throw new NotFoundException<User>(nameof(IModelWithID.Id), id);

            if (objectId == UserId)
                throw new ArgumentException("You cannot delete yourself", nameof(id));

            var currentUserType = await GetPropertyOfCurrentUser(x => x.UserType);
            var userTypeToEdit = await _usersService.GetPropertyAsync(objectId, x => x.UserType);

            if (userTypeToEdit > currentUserType || objectId != UserId && currentUserType == userTypeToEdit)
                throw new UnauthorizedException(userTypeToEdit);

            await base.DeleteAsync(id);
        }

        #endregion delete

        #endregion METHODS
    }
}