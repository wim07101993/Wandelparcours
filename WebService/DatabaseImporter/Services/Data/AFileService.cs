using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DatabaseImporter.Helpers.Extensions;
using DatabaseImporter.Models.MongoModels.Bases;

namespace DatabaseImporter.Services.Data
{
    public abstract class AFileService : IFileDataService
    {
        public abstract string ExtensionFilter { get; }


        public async Task<IEnumerable> GetAsync<T>(
            IEnumerable<Expression<Func<T, object>>> selectors,
            params string[] locationParameters)
            where T : IModelWithObjectID
        {
            if (EnumerableExtensions.IsNullOrEmpty(locationParameters))
                return null;

            string content;
            using (var stream = File.OpenText(locationParameters[0]))
            {
                content = await stream.ReadToEndAsync();
            }

            return Deserialize<T>(content);
        }

        public async Task AddAsync<T>(
            IEnumerable<T> items,
            params string[] locationParameters)
            where T : IModelWithObjectID
        {
            if (EnumerableExtensions.IsNullOrEmpty(locationParameters))
                return;

            using (var stream = File.CreateText(locationParameters[0]))
            {
                await stream.WriteAsync(Serialize(items));
            }
        }


        public abstract string Serialize<T>(IEnumerable<T> values);
        public abstract IEnumerable<T> Deserialize<T>(string str);
    }
}