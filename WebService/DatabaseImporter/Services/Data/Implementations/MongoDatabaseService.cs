using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace DatabaseImporter.Services.Data.Implementations
{
    public class MongoDatabaseService : ADatabaseService, IMongoService
    {
        private static string IpToConnectionString(string ipAddress)
            => $"mongodb://{ipAddress}";

        public override async Task<IEnumerable<string>> GetDatabasesAsync(string ipAddress)
        {
            var databases = new List<string>();

            var client = new MongoClient(IpToConnectionString(ipAddress));
            using (var cursor = await client.ListDatabasesAsync())
            {
                await cursor.ForEachAsync(x => databases.Add(x["name"].AsString));
            }

            return databases;
        }

        public override async Task<IEnumerable<string>> GetTables(string ipAddress, string database)
        {
            var collections = new List<string>();

            var db = new MongoClient(IpToConnectionString(ipAddress)).GetDatabase(database);
            using (var cursor = await db.ListCollectionsAsync())
            {
                await cursor.ForEachAsync(x => collections.Add(x["name"].AsString));
            }

            return collections;
        }

        public override async Task<IEnumerable> GetAsync<T>(
            IEnumerable<Expression<Func<T, object>>> selectors,
            string ipAddress, string database, string collection)
        {
            var foundItems = GetCollection<T>(IpToConnectionString(ipAddress), database, collection)
                .Find(FilterDefinition<T>.Empty);

            if (selectors == null)
                return await foundItems.ToListAsync();

            var selector = Builders<T>.Projection.Include(x => x.Id);
            selector = selectors.Aggregate(selector, (current, property) => current.Include(property));

            return foundItems
                .Project<T>(selector)
                .ToList();
        }

        public override async Task AddAsync<T>(IEnumerable<T> items, string ipAddress, string database,
            string collection)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            await GetCollection<T>($"mongodb://{ipAddress}", database, collection)
                .InsertManyAsync(items);
        }

        private static IMongoCollection<T> GetCollection<T>(string connectionString, string database, string collection)
            => new MongoClient(connectionString).GetDatabase(database).GetCollection<T>(collection);
    }
}