using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using WebService.Models;

namespace WebService.Services.Data
{
    public interface IDataService
    {
        IEnumerable<Resident> GetResidents(IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null);
        
        string CreateResident(Resident resident);
    }
}