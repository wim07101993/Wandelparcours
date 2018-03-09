using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Controllers.Bases;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Data;
using WebService.Services.Logging;

namespace WebService.Controllers
{
    /// <inheritdoc cref="ARestControllerBase{T}"/>
    /// <summary>
    /// ReceiverModulesController handles the reading and writing of receiver module data to the database.
    /// </summary>
    [Route("api/v1/[controller]")]
    public class ReceiverModulesController : ARestControllerBase<ReceiverModule>, IReceiverModulesController
    {
        #region CONSTRUCTOR

        /// <inheritdoc cref="ARestControllerBase{T}" />
        /// <summary>
        /// Residentscontroller creates an instance of the <see cref="ReceiverModulesController" /> class. 
        /// </summary>
        /// <param name="dataService">is a service to handle the database connection</param>
        /// <param name="logger">is a service to hanlde the logging of messages</param>
        public ReceiverModulesController(IDataService<ReceiverModule> dataService, ILogger logger)
            : base(dataService, logger)
        {
        }

        #endregion CONSTRUCTOR


        #region PROPERTIES

        /// <inheritdoc cref="ARestControllerBase{T}.PropertiesForSmallDataTraffic" />
        /// <summary>
        /// SmallDataProperties is a collection of expressions to select the properties that
        /// consume the least space (in this case all of them => value is null).
        /// </summary>
        public override IEnumerable<Expression<Func<ReceiverModule, object>>> PropertiesForSmallDataTraffic { get; } = null;

        #endregion PROPERTIES


        #region METHODS

        /// <inheritdoc cref="ARestControllerBase{T}.ConvertStringToSelector" />
        /// <summary>
        /// ConvertStringToSelector converts a property name to it's selector in the form of a <see cref="Func{TInput,TResult}"/>
        /// </summary>
        /// <param name="propertyName">is the property name to convert to a selector</param>
        /// <returns>An <see cref="Func{TInput,TResult}"/> that contains the converted selector</returns>
        /// <exception cref="WebArgumentException">When the property could not be converted to a selector</exception>
        public override Expression<Func<ReceiverModule, object>> ConvertStringToSelector(string propertyName)
        {
            // if the name of a properties matches a property of a Value, 
            // add the corresponding selector
            if (propertyName.EqualsWithCamelCasing(nameof(ReceiverModule.IsActive)))
                return x => x.IsActive;
            if (propertyName.EqualsWithCamelCasing(nameof(ReceiverModule.Mac)))
                return x => x.Mac;
            if (propertyName.EqualsWithCamelCasing(nameof(ReceiverModule.Position)))
                return x => x.Position;
            if (propertyName.EqualsWithCamelCasing(nameof(ReceiverModule.Id)))
                return x => x.Id;

            throw new WebArgumentException(
                $"Property {propertyName} cannot be found on {typeof(ReceiverModule).Name}", nameof(propertyName));
        }

        #region post (create)

        /// <inheritdoc cref="ARestControllerBase{T}.CreateAsync" />
        /// <summary>
        /// Create is supposed to save the passed <see cref="ReceiverModule"/> to the database.
        /// </summary>
        /// <param name="item">is the <see cref="ReceiverModule"/> to save in the database</param>
        /// <exception cref="Exception">When the item could not be created</exception>
        [HttpPost]
        public override async Task CreateAsync([FromBody] ReceiverModule item)
        {
            item.Position.TimeStamp=DateTime.Now;
            await base.CreateAsync(item);    
        }

        #endregion post (create)

        #region get (read)

        /// <inheritdoc cref="ARestControllerBase{T}.GetPropertyAsync" />
        /// <summary>
        /// GetProperty returns the valu of the asked property of the asked <see cref="ReceiverModule"/>.
        /// </summary>
        /// <param name="id">is the id of the <see cref="ReceiverModule"/></param>
        /// <param name="propertyName">is the name of the property to return</param>
        /// <returns>The value of the asked property</returns>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="ReceiverModule"/> not found</exception>
        /// <exception cref="WebArgumentException">When the property could not be found on <see cref="ReceiverModule"/></exception>
        [HttpGet("{id}/{propertyName}")]
        public override async Task<object> GetPropertyAsync(string id, string propertyName)
            => await base.GetPropertyAsync(id, propertyName);

        /// <inheritdoc cref="ARestControllerBase{T}.GetAsync(string, string[])" />
        /// <summary>
        /// Get is supposed to return the <see cref="ReceiverModule"/> with the given MAC-address in the database. 
        /// To limit data traffic it is possible to select only a number of properties.
        /// <para/>
        /// By default all properties are returned.
        /// </summary>
        /// <param name="mac">is the MAC-address of the <see cref="ReceiverModule"/> to get</param>
        /// <param name="propertiesToInclude">are the properties of which the values should be returned</param>
        /// <returns>The <see cref="ReceiverModule"/> in the database that has the given MAC-address</returns>
        /// <exception cref="NotFoundException">When the MAC-address cannot be parsed or <see cref="ReceiverModule"/> not found</exception>
        /// <exception cref="WebArgumentException">When one ore more properties could not be converted to selectors</exception>
        [HttpGet("{mac}")]
        public override async Task<ReceiverModule> GetAsync(string mac, [FromQuery] string[] propertiesToInclude)
        {
            // convert the property names to selectors, if there are any
            var selectors = !EnumerableExtensions.IsNullOrEmpty(propertiesToInclude)
                ? ConvertStringsToSelectors(propertiesToInclude)
                : null;

            // get the value from the data service
            return await ((IReceiverModuleService) DataService).GetAsync(mac, selectors)
                   ?? throw new NotFoundException(
                       $"The {typeof(ReceiverModule).Name} with id {mac} could not be found");
        }

        /// <inheritdoc cref="ARestControllerBase{T}.GetAsync(string[])" />
        /// <summary>
        /// Get is supposed to return all the Items in the database. 
        /// To limit data traffic it is possible to select only a number of propertie.
        /// <para/>
        /// By default only the properties in the selector <see cref="PropertiesForSmallDataTraffic"/> are returned.
        /// </summary>
        /// <param name="propertiesToInclude">are the properties of which the values should be returned</param>
        /// <returns>All <see cref="ReceiverModule"/>s in the database but only the given properties are filled in</returns>
        /// <exception cref="WebArgumentException">When one ore more properties could not be converted to selectors</exception>
        [HttpGet]
        public override async Task<IEnumerable<ReceiverModule>> GetAsync([FromQuery] string[] propertiesToInclude)
            => await base.GetAsync(propertiesToInclude);

        #endregion get (read)

        #region put (update)

        /// <inheritdoc cref="ARestControllerBase{T}.UpdateAsync" />
        /// <summary>
        /// Update updates the fields of the <see cref="ReceiverModule"/> that are specified in the <see cref="propertiesToUpdate"/> parameter.
        /// If the item doesn't exist, a new is created in the database.
        /// <para/>
        /// By default all properties are updated.
        /// </summary>
        /// <param name="item">is the <see cref="ReceiverModule"/> to update</param>
        /// <param name="propertiesToUpdate">contains the properties that should be updated</param>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="ReceiverModule"/> not found</exception>
        /// <exception cref="WebArgumentException">When one ore more properties could not be converted to selectors</exception>
        [HttpPut]
        public override async Task UpdateAsync([FromBody] ReceiverModule item, [FromQuery] string[] propertiesToUpdate)
            => await base.UpdateAsync(item, propertiesToUpdate);

        /// <inheritdoc cref="IRestController{T}.UpdatePropertyAsync"/>
        /// <summary>
        /// UpdatePropertyAsync is supposed to update the jsonValue of the asked property of the asked <see cref="ReceiverModule"/>.
        /// </summary>
        /// <param name="id">is the id of the <see cref="ReceiverModule"/></param>
        /// <param name="propertyName">is the name of the property to update</param>
        /// <param name="jsonValue">is the new jsonValue of the property</param>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="ReceiverModule"/> not found</exception>
        /// <exception cref="WebArgumentException">When the property could not be found on <see cref="ReceiverModule"/> or the jsonValue could not be assigned</exception>
        /// <exception cref="Exception">When the update failed</exception>
        [HttpPut("{id}/{propertyName}")]
        public override async Task UpdatePropertyAsync(string id, string propertyName, [FromBody] string jsonValue)
            => await base.UpdatePropertyAsync(id, propertyName, jsonValue);

        #endregion put (update)

        #region delete

        /// <inheritdoc cref="ARestControllerBase{T}.DeleteAsync" />
        /// <summary>
        /// Delete is supposed to remove the <see cref="ReceiverModule"/> with the passed MAC-address from the database.
        /// </summary>
        /// <param name="mac">is the MAC-address of the <see cref="ReceiverModule"/> to remove from the database</param>
        /// <exception cref="NotFoundException">When the MAC-address cannot be parsed or <see cref="ReceiverModule"/> not found</exception>
        /// <exception cref="Exception">When the item could not be removed</exception>
        [HttpDelete("{mac}")]
        public override async Task DeleteAsync(string mac)
        {
            // use the data service to remove the item
            var removed = await ((IReceiverModuleService) DataService).RemoveAsync(mac);

            // if the item could not be deleted, throw exception
            if (!removed)
                throw new NotFoundException($"The {typeof(ReceiverModule).Name} with id {mac} could not be found");
        }

        #endregion delete

        #endregion METHODS
    }
}