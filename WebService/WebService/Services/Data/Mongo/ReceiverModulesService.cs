using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using WebService.Helpers;
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
        

        /// <inheritdoc cref="IReceiverModuleService.GetAsync(string)" />
        /// <summary>
        /// GetAsync should return the receiver module with the given mac.
        /// </summary>
        /// <param name="mac">is the mac address of the receiver module to fetch</param>
        /// <returns>The receiver module with the given mac</returns>
        public async Task<ReceiverModule> GetAsync(string mac) 
            // return the item with the given mac address
            => await MongoCollection.Find(x => x.Mac == mac).FirstOrDefaultAsync();

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