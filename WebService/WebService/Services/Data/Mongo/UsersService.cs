using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using WebService.Models;
using WebService.Services.Exceptions;

namespace WebService.Services.Data.Mongo
{
    public class UsersService : AMongoDataService<User>, IUsersService
    {
        public UsersService(IConfiguration config, IThrow iThrow) : base(iThrow)
        {
            MongoCollection =
                // create a new client
                new MongoClient(config["Database:ConnectionString"])
                    // get the database from the client
                    .GetDatabase(config["Database:DatabaseName"])
                    // get the residents mongo collection
                    .GetCollection<User>(config["Database:UsersCollectionName"]);
        }

        public override IMongoCollection<User> MongoCollection { get; }
    }
}
