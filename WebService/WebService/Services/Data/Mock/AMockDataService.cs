using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Helpers;
using WebService.Models.Bases;

namespace WebService.Services.Data.Mock
{
#pragma warning disable CS1998 // disable warning async methods that not use await operator
    /// <inheritdoc cref="IDataService{T}"/>
    /// <summary>
    /// MockItemsService is a class that implements the <see cref="IDataService{T}"/> interface.
    /// <para/>
    /// It handles the saving and retreiving data to and from a list of Items in memory. It does not store anything in a database.
    /// <para/>
    /// The connectionstring, db name and collections that are used are stored in the IConfiguration dependency under the Database object.
    /// </summary>
    public abstract class AMockDataService<T> : IDataService<T> where T: IModelWithID
    {
        /// <summary>
        /// MockData is the list of items to test the application.
        /// </summary>
        public abstract List<T> MockData { get; }


        /// <summary>
        /// CreateNewItems should return a new item of the given type <see cref="T"/> with as Id, <see cref="id"/>.
        /// </summary>
        /// <param name="id">is the id for the new object</param>
        /// <returns>A new object of type <see cref="T"/></returns>
        public abstract T CreateNewItem(ObjectId id);


        /// <inheritdoc cref="IDataService{T}.GetAsync" />
        /// <summary>
        /// Get returns all the items from the mock list. 
        /// <para />
        /// It only fills the properties passed in the <see cref="propertiesToInclude" /> parameter. The id is always passed and 
        /// if the <see cref="propertiesToInclude" /> parameter is null (which it is by default), all the properties are included.
        /// Other properties are given their default value. 
        /// </summary>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>An <see cref="IEnumerable{T}"/> filled with all the items in the mock list.</returns>
        public virtual async Task<IEnumerable<T>> GetAsync(
            IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null)
        {
            var propertiesToIncludeList = propertiesToInclude?.ToList();
            return EnumerableExtensions.IsNullOrEmpty(propertiesToIncludeList)
                // if the properties to include are null, return the complete list
                ? MockData
                // else return a list with only the asked properties filled
                : MockData.Select(mockItem =>
                {
                    // create new newItem to return with the ide filled in
                    var itemToReturn = CreateNewItem(mockItem.Id);

                    // ReSharper disable once PossibleNullReferenceException
                    // go over each property selector that should be included
                    foreach (var selector in propertiesToIncludeList)
                    {
                        // get property
                        var prop = selector.Body is MemberExpression expression
                            // via member expression
                            ? expression.Member as PropertyInfo
                            // via unary expression
                            : ((MemberExpression) ((UnaryExpression) selector.Body).Operand).Member as PropertyInfo;

                        // set the value of the property with the value of the mockItem
                        prop?.SetValue(itemToReturn, prop.GetValue(mockItem));
                    }

                    // return the newItem
                    return itemToReturn;
                });
        }

        /// <inheritdoc cref="IDataService{T}.CreateAsync" />
        /// <summary>
        /// Create saves the passed <see cref="T"/> to the list.
        /// <para/>
        /// If the newItem is created, the method returns the id of the new <see cref="T"/>, else null.
        /// </summary>
        /// <param name="item">is the <see cref="T"/> to save in the list</param>
        /// <returns>
        /// - the new id if the <see cref="T"/> was created in the list
        /// - null if the newItem was not created
        /// </returns>
        public virtual async Task<string> CreateAsync(T item)
        {
            // create a new ide for the item
            item.Id = ObjectId.GenerateNewId();
            // add the new item to the list
            MockData.Add(item);

            // check if the item was created

            return MockData.Any(x => x.Id == item.Id)
                // if it is, return the id
                ? item.Id.ToString()
                // else return null
                : null;
        }

        /// <inheritdoc cref="IDataService{T}.RemoveAsync" />
        /// <summary>
        /// RemoveItem removes the <see cref="T"/> with the given id from the list.
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/> to remove in the list</param>
        /// <returns>
        /// - true if the <see cref="T"/> was removed from the list
        /// - false if the newItem was not removed
        /// </returns>
        public virtual async Task<bool> RemoveAsync(ObjectId id)
        {
            // get the index of the newItem with the given id
            var index = MockData.FindIndex(x => x.Id == id);

            // if the index is -1 there was no item found
            if (index == -1)
                return false;

            // remove the newItem
            MockData.RemoveAt(index);
            return true;
        }

        /// <inheritdoc cref="IDataService{T}.UpdateAsync" />
        /// <summary>
        /// UpdateItem updates the <see cref="T" /> with the id of the given <see cref="T" />.
        /// <para />
        /// The updated properties are defined in the <see cref="propertiesToUpdate" /> parameter.
        /// If the <see cref="!:propertiesToUpdate" /> parameter is null or empty (which it is by default), all properties are updated.
        /// </summary>
        /// <param name="newItem">is the <see cref="T" /> to update</param>
        /// <param name="propertiesToUpdate">are the properties that need to be updated</param>
        /// <returns>The updated newItem</returns>
        public virtual async Task<T> UpdateAsync(T newItem,
            IEnumerable<Expression<Func<T, object>>> propertiesToUpdate = null)
        {
            // create list of the enumerable to prevent multiple enumerations of enumerable
            var propertiesToUpdateList = propertiesToUpdate?.ToList();

            var index = MockData.FindIndex(x => x.Id == newItem.Id);
            if (index < 0)
                return default(T);

            // check if thereare properties to update.
            if (EnumerableExtensions.IsNullOrEmpty(propertiesToUpdateList))
            {
                // update the item;
                MockData[index] = newItem;
                // return the updated item
                return MockData.FirstOrDefault(x => x.Id == newItem.Id);
            }

            // ReSharper disable once PossibleNullReferenceException
            // iterate over all the properties that need to be updated
            foreach (var selector in propertiesToUpdateList)
            {
                // get the property
                var prop = selector.Body is MemberExpression expression
                    // via member expression
                    ? expression.Member as PropertyInfo
                    // if that failse, unary expression
                    : ((MemberExpression) ((UnaryExpression) selector.Body).Operand).Member as PropertyInfo;

                // check if the property exists
                if (prop != null)
                    // if it does, add the selector and value to the updateDefinition
                    prop.SetValue(MockData[index], prop.GetValue(newItem));
            }

            // return the updated item
            return MockData.FirstOrDefault(x => x.Id == newItem.Id);
        }
    }
#pragma warning restore CS1998
}