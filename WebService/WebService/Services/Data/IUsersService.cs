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
        Task UpdatePasswordAsync(ObjectId id, string password);

        Task<User> GetByNameAsync(string userName,
            IEnumerable<Expression<Func<User, object>>> propertiesToInclude = null);

        Task<T> GetPropertyByNameAsync<T>(string userName, Expression<Func<User, T>> propertyToInclude = null);
    }
}