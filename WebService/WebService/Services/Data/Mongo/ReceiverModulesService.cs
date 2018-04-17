using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using WebService.Helpers.Exceptions;
using WebService.Models;

namespace WebService.Services.Data.Mongo
{
    public class ReceiverModulesService : AMongoDataService<ReceiverModule>, IReceiverModulesService
    {
        public ReceiverModulesService(IConfiguration config)
            : base(config["Database:ConnectionString"],
                config["Database:DatabaseName"],
                config["Database:ReceiverModulesCollectionName"])
        {
        }


        public async Task<ReceiverModule> GetAsync(string mac,
            IEnumerable<Expression<Func<ReceiverModule, object>>> propertiesToInclude = null)
        {
            if (mac == null)
                throw new NotFoundException<ReceiverModule>(nameof(ReceiverModule.Mac), mac);

            var find = MongoCollection.Find(x => x.Mac == mac);

            if (find.Count() <= 0)
                throw new NotFoundException<ReceiverModule>(nameof(ReceiverModule.Mac), mac);

            if (propertiesToInclude == null)
                return await find.FirstOrDefaultAsync();

            var selector = Builders<ReceiverModule>.Projection.Include(x => x.Mac);

            selector = propertiesToInclude.Aggregate(selector, (current, property) => current.Include(property));

            return await find
                .Project<ReceiverModule>(selector)
                .FirstOrDefaultAsync();
        }

        public async Task RemoveAsync(string mac)
        {
            if (mac == null)
                throw new NotFoundException<ReceiverModule>(nameof(ReceiverModule.Mac), mac);

            var deleteResult = await MongoCollection.DeleteOneAsync(x => x.Mac == mac);

            if (!deleteResult.IsAcknowledged)
                throw new DatabaseException(EDatabaseMethod.Delete);

            if (deleteResult.DeletedCount <= 0)
                throw new NotFoundException<ReceiverModule>(nameof(ReceiverModule.Mac), mac);
        }
    }
}