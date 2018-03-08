using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using WebService.Helpers.Extensions;
using WebService.Models;

namespace WebService.Services.Data.Mongo
{
    /// <inheritdoc cref="AMongoDataService{T}"/>
    /// <summary>
    /// ReceiverModulesService is a class that extends from the <see cref="AMongoDataService{T}"/> class
    /// and by doing that implements the <see cref="IDataService{T}"/> interface.
    /// <para/>
    /// It handles the saving and retreiving residents to and from the mongo database.
    /// <para/>
    /// The connectionstring, db name and collections that are used are stored in the IConfiguration dependency under the Database object.
    /// </summary>
    public class ReceiverModulesService : AMongoDataService<ReceiverModule>, IReceiverModuleService
    {
        /// <summary>
        /// ReceiverModulesService is the contsructor to create an instance of the <see cref="ReceiverModulesService"/> class.
        /// <para/>
        /// The connectionstring, db name and collections that are used are stored in the IConfiguration dependency under the Database object.
        /// </summary>
        /// <param name="config"></param>
        public ReceiverModulesService(IConfiguration config)
        {
            // create a new client and get the databas from it
            var db = new MongoClient(config["Database:ConnectionString"]).GetDatabase(config["Database:DatabaseName"]);

            // get the residents mongo collection
            MongoCollection = db.GetCollection<ReceiverModule>(config["Database:ReceiverModulesCollectionName"]);
        }


        /// <inheritdoc cref="AMongoDataService{T}.MongoCollection" />
        /// <summary>
        /// MongoCollection is the mongo collection to query residents.
        /// </summary>
        public override IMongoCollection<ReceiverModule> MongoCollection { get; }


        /// <inheritdoc cref="IReceiverModuleService.GetAsync(string,IEnumerable{Expression{Func{ReceiverModule,object}}})" />
        /// <summary>
        /// GetAsync should return the receiver module with the given mac.
        /// </summary>
        /// <param name="mac">is the mac address of the receiver module to fetch</param>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>The receiver module with the given mac</returns>
        public async Task<ReceiverModule> GetAsync(string mac,
                IEnumerable<Expression<Func<ReceiverModule, object>>> propertiesToInclude = null)
        {
            // get the item with the given id
            var foundItem = MongoCollection.Find(x => x.Mac == mac);

            // convert the properties to include to a list (if not null)
            var properties = propertiesToInclude?.ToList();
            // if the proeprties are null or there are none, return all the properties
            if (EnumerableExtensions.IsNullOrEmpty(properties))
                return await foundItem.FirstOrDefaultAsync();

            // create a propertyfilter
            var selector = Builders<ReceiverModule>.Projection.Include(x => x.Id);

            //ReSharper disable once PossibleNullReferenceException
            // iterate over all the properties and add them to the filter
            foreach (var property in properties)
                selector = selector.Include(property);

            // return the item
            return await foundItem
                // filter the properties
                .Project<ReceiverModule>(selector)
                // execute the query
                .FirstOrDefaultAsync();
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
            // remove the receiver module with the given mac from the database
            var result = await MongoCollection.DeleteOneAsync(x => x.Mac == mac);
            // return true if something acutaly happened
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}