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

        /// <inheritdoc cref="ARestControllerBase{T}.PropertiesToSendOnGetAll" />
        /// <summary>
        /// SmallDataProperties is a collection of expressions to select the properties that
        /// consume the least space (in this case all of them => value is null).
        /// </summary>
        public override IEnumerable<Expression<Func<ReceiverModule, object>>> PropertiesToSendOnGetAll { get; } = null;

        #endregion PROPERTIES


        #region METHODS

        /// <inheritdoc cref="ARestControllerBase{T}.ConvertStringsToSelectors" />
        /// <summary>
        /// ConvertStringsToSelectors converts a collection of property names to their selector in the form of 
        /// <see cref="Expression{TDelegate}"/> of type <see cref="Func{TInput,TResult}"/>
        /// </summary>
        /// <param name="strings">are the property names to convert to selectors</param>
        /// <returns>An <see cref="IEnumerable{TDelegate}"/> that contains the converted selectors</returns>
        /// <exception cref="WebArgumentException">When one ore more properties could not be converted to selectors</exception>
        public override IEnumerable<Expression<Func<ReceiverModule, object>>> ConvertStringsToSelectors(
            IEnumerable<string> strings)
        {
            // create a new list of selectors
            var selectors = new List<Expression<Func<ReceiverModule, object>>>();

            // fill the list of selectors by iterating over the properties to update
            foreach (var propertyName in strings)
            {
                // if the name of a properties matches a property of a Value, 
                // add the corresponding selector
                if (propertyName.EqualsWithCamelCasing(nameof(ReceiverModule.IsActive)))
                    selectors.Add(x => x.IsActive);
                else if (propertyName.EqualsWithCamelCasing(nameof(ReceiverModule.Mac)))
                    selectors.Add(x => x.Mac);
                else if (propertyName.EqualsWithCamelCasing(nameof(ReceiverModule.Position)))
                    selectors.Add(x => x.Position);
                else if (propertyName.EqualsWithCamelCasing(nameof(ReceiverModule.Id)))
                    // the id is always passed on get and ignored on update
                    // ReSharper disable once RedundantJumpStatement
                    continue;
                else
                    throw new WebArgumentException(
                        $"Property {propertyName} cannot be found on {typeof(ReceiverModule).Name}", nameof(strings));
            }

            return selectors;
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
            => await base.CreateAsync(item);

        #endregion post (create)

        #region get (read)

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
        /// By default only the properties in the selector <see cref="PropertiesToSendOnGetAll"/> are returned.
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
            var removed = await ((IReceiverModuleService)DataService).RemoveAsync(mac);

            // if the item could not be deleted, throw exception
            if (!removed)
                throw new NotFoundException($"The {typeof(ReceiverModule).Name} with id {mac} could not be found");
        }

        #endregion delete

        #endregion METHODS
    }
}