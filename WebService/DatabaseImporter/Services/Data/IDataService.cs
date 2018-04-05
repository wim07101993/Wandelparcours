using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DatabaseImporter.Models.MongoModels.Bases;

namespace DatabaseImporter.Services.Data
{
    public interface IDataService
    {
        Task<IEnumerable> GetAsync<T>(
            IEnumerable<Expression<Func<T, object>>> selectors,
            params string[] locationParameters)
            where T : IModelWithObjectID;

        Task AddAsync<T>(
            IEnumerable<T> items,
            params string[] locationParameters)
            where T : IModelWithObjectID;
    }

    public interface IJsonFileService : IDataService
    {
    }

    public interface IXmlFileService : IDataService
    {
    }

    public interface ICsvFileService : IDataService
    {
    }

    public interface IMongoService : IDataService
    {
    }
}