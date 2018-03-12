using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Helpers.Exceptions;
using WebService.Models.Bases;

namespace WebService.Services.Data.Mock
{
#pragma warning disable CS1998 // disable warning async methods that not use await operator
    /// <inheritdoc cref="IDataService{T}"/>
    /// <summary>
    /// MockItemsService is a class that implements the <see cref="IDataService{T}"/> interface.
    /// <para/>
    /// It handles the saving and retrieving data to and from a list of Items in memory. It does not store anything in a database.
    /// </summary>
    public abstract class AMockDataService<T> : IDataService<T> where T : IModelWithID
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


        #region CREATE

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
        public virtual async Task<bool> CreateAsync(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            // create a new id for the item
            item.Id = ObjectId.GenerateNewId();
            // add the new item to the list
            MockData.Add(item);

            // check if the item was created
            return MockData.Any(x => x.Id == item.Id);
        }

        #endregion CREATE


        #region READ

        /// <inheritdoc cref="IDataService{T}.GetAsync(IEnumerable{Expression{Func{T, object}}})" />
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
            return propertiesToIncludeList == null
                // if the properties to include are null, return the complete list
                ? MockData
                // else return a list with only the asked properties filled
                : MockData.Select(mockItem =>
                {
                    // create new newItem to return with the id filled in
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

        /// <inheritdoc cref="IDataService{T}.GetAsync(ObjectId,IEnumerable{Expression{Func{T,object}}})" />
        /// <summary>
        /// GetAsync returns the <see cref="T" /> with the given id from the database. 
        /// <para />
        /// It should only fill the properties passed in the <see cref="propertiesToInclude" /> parameter. The id is always passed and 
        /// if the <see cref="propertiesToInclude" /> parameter is null (which it is by default), all the properties are included. 
        /// </summary>
        /// <param name="id">is the id of the item that needs to be fetched</param>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>An <see cref="IEnumerable{T}" /> filled with all the ts in the database.</returns>
        public async Task<T> GetAsync(ObjectId id, IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null)
        {
            var propertiesToIncludeList = propertiesToInclude?.ToList();
            if (propertiesToIncludeList == null)
            {
                var t = MockData.FirstOrDefault(x => x.Id == id);
                return t == null
                    ? throw new NotFoundException($"no {typeof(T).Name} with id {id} is found")
                    : t;
            }

            foreach (var mockItem in MockData)
            {
                if (mockItem.Id != id)
                    continue;

                // create new newItem to return with the id filled in
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
            }

            throw new NotFoundException($"no {typeof(T).Name} with id {id} is found");
        }

        /// <inheritdoc cref="IDataService{T}.GetPropertyAsync" />
        /// <summary>
        /// GetPropertyAsync returns a single property of the <see cref="T"/> with the given id
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/> to get the property from</param>
        /// <param name="propertyToSelect">is the selector to select the property to return</param>
        /// <returns>The value of the asked property</returns>
        public virtual async Task<object> GetPropertyAsync(ObjectId id, Expression<Func<T, object>> propertyToSelect)
            => propertyToSelect == null
                ? throw new ArgumentNullException(nameof(propertyToSelect))
                : MockData
                      .Where(x => x.Id == id)
                      .Select(propertyToSelect.Compile())
                      .FirstOrDefault()
                  ?? throw new NotFoundException($"no {typeof(T).Name} with id {id} is found");

        #endregion READ


        #region UPDATE

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
        public virtual async Task<bool> UpdateAsync(T newItem,
            IEnumerable<Expression<Func<T, object>>> propertiesToUpdate = null)
        {
            if (newItem == null)
                throw new ArgumentNullException(nameof(newItem), "the item to update cannot be null");

            // create list of the enumerable to prevent multiple enumerations of enumerable
            var propertiesToUpdateList = propertiesToUpdate?.ToList();

            var index = MockData.FindIndex(x => x.Id == newItem.Id);
            if (index < 0)
                throw new NotFoundException($"no item with the id {newItem.Id} could be found");

            // check if there are properties to update.
            if (propertiesToUpdateList == null)
            {
                // update the item;
                MockData[index] = newItem;
                // return the updated item
                return true;
            }

            // ReSharper disable once PossibleNullReferenceException
            // iterate over all the properties that need to be updated
            foreach (var selector in propertiesToUpdateList)
            {
                // get the property
                var prop = selector.Body is MemberExpression expression
                    // via member expression
                    ? expression.Member as PropertyInfo
                    // if that fails, unary expression
                    : ((MemberExpression) ((UnaryExpression) selector.Body).Operand).Member as PropertyInfo;

                // check if the property exists
                if (prop != null)
                    // if it does, add the selector and value to the updateDefinition
                    prop.SetValue(MockData[index], prop.GetValue(newItem));
            }

            // return the updated item
            return true;
        }

        /// <inheritdoc cref="IDataService{T}.UpdatePropertyAsync" />
        /// <summary>
        /// GetPropertyAsync updates a single property of the <see cref="T"/> with the given id
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/> to get the property from</param>
        /// <param name="propertyToUpdate">is the selector to select the property to update</param>
        /// <param name="value">is the new value of the property</param>
        /// <returns>
        /// - true if the property was updated
        /// - false if the property was not updated
        /// </returns>
        public async Task<bool> UpdatePropertyAsync(ObjectId id, Expression<Func<T, object>> propertyToUpdate,
            object value)
        {
            if (propertyToUpdate == null)
                throw new ArgumentNullException(nameof(propertyToUpdate));

            // get the property
            var prop = propertyToUpdate.Body is MemberExpression expression
                // via member expression
                ? expression.Member as PropertyInfo
                // if that fails, unary expression
                : ((MemberExpression) ((UnaryExpression) propertyToUpdate.Body).Operand).Member as PropertyInfo;

            // check if the property exists
            if (prop == null)
                throw new ArgumentException(
                    $"the property {propertyToUpdate} could not be found on the type {typeof(T).Name}");

            if (!prop.PropertyType.IsInstanceOfType(value))
                throw new ArgumentException($"the value {value} cannot be assigned to the property {prop.Name}",
                    nameof(value));

            var index = MockData.FindIndex(x => x.Id == id);
            if (index < 0)
                throw new NotFoundException($"no item with the id {id} could be found");

            prop.SetValue(MockData[index], value);
            return true;
        }

        #endregion UPDATE


        #region DELETE

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
                throw new NotFoundException($"no item with the id {id} could be found");

            // remove the newItem
            MockData.RemoveAt(index);
            return true;
        }

        #endregion DELETE
    }
#pragma warning restore CS1998
}