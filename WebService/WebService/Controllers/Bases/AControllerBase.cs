using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebService.Models;
using WebService.Services.Data;

namespace WebService.Controllers.Bases
{
    public abstract class AControllerBase : Controller, IController
    {
        private readonly IUsersService _usersService;


        protected AControllerBase(IUsersService usersService)
        {
            _usersService = usersService;
            // disables auth
//            UserId = _usersService
//                         .GetAsync()
//                         .Result
//                         .FirstOrDefault(x => x.UserType == EUserType.SysAdmin)
//                         ?.Id
//                     ?? ObjectId.Empty;
        }


        public ObjectId CurrentUserId { get; set; }


        protected async Task<User> GetCurrentUser(IEnumerable<Expression<Func<User, object>>> propertiesToSelect = null)
            => await _usersService.GetOneAsync(CurrentUserId, propertiesToSelect);

        protected async Task<T> GetPropertyOfCurrentUser<T>(Expression<Func<User, T>> propertyToGet)
            => await _usersService.GetPropertyAsync(CurrentUserId, propertyToGet);
    }
}