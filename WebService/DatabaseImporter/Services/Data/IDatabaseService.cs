using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DatabaseImporter.Models.MongoModels.Bases;

namespace DatabaseImporter.Services.Data
{
    public interface IDatabaseService : IDataService
    {
        Task<IEnumerable<string>> GetDatabasesAsync(string ipAddress);

        Task<IEnumerable<string>> GetTables(string ipAddress, string database);

        Task<IEnumerable> GetAsync<T>(
            IEnumerable<Expression<Func<T, object>>> selectors,
            string ipAddres, string database, string collection)
            where T : IModelWithObjectID;

        Task AddAsync<T>(
            IEnumerable<T> items,
            string connectionString, string database, string collection)
            where T : IModelWithObjectID;
    }


    public interface IMongoService : IDatabaseService
    {
    }
}