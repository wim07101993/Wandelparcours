using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
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
    public abstract class ARestControllerBase<T> : AControllerBase, IRestController<T>
        where T : IModelWithID
    {
        #region FIELDS

        protected readonly IDataService<T> DataService;
        protected readonly ILogger Logger;

        #endregion FIELDS


        #region CONSTRUCTORS

        protected ARestControllerBase(IDataService<T> dataService, ILogger logger, IUsersService usersService)
            : base(usersService)
        {
            DataService = dataService;
            Logger = logger;
        }

        #endregion CONSTRUCTORS


        #region PROPERTIES

        protected abstract IDictionary<string, Expression<Func<T, object>>> PropertySelectors { get; }

        #endregion PROPERTIES


        #region METHODS

        protected IEnumerable<Expression<Func<T, object>>> ConvertStringsToSelectors(IEnumerable<string> propertyNames)
            => propertyNames
                .Select(
                    x => PropertySelectors.FirstOrDefault(y => y.Key.EqualsWithCamelCasing(x)).Value
                         ?? throw new PropertyNotFoundException<T>(x));

        #region create

        public virtual async Task<string> CreateAsync(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            await DataService.CreateAsync(item);
            return item.Id.ToString();
        }

        public virtual async Task<StatusCodeResult> AddItemToListAsync(string id, string propertyName, string jsonValue)
        {
            var property = typeof(T)
                .GetProperties()
                .FirstOrDefault(
                    x => x.Name.EqualsWithCamelCasing(propertyName)
                         && x.PropertyType.IsGenericType
                         && typeof(IEnumerable).IsAssignableFrom(x.PropertyType));

            if (property == null)
                throw new PropertyNotFoundException<T>(nameof(propertyName));

            var valueType = property.PropertyType.GetGenericArguments()[0];
            var objectId = id.ToObjectId();

            try
            {
                var value = JsonConvert.DeserializeObject(jsonValue, valueType);
                await DataService.AddItemToListProperty(
                    objectId,
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

        public virtual async Task<IEnumerable<T>> GetAllAsync(string[] propertiesToInclude)
        {
            if (EnumerableExtensions.IsNullOrEmpty(propertiesToInclude))
                return await DataService.GetAsync();
            
            var selectors = ConvertStringsToSelectors(propertiesToInclude);
            return await DataService.GetAsync(selectors);
        }

        public virtual async Task<T> GetOneAsync(string id, string[] propertiesToInclude)
        {
            var objectId = id.ToObjectId();

            T item;
            if (EnumerableExtensions.IsNullOrEmpty(propertiesToInclude))
                item = await DataService.GetOneAsync(objectId);
            else
            {
                var selectors = ConvertStringsToSelectors(propertiesToInclude);
                item = await DataService.GetOneAsync(objectId, selectors);
            }

            return Equals(item, default(T))
                ? throw new NotFoundException<T>(nameof(IModelWithID.Id), id)
                : item;
        }

        public virtual async Task<object> GetPropertyAsync(string id, string propertyName)
        {
            if (!typeof(T).GetProperties().Any(x => x.Name.EqualsWithCamelCasing(propertyName)))
                throw new PropertyNotFoundException<T>(propertyName);

            var objectId = id.ToObjectId();
            var selector = PropertySelectors[propertyName.ToUpperCamelCase()];
            return await DataService.GetPropertyAsync(objectId,selector);
        }

        #endregion read


        #region update

        public virtual async Task UpdateAsync(T item, string[] properties)
        {
            if (EnumerableExtensions.IsNullOrEmpty(properties))
                await DataService.UpdateAsync(item);
            else
            {
                var selectors = ConvertStringsToSelectors(properties);
                await DataService.UpdateAsync(item, selectors);
            }
        }

        public virtual async Task UpdatePropertyAsync(string id, string propertyName, string jsonValue)
        {
            var propertyInfo = typeof(T)
                .GetProperties()
                .FirstOrDefault(x => x.Name.EqualsWithCamelCasing(propertyName));

            if (propertyInfo == null)
                throw new PropertyNotFoundException<T>(nameof(propertyName));

            object value;
            try
            {
                // try to convert the jsonValue to the type of the property       
                value = typeof(string) == propertyInfo.PropertyType
                    ? jsonValue
                    : JsonConvert.DeserializeObject(jsonValue, propertyInfo.PropertyType);
            }
            catch (JsonException)
            {
                throw new WrongArgumentTypeException(jsonValue, propertyInfo.PropertyType);
            }

            var objectId = id.ToObjectId();
            var property = PropertySelectors[propertyName.ToUpperCamelCase()];
            await DataService.UpdatePropertyAsync(objectId, property, value);
        }

        #endregion update


        #region delete

        public virtual async Task DeleteAsync(string id)
        {
            var objectId = id.ToObjectId();
            await DataService.RemoveAsync(objectId);
        }

        #endregion delete

        #endregion METHOD
    }
}