using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DatabaseImporter.Models.MongoModels;
using MongoDB.Driver;

namespace DatabaseImporter.Services.DataIO
{
    public class MongoService : IMongoService
    {
        #region FIELDS

        private IMongoClient _mongoClient;
        private string _connectionString;

        #endregion FIELDS


        #region PROPERTIES

        public string ConnectionString
        {
            get => _connectionString;
            set
            {
                if (Equals(_connectionString, value))
                    return;

                _connectionString = value;
                _mongoClient = new MongoClient(value);
            }
        }

        public string DatabaseName { get; set; }

        public string ResidentsTableName { get; set; }
        public string UsersTableName { get; set; }
        public string ReceiverModuleTableName { get; set; }
        public string MediaTableName { get; set; }

        private IMongoDatabase Database
            => _mongoClient.GetDatabase(DatabaseName);

        private IMongoCollection<Resident> ResidentsCollection
            => Database.GetCollection<Resident>(ResidentsTableName);

        private IMongoCollection<User> UsersCollection
            => Database.GetCollection<User>(UsersTableName);

        private IMongoCollection<ReceiverModule> ReceiverModulesCollection
            => Database.GetCollection<ReceiverModule>(ReceiverModuleTableName);

        private IMongoCollection<MediaData> MediaCollection
            => Database.GetCollection<MediaData>(MediaTableName);

        #endregion PROPERTIES


        #region METHODS

        #region get

        public async Task<IEnumerable<Resident>> GetResidents(
            IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null)
        {
            var foundItems = ResidentsCollection.Find(FilterDefinition<Resident>.Empty);

            if (propertiesToInclude == null)
                return await foundItems.ToListAsync();

            var selector = Builders<Resident>.Projection.Include(x => x.Id);
            selector = propertiesToInclude.Aggregate(selector, (current, property) => current.Include(property));

            return foundItems
                .Project<Resident>(selector)
                .ToList();
        }

        public async Task<IEnumerable<User>> GetUsers(
            IEnumerable<Expression<Func<User, object>>> propertiesToInclude = null)
        {
            var foundItems = UsersCollection.Find(FilterDefinition<User>.Empty);

            if (propertiesToInclude == null)
                return await foundItems.ToListAsync();

            var selector = Builders<User>.Projection.Include(x => x.Id);
            selector = propertiesToInclude.Aggregate(selector, (current, property) => current.Include(property));

            return foundItems
                .Project<User>(selector)
                .ToList();
        }

        public async Task<IEnumerable<ReceiverModule>> GetReceiverModules(
            IEnumerable<Expression<Func<ReceiverModule, object>>> propertiesToInclude = null)
        {
            var foundItems = ReceiverModulesCollection.Find(FilterDefinition<ReceiverModule>.Empty);

            if (propertiesToInclude == null)
                return await foundItems.ToListAsync();

            var selector = Builders<ReceiverModule>.Projection.Include(x => x.Id);
            selector = propertiesToInclude.Aggregate(selector, (current, property) => current.Include(property));

            return foundItems
                .Project<ReceiverModule>(selector)
                .ToList();
        }

        public async Task<IEnumerable<MediaData>> GetMedia(
            IEnumerable<Expression<Func<MediaData, object>>> propertiesToInclude = null)
        {
            var foundItems = MediaCollection.Find(FilterDefinition<MediaData>.Empty);

            if (propertiesToInclude == null)
                return await foundItems.ToListAsync();

            var selector = Builders<MediaData>.Projection.Include(x => x.Id);
            selector = propertiesToInclude.Aggregate(selector, (current, property) => current.Include(property));

            return foundItems
                .Project<MediaData>(selector)
                .ToList();
        }

        #endregion get


        #region add

        public async Task Add(IEnumerable<Resident> residents)
        {
            if (residents == null)
                throw new ArgumentNullException(nameof(residents));

            await ResidentsCollection.InsertManyAsync(residents);
        }

        public async Task Add(IEnumerable<User> users)
        {
            if (users == null)
                throw new ArgumentNullException(nameof(users));

            await UsersCollection.InsertManyAsync(users);
        }

        public async Task Add(IEnumerable<ReceiverModule> receiverModules)
        {
            if (receiverModules == null)
                throw new ArgumentNullException(nameof(receiverModules));

            await ReceiverModulesCollection.InsertManyAsync(receiverModules);
        }

        public async Task Add(IEnumerable<MediaData> media)
        {
            if (media == null)
                throw new ArgumentNullException(nameof(media));

            await MediaCollection.InsertManyAsync(media);
        }

        #endregion add 

        #endregion METHODS
    }
}