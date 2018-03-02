using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using WebService.Models;

namespace WebService.Services.Data.Mondo
{
    /// <inheritdoc cref="AMongoDataService{T}"/>
    /// <summary>
    /// ReceiverModuleService is a class that extends from the <see cref="AMongoDataService{T}"/> class
    /// and by doing that implements the <see cref="IDataService{T}"/> interface.
    /// <para/>
    /// It handles the saving and retreiving residents to and from the mongo database.
    /// <para/>
    /// The connectionstring, db name and collections that are used are stored in the IConfiguration dependency under the Database object.
    /// </summary>
    public class ReceiverModuleService : AMongoDataService<ReceiverModule>
    {
        /// <summary>
        /// ReceiverModuleService is the contsructor to create an instance of the <see cref="ReceiverModuleService"/> class.
        /// <para/>
        /// The connectionstring, db name and collections that are used are stored in the IConfiguration dependency under the Database object.
        /// </summary>
        /// <param name="config"></param>
        public ReceiverModuleService(IConfiguration config)
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
    }
}
