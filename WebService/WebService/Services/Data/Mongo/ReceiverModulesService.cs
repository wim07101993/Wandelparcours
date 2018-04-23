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
            : base(config["Database:ConnectionString"],
                config["Database:DatabaseName"],
                config["Database:ReceiverModulesCollectionName"])
        {
        }


        public async Task<ReceiverModule> GetAsync(string mac,
            IEnumerable<Expression<Func<ReceiverModule, object>>> propertiesToInclude = null)
            => await GetByAsync(x => x.Mac == mac);

        public async Task RemoveAsync(string mac)
            => await RemoveByAsync(x => x.Mac == mac);
    }
}