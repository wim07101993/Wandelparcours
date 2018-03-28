using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using Newtonsoft.Json;
using WebService.Helpers.Attributes;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Models.Bases;
using WebService.Services.Data;
using WebService.Services.Logging;
using ArgumentNullException = WebService.Helpers.Exceptions.ArgumentNullException;

namespace WebService.Controllers.Bases
{
    /// <inheritdoc cref="Controller" />
    /// <inheritdoc cref="IRestController{T}" />
    /// <summary>
    /// ARestControllerBase is an abstract class that holds the methods to Get, Create, Delete and Update data to a database.
    /// </summary>
    /// <typeparam name="T">is the type of the data to handle</typeparam>
    public abstract class ARestControllerBase<T> : AControllerBase, IRestController<T> where T : IModelWithID
    {
        #region FIELDS

        public const string CreateTemplate = "";
        public const string AddItemToListTemplate = "{id}/{propertyName}";

        public const string GetAllTemplate = "";
        public const string GetOneTemplate = "{id}";
        public const string GetPropertyTemplate = "{id}/{propertyName}";

        public const string UpdateTemplate = "";
        public const string UpdatePropertyTemplate = "{id}/{propertyName}";

        public const string DeleteTemplate = "{id}";

        /// <summary>
        /// _dataService is used to handle the data traffic to and from the database.
        /// </summary>
        protected readonly IDataService<T> DataService;

        /// <summary>
        /// _logger is used to handle the logging of messages.
        /// </summary>
        protected readonly ILogger Logger;

        #endregion FIELDS


        #region CONSTRUCTORS

        /// <summary>
        /// ARestControllerBase creates an instance of the <see cref="ARestControllerBase{T}"/> class. 
        /// </summary>
        /// <param name="dataService">is a service to handle the database connection</param>
        /// <param name="logger">is a service to handle the logging of messages</param>
        /// <param name="usersService">is the service to get the current user from</param>
        protected ARestControllerBase(IDataService<T> dataService, ILogger logger, IUsersService usersService) : base(
            usersService)
        {
            // initiate the services
            DataService = dataService;
            Logger = logger;
        }

        #endregion CONSTRUCTORS


        #region PROPERTIES

        /// <summary>
        /// PropertiesToSendOnGetAll are the selectors of the properties to send when all the <see cref="T"/>s are asked from the database.
        /// </summary>
        public abstract IEnumerable<Expression<Func<T, object>>> PropertiesToSendOnGetAll { get; }

        /// <summary>
        /// PropertySelectors should return an <see cref="IDictionary{TKey,TValue}"/> with as keys the property names
        /// and as values the selectors.
        /// </summary>
        public abstract IDictionary<string, Expression<Func<T, object>>> PropertySelectors { get; }

        #endregion PROPERTIES


        #region METHODS

        /// <summary>
        /// PropertySelectors converts property names to their selectors in the form of <see cref="Func{TInput,TResult}"/>
        /// </summary>
        /// <param name="propertyNames">are the property names to convert to a selector</param>
        /// <returns>An <see cref="Func{TInput,TResult}"/> that contains the converted selector</returns>
        /// <exception cref="PropertyNotFoundException{T}">When the property could not be converted to a selector</exception>
        public IEnumerable<Expression<Func<T, object>>> ConvertStringsToSelectors(IEnumerable<string> propertyNames)
            => propertyNames
                .Select(x =>
                    PropertySelectors.FirstOrDefault(y => y.Key.EqualsWithCamelCasing(x)).Value ??
                    throw new PropertyNotFoundException<T>(x));

        #region create

        [Authorize]
        [HttpPost(CreateTemplate)]
        public virtual async Task<string> CreateAsync([FromBody] T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            await DataService.CreateAsync(item);
            return item.Id.ToString();
        }

        [HttpPost(AddItemToListTemplate)]
        public virtual async Task<StatusCodeResult> AddItemToListAsync(string id, string propertyName,
            [FromBody] string jsonValue)
        {
            var property = typeof(T)
                .GetProperties()
                .FirstOrDefault(x => x.Name.EqualsWithCamelCasing(propertyName) &&
                                     x.PropertyType.IsGenericType &&
                                     typeof(IEnumerable).IsAssignableFrom(x.PropertyType));

            if (property == null)
                throw new PropertyNotFoundException<T>(nameof(propertyName));

            if (!ObjectId.TryParse(id, out var objectId))
                throw new NotFoundException<T>(nameof(IModelWithID.Id), id);

            var valueType = property.PropertyType.GetGenericArguments()[0];

            try
            {
                var value = JsonConvert.DeserializeObject(jsonValue, valueType);
                await DataService.AddItemToListProperty(objectId,
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    PropertySelectors[propertyName.ToUpperCamelCase()] as Expression<Func<T, IEnumerable<object>>>,
                    value);
                return StatusCode((int) HttpStatusCode.Created);
            }
            catch (Exception)
            {
                throw new WrongArgumentTypeException(jsonValue, valueType);
            }
        }

        #endregion create

        #region read

        [HttpGet(GetAllTemplate)]
        public virtual async Task<IEnumerable<T>> GetAllAsync([FromQuery] string[] propertiesToInclude)
        {
            var selectors = !EnumerableExtensions.IsNullOrEmpty(propertiesToInclude)
                ? ConvertStringsToSelectors(propertiesToInclude)
                : PropertiesToSendOnGetAll;

            return await DataService.GetAsync(selectors);
        }

        [HttpGet(GetOneTemplate)]
        public virtual async Task<T> GetOneAsync(string id, [FromQuery] string[] propertiesToInclude)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                throw new NotFoundException<T>(nameof(IModelWithID.Id), id);

            var selectors = !EnumerableExtensions.IsNullOrEmpty(propertiesToInclude)
                ? ConvertStringsToSelectors(propertiesToInclude)
                : null;

            var item = await DataService.GetOneAsync(objectId, selectors);

            return Equals(item, default(T))
                ? throw new NotFoundException<T>(nameof(IModelWithID.Id), id)
                : item;
        }

        [HttpGet(GetPropertyTemplate)]
        public virtual async Task<object> GetPropertyAsync(string id, string propertyName)
        {
            if (!typeof(T).GetProperties().Any(x => x.Name.EqualsWithCamelCasing(propertyName)))
                throw new PropertyNotFoundException<T>(nameof(propertyName));

            if (!ObjectId.TryParse(id, out var objectId))
                throw new NotFoundException<T>(nameof(IModelWithID.Id), id);

            return await DataService.GetPropertyAsync(objectId, PropertySelectors[propertyName.ToUpperCamelCase()]);
        }

        #endregion read

        #region update

        [HttpPut(UpdateTemplate)]
        public virtual async Task UpdateAsync([FromBody] T item, [FromQuery] string[] properties)
        {
            var selectors = !EnumerableExtensions.IsNullOrEmpty(properties)
                ? ConvertStringsToSelectors(properties)
                : null;

            await DataService.UpdateAsync(item, selectors);
        }

        [HttpPut(UpdatePropertyTemplate)]
        public virtual async Task UpdatePropertyAsync(string id, string propertyName, [FromBody] string jsonValue)
        {
            var property = typeof(T)
                .GetProperties()
                .FirstOrDefault(propertyInfo => propertyInfo.Name.EqualsWithCamelCasing(propertyName));

            if (property == null)
                throw new PropertyNotFoundException<T>(nameof(propertyName));

            object value;
            try
            {
                // try to convert the jsonValue to the type of the property       
                if (typeof(String)==property.PropertyType)
                {
                    value = jsonValue;
                }
                else
                {
                    value = JsonConvert.DeserializeObject(jsonValue, property.PropertyType);
                }

            }
            catch (JsonException)
            {
                throw new WrongArgumentTypeException(jsonValue, property.PropertyType);
            }

            if (!ObjectId.TryParse(id, out var objectId))
                throw new NotFoundException<T>(nameof(IModelWithID.Id), id);

            await DataService.UpdatePropertyAsync(objectId, PropertySelectors[propertyName], value);
        }

        #endregion update

        #region delete

        [HttpDelete(DeleteTemplate)]
        public virtual async Task DeleteAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                throw new NotFoundException<T>(nameof(IModelWithID.Id), id);

            await DataService.RemoveAsync(objectId);
        }

        #endregion delete

        #endregion METHOD
    }
}