using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models;
using ArgumentNullException = System.ArgumentNullException;

namespace WebService.Services.Data.Mongo
{
    public class UsersService : AMongoDataService<User>, IUsersService
    {
        public UsersService(IConfiguration config)
            : base(config["Database:ConnectionString"],
                config["Database:DatabaseName"],
                config["Database:UsersCollectionName"])
        {
        }


        public override async Task CreateAsync(User item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            item.Id = ObjectId.GenerateNewId();
            item.Password = item.Password.Hash(item.Id);

            try
            {
                await MongoCollection.InsertOneAsync(item);
            }
            catch (Exception e)
            {
                throw new DatabaseException(EDatabaseMethod.Create, e);
            }
        }

        public async Task<bool> CheckCredentialsAsync(ObjectId id, string password)
        {
            var find = await MongoCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            return password.EqualsToHash(id, find.Password);
        }

        public async Task UpdatePasswordAsync(ObjectId id, string password)
        {
            var user = await GetOneAsync(id);
            user.Password = password;
            await UpdatePropertyAsync(user.Id, x => x.Password, password.Hash(id));
        }

        public async Task<User> GetByNameAsync(string userName,
            IEnumerable<Expression<Func<User, object>>> propertiesToInclude = null)
        {
            var find = MongoCollection.Find(x => x.UserName == userName);

            if (find.Count() <= 0)
                throw new NotFoundException<User>(nameof(User.UserName), userName);

            if (propertiesToInclude == null)
                return await find.FirstOrDefaultAsync();

            var selector = Builders<User>.Projection.Include(x => x.Id);

            selector = propertiesToInclude.Aggregate(selector, (current, property) => current.Include(property));

            return await find
                .Project<User>(selector)
                .FirstOrDefaultAsync();
        }

        public async Task<T> GetPropertyByNameAsync<T>(string userName,
            Expression<Func<User, T>> propertyToSelect = null)
        {
            if (propertyToSelect == null)
                throw new ArgumentNullException(nameof(propertyToSelect));

            var find = MongoCollection.Find(x => x.UserName == userName);

            if (find.Count() <= 0)
                throw new NotFoundException<User>(nameof(User.UserName), userName);

            var fieldDef = new ExpressionFieldDefinition<User>(propertyToSelect);
            var selector = Builders<User>.Projection.Include(fieldDef);

            var item = await find
                .Project<User>(selector)
                .FirstOrDefaultAsync();

            return propertyToSelect.Compile()(item);
        }
    }
}