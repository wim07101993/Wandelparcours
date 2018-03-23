using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models.Bases;
using WebService.Services.Data;
using WebService.Services.Exceptions;
using WebService.Services.Logging;

namespace WebService.Controllers.Bases
{
    /// <inheritdoc cref="Controller" />
    /// <inheritdoc cref="IRestController{T}" />
    /// <summary>
    /// ARestControllerBase is an abstract class that holds the methods to Get, Create, Delete and Update data to a database.
    /// </summary>
    /// <typeparam name="T">is the type of the data to handle</typeparam>
    public abstract class ARestControllerBase<T> : Controller, IRestController<T> where T : IModelWithID
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

        protected readonly IThrow Throw;

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
        /// <param name="iThrow"></param>
        /// <param name="dataService">is a service to handle the database connection</param>
        /// <param name="logger">is a service to handle the logging of messages</param>
        protected ARestControllerBase(IThrow iThrow, IDataService<T> dataService, ILogger logger)
        {
            // initiate the services
            Throw = iThrow;
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
        /// <exception cref="WebArgumentException">When the property could not be converted to a selector</exception>
        public IEnumerable<Expression<Func<T, object>>> ConvertStringsToSelectors(IEnumerable<string> propertyNames)
            => propertyNames
                .Select(x =>
                {
                    var selector = PropertySelectors.FirstOrDefault(y => y.Key.EqualsWithCamelCasing(x)).Value;
                    if (selector != null)
                        return selector;

                    Throw.PropertyNotKnown<T>(x);
                    return null;
                });

        #region create

        /// <inheritdoc cref="IRestController{T}.CreateAsync" />
        /// <summary>
        /// Create is supposed to save the passed <see cref="T"/> to the database.
        /// </summary>
        /// <param name="item">is the <see cref="T"/> to save in the database</param>
        /// <exception cref="Exception">When the item could not be created</exception>
        [HttpPost(CreateTemplate)]
        public virtual async Task<StatusCodeResult> CreateAsync([FromBody] T item)
        {
            if (item == null)
            {
                Throw.NullArgument("item");
                return null;
            }

            // use the data service to create a new item
            await DataService.CreateAsync(item);
            return StatusCode((int) HttpStatusCode.Created);
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

            // check if the property exists on the item
            if (property == null)
                throw new WebArgumentException(
                    $"Property {propertyName} cannot be found on {typeof(T).Name}", nameof(propertyName));

            // parse the id
            if (!ObjectId.TryParse(id, out var objectId))
                // if it fails, throw not found exception
                throw new NotFoundException($"The {typeof(T).Name} with id {id} could not be found");

            var valueType = property.PropertyType.GetGenericArguments()[0];

            try
            {
                var value = JsonConvert.DeserializeObject(jsonValue, valueType);
                await DataService.AddItemToListProperty(objectId,
                    PropertySelectors[propertyName.ToUpperCamelCase()] as Expression<Func<T, IEnumerable<object>>>,
                    value);
                return StatusCode((int) HttpStatusCode.Created);
            }
            catch (Exception)
            {
                Throw.WrongTypeArgument(valueType, null);
                return null;
            }
        }

        #endregion create

        #region read

        /// <inheritdoc cref="IRestController{T}.GetAsync(string[])" />
        /// <summary>
        /// Get is supposed to return all the Items in the database. 
        /// To limit data traffic it is possible to select only a number of properties.
        /// <para/>
        /// By default only the properties in the selector <see cref="PropertiesToSendOnGetAll"/> are returned.
        /// </summary>
        /// <param name="propertiesToInclude">are the properties of which the values should be returned</param>
        /// <returns>All <see cref="T"/>s in the database but only the given properties are filled in</returns>
        /// <exception cref="WebArgumentException">When one ore more properties could not be converted to selectors</exception>
        [HttpGet(GetAllTemplate)]
        public virtual async Task<IEnumerable<T>> GetAllAsync([FromQuery] string[] propertiesToInclude)
        {
            // convert the property names to selectors, if there are any
            var selectors = !EnumerableExtensions.IsNullOrEmpty(propertiesToInclude)
                ? ConvertStringsToSelectors(propertiesToInclude)
                : PropertiesToSendOnGetAll;

            // return the items got from the data service
            return await DataService.GetAsync(selectors);
        }

        /// <inheritdoc cref="IRestController{T}.GetAsync(string, string[])" />
        /// <summary>
        /// Get is supposed to return the <see cref="T"/> with the given id in the database. 
        /// To limit data traffic it is possible to select only a number of properties.
        /// <para/>
        /// By default all properties are returned.
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/> to get</param>
        /// <param name="propertiesToInclude">are the properties of which the values should be returned</param>
        /// <returns>The <see cref="T"/> in the database that has the given id</returns>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="T"/> not found</exception>
        /// <exception cref="WebArgumentException">When one ore more properties could not be converted to selectors</exception>
        [HttpGet(GetOneTemplate)]
        public virtual async Task<T> GetOneAsync(string id, [FromQuery] string[] propertiesToInclude)
        {
            // parse the id
            if (!ObjectId.TryParse(id, out var objectId))
                // if it fails, throw not found exception
                throw new NotFoundException($"The {typeof(T).Name} with id {id} could not be found");

            // convert the property names to selectors, if there are any
            var selectors = !EnumerableExtensions.IsNullOrEmpty(propertiesToInclude)
                ? ConvertStringsToSelectors(propertiesToInclude)
                : null;

            // get the jsonValue from the data service
            var item = await DataService.GetAsync(objectId, selectors);

            return Equals(item, default(T))
                // if the item is null, throw a not found exception
                ? throw new NotFoundException($"The {typeof(T).Name} with id {id} could not be found")
                // else return the values
                : item;
        }

        /// <inheritdoc cref="IRestController{T}.GetPropertyAsync" />
        /// <summary>
        /// GetProperty returns the jsonValue of the asked property of the asked <see cref="T"/>.
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/></param>
        /// <param name="propertyName">is the name of the property to return</param>
        /// <returns>The jsonValue of the asked property</returns>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="T"/> not found</exception>
        /// <exception cref="WebArgumentException">When the property could not be found on <see cref="T"/></exception>
        [HttpGet(GetPropertyTemplate)]
        public virtual async Task<object> GetPropertyAsync(string id, string propertyName)
        {
            // check if the property exists on the item
            if (!typeof(T).GetProperties().Any(x => x.Name.EqualsWithCamelCasing(propertyName)))
                throw new WebArgumentException(
                    $"Property {propertyName} cannot be found on {typeof(T).Name}", nameof(propertyName));

            // parse the id
            if (!ObjectId.TryParse(id, out var objectId))
                // if it fails, throw not found exception
                throw new NotFoundException($"The {typeof(T).Name} with id {id} could not be found");

            // get the property from the database
            return await DataService.GetPropertyAsync(objectId, PropertySelectors[propertyName.ToUpperCamelCase()]);
        }

        #endregion read

        #region update

        /// <inheritdoc cref="IRestController{T}.UpdateAsync" />
        /// <summary>
        /// Update updates the fields of the <see cref="T"/> that are specified in the <see cref="properties"/> parameter.
        /// If the item doesn't exist, a new is created in the database.
        /// <para/>
        /// By default all properties are updated.
        /// </summary>
        /// <param name="item">is the <see cref="T"/> to update</param>
        /// <param name="properties">contains the properties that should be updated</param>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="T"/> not found</exception>
        /// <exception cref="WebArgumentException">When one ore more properties could not be converted to selectors</exception>
        [HttpPut(UpdateTemplate)]
        public virtual async Task UpdateAsync([FromBody] T item, [FromQuery] string[] properties)
        {
            // convert the property names to selectors, if there are any
            var selectors = !EnumerableExtensions.IsNullOrEmpty(properties)
                ? ConvertStringsToSelectors(properties)
                : null;

            // update the item in the database
            await DataService.UpdateAsync(item, selectors);
        }

        /// <inheritdoc cref="IRestController{T}.UpdatePropertyAsync"/>
        /// <summary>
        /// UpdatePropertyAsync is supposed to update the jsonValue of the asked property of the asked <see cref="T"/>.
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/></param>
        /// <param name="propertyName">is the name of the property to update</param>
        /// <param name="jsonValue">is the new jsonValue of the property</param>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="T"/> not found</exception>
        /// <exception cref="WebArgumentException">When the property could not be found on <see cref="T"/> or the jsonValue could not be assigned</exception>
        /// <exception cref="Exception">When the update failed</exception>
        [HttpPut(UpdatePropertyTemplate)]
        public virtual async Task UpdatePropertyAsync(string id, string propertyName, [FromBody] string jsonValue)
        {
            var property = typeof(T)
                .GetProperties()
                .FirstOrDefault(propertyInfo => propertyInfo.Name.EqualsWithCamelCasing(propertyName));

            // check if the property exists on the item
            if (property == null)
                throw new WebArgumentException(
                    $"Property {propertyName} cannot be found on {typeof(T).Name}", nameof(propertyName));

            object value;
            try
            {
                // try to convert the jsonValue to the type of the property
                value = JsonConvert.DeserializeObject(jsonValue, property.PropertyType);
            }
            catch (JsonException)
            {
                // if it fails, throw web argument exception
                throw new WebArgumentException(
                    $"The passed jsonValue is not assignable to the property {propertyName} of type {typeof(T).Name}",
                    jsonValue);
            }


            if (!ObjectId.TryParse(id, out var objectId))
                // if it fails, throw not found exception
                throw new NotFoundException($"The {typeof(T).Name} with id {id} could not be found");

            // update the property int the database
            await DataService.UpdatePropertyAsync(objectId, PropertySelectors[propertyName], value);
        }

        #endregion update

        #region delete

        /// <inheritdoc cref="IRestController{T}.DeleteAsync" />
        /// <summary>
        /// Delete is supposed to remove the <see cref="T"/> with the passed id from the database.
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/> to remove from the database</param>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="T"/> not found</exception>
        [HttpDelete(DeleteTemplate)]
        public virtual async Task DeleteAsync(string id)
        {
            // parse the id
            if (!ObjectId.TryParse(id, out var objectId))
                // if it fails, throw not found exception
                throw new NotFoundException($"The {typeof(T).Name} with id {id} could not be found");

            // use the data service to remove the item
            await DataService.RemoveAsync(objectId);
        }

        #endregion delete

        #endregion METHOD
    }
}