using System;
using System.Collections.Generic;
using System.Linq;
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


        public override Task CreateAsync(User item)
        {
            item.Password = item.Password.Hash(item.Id);
            return base.CreateAsync(item);
        }

        public async Task<bool> CheckCredentialsAsync(ObjectId id, string password)
        {
            var find = await MongoCollection.FindAsync(
                x => x.Id == id && password.EqualsToHash(id, x.Password, true, true));
            return find.Any();
        }

        public async Task TaskUpdatePasswordAsync(ObjectId id, string password)
        {
            var user = await GetOneAsync(id);
            user.Password = password;
            await UpdatePropertyAsync(user.Id, x => x.Password, password.Hash(id));
        }

        public async Task<User> GetByNameAsync(string userName,
            IEnumerable<Expression<Func<User, object>>> propertiesToInclude = null)
        {
            // get the item with the given id
            var find = MongoCollection.Find(x => x.UserName == userName);

            // if there is no item with the given id, throw exception
            if (find.Count() <= 0)
            {
                Throw?.NotFound<User>(userName);
                return default(User);
            }

            // if the properties are null or there are none, return all the properties
            if (propertiesToInclude == null)
                return await find.FirstOrDefaultAsync();

            // create a property filter
            var selector = Builders<User>.Projection.Include(x => x.Id);

            // iterate over all the properties and add them to the filter
            selector = propertiesToInclude.Aggregate(selector, (current, property) => current.Include(property));

            // return the item
            return await find
                // filter the properties
                .Project<User>(selector)
                // execute the query
                .FirstOrDefaultAsync();
        }

        public async Task<object> GetPropertyByNameAsync(string userName,
            Expression<Func<User, object>> propertyToSelect = null)
        {
            // if the property to select is null, throw exception
            if (propertyToSelect == null)
            {
                Throw?.NullArgument(nameof(propertyToSelect));
                return null;
            }

            // get the item with the given id
            var find = MongoCollection.Find(x => x.UserName == userName);

            // if there is no item with the given id, throw exception
            if (find.Count() <= 0)
            {
                Throw?.NotFound<User>(userName);
                return null;
            }

            // create a property filter
            var selector = Builders<User>.Projection.Include(propertyToSelect);

            // execute the query
            var item = await find
                .Project<User>(selector)
                .FirstOrDefaultAsync();

            // return only the asked property
            return propertyToSelect.Compile()(item);
        }
    }
}