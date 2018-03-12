using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
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
    public partial class MockReceiverModulesService : AMockDataService<ReceiverModule>, IReceiverModuleService
    {
        /// <inheritdoc cref="AMockDataService{T}" />
        /// <summary>
        /// CreateNewItems should return a new item of the given type <see cref="ReceiverModule" /> with as Id, <see cref="id" />.
        /// </summary>
        /// <param name="id">is the id for the new object</param>
        /// <returns>A new object of type <see cref="ReceiverModule" /></returns>
        public override ReceiverModule CreateNewItem(ObjectId id)
            => new ReceiverModule {Id = id};

        /// <inheritdoc cref="IReceiverModuleService.GetAsync(string, IEnumerable{Expression{Func{ReceiverModule, object}}})" />
        /// <summary>
        /// GetAsync should return the receiver module with the given mac.
        /// </summary>
        /// <param name="mac">is the mac address of the receiver module to fetch</param>
        /// <param name="propertiesToInclude">are the properties of which the values should be returned</param>
        /// <returns>The receiver module with the given mac</returns>
        public async Task<ReceiverModule> GetAsync(string mac,
            IEnumerable<Expression<Func<ReceiverModule, object>>> propertiesToInclude = null)
        {
            if (mac == null)
                throw new ArgumentNullException(nameof(mac), "a mac address must be passed");

            if (string.IsNullOrEmpty(mac))
                throw new NotFoundException($"cannot find {typeof(ReceiverModule).Name} with MAC-address {mac}");

            var propertiesToIncludeList = propertiesToInclude?.ToList();
            if (EnumerableExtensions.IsNullOrEmpty(propertiesToIncludeList))
                return MockData.FirstOrDefault(x => x.Mac == mac)
                       ?? throw new NotFoundException(
                           $"cannot find {typeof(ReceiverModule).Name} with MAC-address {mac}");

            foreach (var mockItem in MockData)
            {
                if (mockItem.Mac != mac)
                    continue;

                // create new newItem to return with the id filled in
                var itemToReturn = CreateNewItem(mockItem.Id);

                // ReSharper disable once PossibleNullReferenceException
                // go over each property selector that should be included
                foreach (var selector in propertiesToIncludeList)
                {
                    // get property
                    var prop = selector.Body is MemberExpression expression
                        // via member expression
                        ? expression.Member as PropertyInfo
                        // via unary expression
                        : ((MemberExpression) ((UnaryExpression) selector.Body).Operand).Member as PropertyInfo;

                    // set the value of the property with the value of the mockItem
                    prop?.SetValue(itemToReturn, prop.GetValue(mockItem));
                }

                // return the newItem
                return itemToReturn;
            }

            // if no item is found, return the default value
            throw new NotFoundException($"cannot find {typeof(ReceiverModule).Name} with MAC-address {mac}");
        }

        /// <inheritdoc cref="IReceiverModuleService.RemoveAsync(string)" />
        /// <summary>
        /// Remove removes the <see cref="ReceiverModule"/> with the given mac from the database.
        /// </summary>
        /// <param name="mac">is the mac of the <see cref="ReceiverModule"/> to remove in the database</param>
        /// <returns>
        /// - true if the <see cref="ReceiverModule"/> was removed from the database
        /// - false if the item was not removed
        /// </returns>
        public async Task<bool> RemoveAsync(string mac)
        {
            if (mac == null)
                throw new ArgumentNullException(nameof(mac), "a mac address must be passed");

            // get the index of the newItem with the given id
            var index = MockData.FindIndex(x => x.Mac == mac);

            // if the index is -1 there was no item found
            if (index == -1)
                throw new NotFoundException($"the {typeof(ReceiverModule).Name} with mac {mac} could not be found");

            // remove the newItem
            MockData.RemoveAt(index);
            return true;
        }
    }
#pragma warning restore CS1998
}