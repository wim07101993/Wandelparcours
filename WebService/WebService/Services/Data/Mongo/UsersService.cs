using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
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

            item.Password = item.Password.Hash(item.Id);

            await CreateAsync(item, true);
        }

        public async Task<bool> CheckCredentialsAsync(ObjectId id, string password)
        {
            var userPassword = await GetPropertyAsync(id, x => x.Password);
            return password.EqualsToHash(id, userPassword);
        }

        public async Task UpdatePasswordAsync(ObjectId id, string password)
            => await UpdatePropertyAsync(id, x => x.Password, password.Hash(id));

        public async Task<User> GetByNameAsync(string userName,
            IEnumerable<Expression<Func<User, object>>> propertiesToInclude = null)
            => await GetByAsync(x => x.UserName == userName, propertiesToInclude);

        public async Task<T> GetPropertyByNameAsync<T>(string userName,
            Expression<Func<User, T>> propertyToSelect = null)
            => await GetPropertyByAsync(x => x.UserName == userName, propertyToSelect);
    }
}