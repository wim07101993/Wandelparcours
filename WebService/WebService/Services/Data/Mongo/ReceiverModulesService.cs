using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WebService.Models;

namespace WebService.Services.Data.Mongo
{
    public class ReceiverModulesService : AMongoDataService<ReceiverModule>, IReceiverModulesService
    {
        public ReceiverModulesService(IConfiguration config)
            : base(
                config["Database:ConnectionString"],
                config["Database:DatabaseName"],
                config["Database:ReceiverModulesCollectionName"])
        {
        }


        public async Task<ReceiverModule> GetOneAsync(string name,
            IEnumerable<Expression<Func<ReceiverModule, object>>> propertiesToInclude = null)
            => await GetByAsync(x => x.Name == name);

        public async Task RemoveAsync(string name)
            => await RemoveByAsync(x => x.Name == name);
    }
}