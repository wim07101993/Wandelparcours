using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using WebService.Helpers.Exceptions;
using WebService.Models;

namespace WebService.Services.Data.Mongo
{
    /// <inheritdoc cref="AMongoDataService{T}"/>
    /// <summary>
    /// ReceiverModulesService is a class that extends from the <see cref="AMongoDataService{T}"/> class
    /// and by doing that implements the <see cref="IDataService{T}"/> interface.
    /// <para/>
    /// It handles the saving and retrieving receiverModules to and from the mongo database.
    /// <para/>
    /// The connection string, db name and collections that are used are stored in the IConfiguration dependency under the Database object.
    /// </summary>
    public class ReceiverModulesService : AMongoDataService<ReceiverModule>, IReceiverModulesService
    {
        /// <inheritdoc cref="AMongoDataService{T}"/>
        /// <summary>
        /// ReceiverModulesService is the constructor to create an instance of the <see cref="ReceiverModulesService"/> class.
        /// <para/>
        /// The connection string, db name and collections that are used are stored in the IConfiguration dependency under the Database object.
        /// </summary>
        /// <param name="config">are the configurations to get the database details from</param>
        /// <param name="iThrow">is the object to throw exceptions</param>
        public ReceiverModulesService(IConfiguration config)
        {
            MongoCollection =
                // create a new client
                new MongoClient(config["Database:ConnectionString"])
                    // get the database from the client
                    .GetDatabase(config["Database:DatabaseName"])
                    // get the residents mongo collection
                    .GetCollection<ReceiverModule>(config["Database:ReceiverModulesCollectionName"]);
        }


        /// <inheritdoc cref="AMongoDataService{T}.MongoCollection" />
        /// <summary>
        /// MongoCollection is the mongo collection to query residents.
        /// </summary>
        public override IMongoCollection<ReceiverModule> MongoCollection { get; }


        /// <inheritdoc cref="IReceiverModulesService.GetAsync(string,IEnumerable{Expression{Func{ReceiverModule,object}}})" />
        /// <summary>
        /// GetOneAsync returns the receiver module with the given mac.
        /// </summary>
        /// <param name="mac">is the mac address of the receiver module to fetch</param>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>The receiver module with the given mac</returns>
        /// <exception cref="System.ArgumentNullException">when the mac address is null</exception>
        /// <exception cref="NotFoundException">when there is no item found with the given mac address</exception>
        public async Task<ReceiverModule> GetAsync(string mac,
            IEnumerable<Expression<Func<ReceiverModule, object>>> propertiesToInclude = null)
        {
            // if the mac is null, throw exception
            if (mac == null)
                throw new NotFoundException<ReceiverModule>(nameof(ReceiverModule.Mac), mac);

            // get the item with the given id
            var find = MongoCollection.Find(x => x.Mac == mac);

            // if there is no resident with the given id, throw exception
            if (find.Count() <= 0)
                throw new NotFoundException<ReceiverModule>(nameof(ReceiverModule.Mac), mac);

            // if the properties are null or there are none, return all the properties
            if (propertiesToInclude == null)
                return await find.FirstOrDefaultAsync();

            // create a property filter and always include the mac address
            var selector = Builders<ReceiverModule>.Projection.Include(x => x.Mac);

            // iterate over all the properties and add them to the filter
            selector = propertiesToInclude.Aggregate(selector, (current, property) => current.Include(property));

            // return the item
            return await find
                // filter the properties
                .Project<ReceiverModule>(selector)
                // execute the query
                .FirstOrDefaultAsync();
        }

        /// <inheritdoc cref="IReceiverModulesService.RemoveAsync(string)" />
        /// <summary>
        /// Remove removes the <see cref="ReceiverModule"/> with the given mac from the database.
        /// </summary>
        /// <param name="mac">is the mac of the <see cref="ReceiverModule"/> to remove in the database</param>
        /// <exception cref="System.ArgumentNullException">when the mac is null</exception>
        /// <exception cref="MongoException">when the query was not acknowledged</exception>
        /// <exception cref="NotFoundException">when there was no item removed</exception>
        public async Task RemoveAsync(string mac)
        {
            // if the mac is null, throw exception
            if (mac == null)
                throw new NotFoundException<ReceiverModule>(nameof(ReceiverModule.Mac), mac);

            // remove the receiver module with the given mac from the database
            var deleteResult = await MongoCollection.DeleteOneAsync(x => x.Mac == mac);

            // if the query is not acknowledged, throw exception
            if (!deleteResult.IsAcknowledged)
                throw new DatabaseException(EDatabaseMethod.Delete);

            // if there is no item with the given id, throw exception
            if (deleteResult.DeletedCount <= 0)
                throw new NotFoundException<ReceiverModule>(nameof(ReceiverModule.Mac), mac);
        }
    }
}