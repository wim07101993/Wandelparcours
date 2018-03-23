using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Models;

namespace WebService.Services.Data
{
    public interface IUsersService : IDataService<User>
    {
        Task<bool> CheckCredentialsAsync(ObjectId id, string password);
        Task TaskUpdatePasswordAsync(ObjectId id, string password);

        Task<User> GetByNameAsync(string userName,
            IEnumerable<Expression<Func<User, object>>> propertiesToInclude = null);

        Task<object> GetPropertyByNameAsync(string userName, Expression<Func<User, object>> propertyToInclude = null);
    }
}