using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebService.Models;

namespace WebService.Services.Data
{
    public interface IReceiverModulesService : IDataService<ReceiverModule>
    {
        Task<ReceiverModule> GetOneAsync(string mac,
            IEnumerable<Expression<Func<ReceiverModule, object>>> propertiesToInclude = null);

        Task RemoveAsync(string mac);
    }
}