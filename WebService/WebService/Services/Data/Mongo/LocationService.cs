using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using WebService.Models;


namespace WebService.Services.Data.Mongo
{
    public class LocationService : AMongoDataService<Location>, ILocationService
    {
        public LocationService(IConfiguration config) : base()
        { MongoCollection =
            // create a new client
            new MongoClient(config["Database:ConnectionString"])
                // get the database from the client
                .GetDatabase(config["Database:DatabaseName"])
                // get the residents mongo collection
                .GetCollection<Location>(config["Database:LocationsCollectionName"]);
        }

        public override IMongoCollection<Location> MongoCollection { get; }
    }
}