using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Helpers.Exceptions;
using WebService.Models;

namespace WebService.Services.Data.Mock
{
#pragma warning disable CS1998 // disable warning async methods that not use await operator
    /// <inheritdoc cref="IDataService{T}"/>
    /// <summary>
    /// MockResidentsService is a class that implements the <see cref="IDataService{T}"/> interface.
    /// <para/>
    /// It handles the saving and retrieving data to and from a list of Residents in memory. It does not store anything in a database.
    /// </summary>
    public partial class MockReceiverModulesesService : AMockDataService<ReceiverModule>, IReceiverModulesService
    {
        /// <inheritdoc cref="AMockDataService{T}" />
        /// <summary>
        /// CreateNewItems returns a new item of the given type <see cref="ReceiverModule" /> with as Id, <see cref="id" />.
        /// </summary>
        /// <param name="id">is the id for the new object</param>
        /// <returns>A new object of type <see cref="ReceiverModule" /></returns>
        public override ReceiverModule CreateNewItem(ObjectId id)
            => new ReceiverModule {Id = id};

        /// <inheritdoc cref="IReceiverModulesService.GetAsync(string,IEnumerable{Expression{Func{ReceiverModule,object}}})" />
        /// <summary>
        /// GetAsync returns the receiver module with the given mac.
        /// </summary>
        /// <param name="mac">is the mac address of the receiver module to fetch</param>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>The receiver module with the given mac</returns>
        /// <exception cref="ArgumentNullException">when the mac address is null</exception>
        /// <exception cref="NotFoundException">when there is no item found with the given mac address</exception>
        public async Task<ReceiverModule> GetAsync(string mac,
            IEnumerable<Expression<Func<ReceiverModule, object>>> propertiesToInclude = null)
        {
            // if the mac is null, throw exception
            if (mac == null)
                throw new ArgumentNullException(nameof(mac), "a mac address must be passed");

            // if the mac address is empty, it can't be found so throw exception
            if (string.IsNullOrEmpty(mac))
                throw new NotFoundException($"cannot find {typeof(ReceiverModule).Name} with MAC-address {mac}");

            // get the index of the module to return
            var index = MockData.FindIndex(x => x.Mac == mac);

            // if the module does not exist, throw exception
            if (index < 0)
                throw new NotFoundException($"cannot find {typeof(ReceiverModule).Name} with MAC-address {mac}");

            // if the properties are null or there are none, return all the properties
            if (propertiesToInclude == null)
                return MockData[index];

            // create new newItem to return with the id filled in
            var itemToReturn = new ReceiverModule {Mac = MockData[index].Mac};

             // go over each property selector that should be included
            foreach (var selector in propertiesToInclude)
            {
                // get property
                var prop = selector.Body is MemberExpression expression
                    // via member expression
                    ? expression.Member as PropertyInfo
                    // via unary expression
                    : ((MemberExpression) ((UnaryExpression) selector.Body).Operand).Member as PropertyInfo;

                // set the value of the property with the value of the mockItem
                prop?.SetValue(itemToReturn, prop.GetValue(MockData[index]));
            }

            // return the newItem
            return itemToReturn;
        }

        /// <inheritdoc cref="IReceiverModulesService.RemoveAsync(string)" />
        /// <summary>
        /// Remove removes the <see cref="ReceiverModule"/> with the given mac from the database.
        /// </summary>
        /// <param name="mac">is the mac of the <see cref="ReceiverModule"/> to remove in the database</param>
        /// <exception cref="ArgumentNullException">when the mac is null</exception>
        /// <exception cref="Exception">when the query was not acknowledged</exception>
        /// <exception cref="NotFoundException">when there was no item removed</exception>
        public async Task RemoveAsync(string mac)
        {
            // if the mac is null, throw exception
            if (mac == null)
                throw new ArgumentNullException(nameof(mac), "a mac address must be passed");

            // get the index of the newItem with the given id
            var index = MockData.FindIndex(x => x.Mac == mac);

            // if the index is -1 there was no item found
            if (index == -1)
                throw new NotFoundException($"the {typeof(ReceiverModule).Name} with mac {mac} could not be found");

            // remove the module
            MockData.RemoveAt(index);
        }
    }
#pragma warning restore CS1998
}