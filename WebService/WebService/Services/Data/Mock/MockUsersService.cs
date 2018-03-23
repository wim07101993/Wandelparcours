using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Exceptions;

#pragma warning disable CS1998 // disable warning async methods that not use await operator

namespace WebService.Services.Data.Mock
{
    public class MockUsersService : AMockDataService<User>, IUsersService
    {
        public MockUsersService(IThrow iThrow) : base(iThrow)
        {
        }

        public override List<User> MockData { get; } = Mock.MockData.MockUsers;

        public override User CreateNewItem(ObjectId id)
            => new User {Id = id};

        public override Task CreateAsync(User item)
        {
            item.Password = item.Password.Hash(item.Id);
            return base.CreateAsync(item);
        }

        public Task<bool> CheckCredentialsAsync(ObjectId id, string password)
            => Task.FromResult(MockData.Any(x => x.Id == id && password.EqualsToHash(id,x.Password)));

        public Task TaskUpdatePasswordAsync(ObjectId id, string password)
            => Task.Factory.StartNew(() =>
            {
                var user = MockData.FirstOrDefault(x => x.Id == id);
                if (user != null)
                    user.Password = password.Hash(user.Id);
            });

        public async Task<User> GetByNameAsync(string userName,
            IEnumerable<Expression<Func<User, object>>> propertiesToInclude = null)
        {
            // get the index of the item
            var index = MockData.FindIndex(x => x.UserName == userName);

            // if the item doesn't exist, throw exception
            if (index < 0)
                throw new NotFoundException($"no {typeof(User).Name} with user name {userName} is found");

            // if there are no properties to select, select them all
            if (propertiesToInclude == null)
                return MockData[index];

            // create new newItem to return with the id filled in
            var itemToReturn = CreateNewItem(MockData[index].Id);

            // go over each property selector that should be included
            foreach (var selector in propertiesToInclude)
            {
                // get property
                var prop = selector.Body is MemberExpression expression
                    // via member expression
                    ? expression.Member as PropertyInfo
                    // via unary expression
                    : ((MemberExpression) ((UnaryExpression) selector.Body).Operand).Member as PropertyInfo;

                // set the value of the property with the value of the mockItem
                prop?.SetValue(itemToReturn, prop.GetValue(MockData[index]));
            }

            // return the newItem
            return itemToReturn;
        }

        public async Task<object> GetPropertyByNameAsync(string userName,
            Expression<Func<User, object>> propertyToSelect = null)
        {
            // if the property to select is null, throw exception
            if (propertyToSelect == null)
                throw new ArgumentNullException(nameof(propertyToSelect),
                    "the property to select selector cannot be null");

            // get the item
            var item = MockData.FirstOrDefault(x => x.UserName == userName);

            // if there are no items, throw exception
            if (item == null)
                throw new NotFoundException($"no {typeof(User).Name} with user name {userName} is found");

            // return the property
            return propertyToSelect.Compile()(item);
        }
    }
}