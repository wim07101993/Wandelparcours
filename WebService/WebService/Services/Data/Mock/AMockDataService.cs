using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Helpers.Exceptions;
using WebService.Models.Bases;
using WebService.Services.Exceptions;

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
        protected readonly IThrow Throw;

        protected AMockDataService(IThrow iThrow)
        {
            Throw = iThrow;
        }

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
        /// Create saves the passed <see cref="T"/> to the database.
        /// </summary>
        /// <param name="item">is the <see cref="T"/> to save in the database</param>
        /// <exception cref="ArgumentNullException">when the item to create is null</exception>
        public virtual async Task CreateAsync(T item)
        {
            // if the item is null, throw exception
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            // create a new id for the item
            item.Id = ObjectId.GenerateNewId();
            // add the new item to the list
            MockData.Add(item);
        }

        public virtual async Task AddItemToListProperty(ObjectId id,
            Expression<Func<T, IEnumerable<object>>> propertyToAddItemTo, object itemToAdd)
        {
            if (itemToAdd == null)
            {
                Throw.NullArgument(nameof(itemToAdd));
                return;
            }


            if (itemToAdd is IModelWithID modelWithID)
                modelWithID.Id = ObjectId.GenerateNewId();

            // get the index of the item to update
            var index = MockData.FindIndex(x => x.Id == id);

            // if the item doesn't exist, throw exception
            if (index < 0)
                Throw.NotFound<T>(id);


            // get the property
            var prop = propertyToAddItemTo.Body is MemberExpression expression
                // via member expression
                ? expression.Member as PropertyInfo
                // if that fails, unary expression
                : ((MemberExpression) ((UnaryExpression) propertyToAddItemTo.Body).Operand).Member as PropertyInfo;

            if (prop == null)
            {
                Throw.PropertyNotKnown<T>("");
                return;
            }

            var oldValue = (prop.GetValue(MockData[index]) as IEnumerable<object>)?.ToList();

            if (oldValue == null)
            {
                Throw.Exception("could not convert property");
                return;
            }

            oldValue.Add(itemToAdd);

            prop.SetValue(MockData[index], oldValue);
        }

        #endregion CREATE


        #region READ

        /// <inheritdoc cref="IDataService{T}.GetAsync(IEnumerable{Expression{System.Func{T,object}}})" />
        /// <summary>
        /// Get returns all the items from the database. 
        /// <para />
        /// It only fills the properties passed in the <see cref="propertiesToInclude" /> parameter. The id is always passed and 
        /// if the <see cref="propertiesToInclude" /> parameter is null (which it is by default), all the properties are included.
        /// Other properties are given their default value. 
        /// </summary>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>An <see cref="IEnumerable{T}"/> filled with all the items in the database.</returns>
        public virtual async Task<IEnumerable<T>> GetAsync(
            IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null)
        {
            // if there are no properties to select, select them all
            if (propertiesToInclude == null)
                return MockData;

            // select the needed properties
            return MockData.Select(mockItem =>
            {
                // create new newItem to return with the id filled in
                var itemToReturn = CreateNewItem(mockItem.Id);

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
                    prop?.SetValue(itemToReturn, prop.GetValue(mockItem));
                }

                // return the newItem
                return itemToReturn;
            });
        }

        /// <inheritdoc cref="IDataService{T}.GetAsync(ObjectId,IEnumerable{Expression{System.Func{T,object}}})" />
        /// <summary>
        /// GetAsync returns the <see cref="T"/> with the given id from the database. 
        /// <para/>
        /// It only fills the properties passed in the <see cref="propertiesToInclude"/> parameter. The id is always passed and 
        /// if the <see cref="propertiesToInclude"/> parameter is null (which it is by default), all the properties are included. 
        /// </summary>
        /// <param name="id">is the id of the item that needs to be fetched</param>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>An <see cref="IEnumerable{T}"/> filled with all the ts in the database.</returns>
        /// <exception cref="NotFoundException">when there is no item found with the given id</exception>
        public async Task<T> GetAsync(ObjectId id, IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null)
        {
            // get the index of the item
            var index = MockData.FindIndex(x => x.Id == id);

            // if the item doesn't exist, throw exception
            if (index < 0)
                throw new NotFoundException($"no {typeof(T).Name} with id {id} is found");

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

        /// <inheritdoc cref="IDataService{T}.GetPropertyAsync"/>
        /// <summary>
        /// GetPropertyAsync is supposed to return a single property of the <see cref="T"/> with the given id
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/> to get the property from</param>
        /// <param name="propertyToSelect">is the selector to select the property to return</param>
        /// <returns>The value of the asked property</returns>
        /// <exception cref="ArgumentNullException">when the property to select is null</exception>
        /// <exception cref="NotFoundException">when there is no item with the given id</exception>
        public virtual async Task<object> GetPropertyAsync(ObjectId id, Expression<Func<T, object>> propertyToSelect)
        {
            // if the property to select is null, throw exception
            if (propertyToSelect == null)
                throw new ArgumentNullException(nameof(propertyToSelect),
                    "the property to select selector cannot be null");

            // get the item
            var item = MockData.FirstOrDefault(x => x.Id == id);

            // if there are no items, throw exception
            if (item == null)
                throw new NotFoundException($"no {typeof(T).Name} with id {id} is found");

            // return the property
            return propertyToSelect.Compile()(item);
        }

        #endregion READ


        #region UPDATE

        /// <inheritdoc cref="IDataService{T}.UpdateAsync" />
        /// <summary>
        /// Update updates the <see cref="T" /> with the id of the given <see cref="T" />.
        /// <para />
        /// The updated properties are defined in the <see cref="propertiesToUpdate" /> parameter.
        /// If the <see cref="propertiesToUpdate" /> parameter is null (which it is by default), all properties are updated.
        /// </summary>
        /// <param name="newItem">is the <see cref="T" /> to update</param>
        /// <param name="propertiesToUpdate">are the properties that need to be updated</param>
        /// <exception cref="ArgumentNullException">when the new item is null</exception>
        /// <exception cref="Exception">when the query was not acknowledged</exception>
        /// <exception cref="NotFoundException">when there was no item with the same id as the newItem</exception>
        public virtual async Task UpdateAsync(T newItem,
            IEnumerable<Expression<Func<T, object>>> propertiesToUpdate = null)
        {
            // if there are no properties to update, replace the old item with the new
            if (propertiesToUpdate == null)
            {
                await ReplaceAsync(newItem);
                return;
            }

            // if the new item is null, throw exception
            if (newItem == null)
                throw new ArgumentNullException(nameof(newItem), "the item to update cannot be null");

            // get the index of the item to update
            var index = MockData.FindIndex(x => x.Id == newItem.Id);

            // if the item doesn't exist, throw exception
            if (index < 0)
                Throw.NotFound<T>(newItem.Id);

            // iterate over all the properties that need to be updated
            foreach (var selector in propertiesToUpdate)
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
        }

        /// <summary>
        /// ReplaceAsync replaces an item in the database with the new item. The item to replace is the item with the same id as the newItem
        /// </summary>
        /// <param name="newItem">is the new <see cref="T"/></param>
        /// <exception cref="ArgumentNullException">when the new item is null</exception>
        /// <exception cref="Exception">when the query was not acknowledged</exception>
        /// <exception cref="NotFoundException">when there was no item with the same id as the newItem</exception>
        private async Task ReplaceAsync(T newItem)
        {
            // if the new item is null, throw exception
            if (newItem == null)
                throw new ArgumentNullException(nameof(newItem), "the item to update cannot be null");

            // get the index of the item to update
            var index = MockData.FindIndex(x => x.Id == newItem.Id);

            // if the item doesn't exist, throw exception
            if (index < 0)
                throw new NotFoundException($"no item with the id {newItem.Id} could be found");

            // replace the old item
            MockData[index] = newItem;
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
        public async Task UpdatePropertyAsync<TValue>(ObjectId id, Expression<Func<T, TValue>> propertyToUpdate,
            TValue value)
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

            var index = MockData.FindIndex(x => x.Id == id);
            if (index < 0)
                throw new NotFoundException($"no item with the id {id} could be found");

            prop.SetValue(MockData[index], value);
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
        public virtual async Task RemoveAsync(ObjectId id)
        {
            // get the index of the newItem with the given id
            var index = MockData.FindIndex(x => x.Id == id);

            // if the index is -1 there was no item found
            if (index == -1)
                throw new NotFoundException($"no item with the id {id} could be found");

            // remove the newItem
            MockData.RemoveAt(index);
        }

        #endregion DELETE
    }
#pragma warning restore CS1998
}