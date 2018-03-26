using System;
using System.Collections.Generic;
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
        #region FIELDS

        private readonly IUsersService _usersService;

        #endregion FIELDS


        #region CONSTRUCTOR

        protected AControllerBase(IUsersService usersService)
        {
            _usersService = usersService;
        }

        #endregion CONSTRUCTRO


        #region PROPERTIES

        public ObjectId UserId { get; set; }

        #endregion PROPERTIES


        #region METHODS

        protected async Task<User> GetCurrentUser(IEnumerable<Expression<Func<User, object>>> propertiesToSelect = null)
            => await _usersService.GetOneAsync(UserId, propertiesToSelect);

        protected async Task<T> GetPropertyOfCurrentUser<T>(Expression<Func<User, T>> propertyToGet)
            => await _usersService.GetPropertyAsync(UserId, propertyToGet);

        #endregion METHODS
    }
}