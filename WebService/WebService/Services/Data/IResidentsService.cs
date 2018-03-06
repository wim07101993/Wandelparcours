using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebService.Models;

namespace WebService.Services.Data
{
    public interface IResidentsService : IDataService<Resident>
    {
        Task<Resident> GetAsync(int tag, IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null);
    }
}