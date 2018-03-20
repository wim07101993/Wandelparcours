using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using WebService.Helpers.Extensions;
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


        public async Task<bool> CheckCredentialsAsync(string userName, string password)
        {
            var find = await MongoCollection.FindAsync(x => x.UserName == userName && x.Password == password);
            return find.Any();
        }

        public override Task CreateAsync(User item)
        {
            item.HashPassword();
            return base.CreateAsync(item);
        }

        public override Task UpdateAsync(User newItem,
            IEnumerable<Expression<Func<User, object>>> propertiesToUpdate = null)
        {
            newItem.HashPassword();
            return base.UpdateAsync(newItem, propertiesToUpdate);
        }

        public async Task TaskUpdatePasswordAsync(ObjectId id, string password)
        {
            var user = await GetAsync(id);
            user.Password = password;
            user.HashPassword();
            await UpdatePropertyAsync(user.Id, x => x.Password, user.Password);
        }
    }
}