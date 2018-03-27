﻿using System;
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
using WebService.Models.Bases;
using ArgumentNullException = System.ArgumentNullException;

namespace WebService.Services.Data.Mongo
{
    public class UsersService : AMongoDataService<User>, IUsersService
    {
        public UsersService(IConfiguration config)
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


        public override async Task CreateAsync(User item)
        {
            // if the item is null, throw exception
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            // create a new id for the new item
            item.Id = ObjectId.GenerateNewId();
            item.Password = item.Password.Hash(item.Id);

            try
            {
                // save the new item to the database
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
            // get the item with the given id
            var find = MongoCollection.Find(x => x.UserName == userName);

            // if there is no item with the given id, throw exception
            if (find.Count() <= 0)
                throw new NotFoundException<User>(nameof(User.UserName), userName);

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
                throw new ArgumentNullException(nameof(propertyToSelect));

            // get the item with the given id
            var find = MongoCollection.Find(x => x.UserName == userName);

            // if there is no item with the given id, throw exception
            if (find.Count() <= 0)
                throw new NotFoundException<User>(nameof(User.UserName), userName);

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