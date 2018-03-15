using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using WebService.Helpers.Exceptions;
using WebService.Models.Bases;
using WebService.Services.Exceptions;

namespace WebService.Services.Data.Mongo
{
    public abstract class AMongoDataService<T> : IDataService<T> where T : IModelWithID
    {
        #region FIELDS

        /// <summary>
        /// Throw is an object that handles exception throwing
        /// </summary>
        protected readonly IThrow Throw;

        #endregion FIELDS


        #region CONSTRUCTOR

        /// <summary>
        /// AMongoDataServcie creates an instance of the <see cref="AMongoDataService{T}"/> class
        /// </summary>
        /// <param name="iThrow">is the object that handles exception throwing</param>
        protected AMongoDataService(IThrow iThrow)
        {
            Throw = iThrow;
        }

        #endregion CONSTRUCTOR


        #region PROPERTIES

        /// <summary>
        /// MongoCollection is the mongo collection to query items.
        /// </summary>
        public abstract IMongoCollection<T> MongoCollection { get; }

        #endregion PROPERTIES


        #region METHDOS

        #region create

        /// <inheritdoc cref="IDataService{T}.CreateAsync" />
        /// <summary>
        /// Create saves the passed <see cref="T"/> to the database.
        /// </summary>
        /// <param name="item">is the <see cref="T"/> to save in the database</param>
        /// <exception cref="ArgumentNullException">when the item to create is null</exception>
        public async Task CreateAsync(T item)
        {
            // if the item is null, throw exception
            if (item == null)
            {
                Throw?.NullArgument(nameof(item));
                return;
            }

            // create a new id for the new item
            item.Id = ObjectId.GenerateNewId();

            try
            {
                // save the new item to the database
                await MongoCollection.InsertOneAsync(item);
            }
            catch (Exception)
            {
                Throw.Database<T>(EDatabaseMethod.Create);
            }
        }

        #endregion create

        #region read

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
        public async Task<IEnumerable<T>> GetAsync(IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null)
        {
            // get all the items
            var foundItems = MongoCollection.Find(FilterDefinition<T>.Empty);

            // if the properties are null or there are none, return all the properties
            if (propertiesToInclude == null)
                return await foundItems.ToListAsync();

            // create a property filter
            var selector = Builders<T>.Projection.Include(x => x.Id);

            // iterate over all the properties and add them to the filter
            selector = propertiesToInclude.Aggregate(selector, (current, property) => current.Include(property));

            // return the items
            return foundItems
                // filter the properties
                .Project<T>(selector)
                // execute the query
                .ToList();
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
            // get the item with the given id
            var find = MongoCollection.Find(x => x.Id == id);

            // if there is no item with the given id, throw exception
            if (find.Count() <= 0)
            {
                Throw?.NotFound<T>(id);
                return default(T);
            }

            // if the properties are null or there are none, return all the properties
            if (propertiesToInclude == null)
                return await find.FirstOrDefaultAsync();

            // create a property filter
            var selector = Builders<T>.Projection.Include(x => x.Id);

            // iterate over all the properties and add them to the filter
            selector = propertiesToInclude.Aggregate(selector, (current, property) => current.Include(property));

            // return the item
            return await find
                // filter the properties
                .Project<T>(selector)
                // execute the query
                .FirstOrDefaultAsync();
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
        public async Task<object> GetPropertyAsync(ObjectId id, Expression<Func<T, object>> propertyToSelect)
        {
            // if the property to select is null, throw exception
            if (propertyToSelect == null)
            {
                Throw?.NullArgument(nameof(propertyToSelect));
                return null;
            }

            // get the item with the given id
            var find = MongoCollection.Find(x => x.Id == id);

            // if there is no item with the given id, throw exception
            if (find.Count() <= 0)
            {
                Throw?.NotFound<T>(id);
                return null;
            }

            // create a property filter
            var selector = Builders<T>.Projection.Include(propertyToSelect);

            // TODO write pretty query
            // execute the query
            var item = await find
                .Project<T>(selector)
                .FirstOrDefaultAsync();

            // return only the asked property
            return propertyToSelect.Compile()(item);
        }

        #endregion read

        #region update

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
        /// <exception cref="MongoException">when the query was not acknowledged</exception>
        /// <exception cref="NotFoundException">when there was no item with the same id as the newItem</exception>
        public async Task UpdateAsync(T newItem,
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
            {
                Throw.NullArgument(nameof(newItem));
                return;
            }

            // create a filter that filters on id
            var filter = Builders<T>.Filter.Eq(x => x.Id, newItem.Id);

            // create an update definition.
            // since there needs to be an updateDefinition to start from, update the id, that is the same for the old an new object
            var update = Builders<T>.Update.Set(x => x.Id, newItem.Id);

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
                    update = update.Set(selector, prop.GetValue(newItem));
            }

            // update the document
            var updateResult = await MongoCollection.UpdateOneAsync(filter, update);

            // if the query is not acknowledged, throw exception
            if (!updateResult.IsAcknowledged)
            {
                Throw.Database<T>(EDatabaseMethod.Update, newItem.Id);
                return;
            }

            // if there is no item with the given id, throw exception
            if (updateResult.MatchedCount <= 0)
                Throw.NotFound<T>(newItem.Id);
        }

        /// <summary>
        /// ReplaceAsync replaces an item in the database with the new item. The item to replace is the item with the same id as the newItem
        /// </summary>
        /// <param name="newItem">is the new <see cref="T"/></param>
        /// <exception cref="ArgumentNullException">when the new item is null</exception>
        /// <exception cref="MongoException">when the query was not acknowledged</exception>
        /// <exception cref="NotFoundException">when there was no item with the same id as the newItem</exception>
        private async Task ReplaceAsync(T newItem)
        {
            // if the new item is null, throw exception
            if (newItem == null)
            {
                Throw.NullArgument(nameof(newItem));
                return;
            }

            // if there are no properties in the list, replace the document
            var replaceOneResult = await MongoCollection.ReplaceOneAsync(x => x.Id == newItem.Id, newItem);

            // if the query is not acknowledged, throw exception
            if (!replaceOneResult.IsAcknowledged)
            {
                Throw.Database<T>(EDatabaseMethod.Update, newItem.Id);
                return;
            }

            // if there is no item with the given id, throw exception
            if (replaceOneResult.MatchedCount <= 0)
                Throw.NotFound<T>(newItem.Id);
        }

        /// <inheritdoc cref="IDataService{T}.UpdatePropertyAsync" />
        /// <summary>
        /// GetPropertyAsync updates a single property of the <see cref="T"/> with the given id
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/> to get the property from</param>
        /// <param name="propertyToUpdate">is the selector to select the property to update</param>
        /// <param name="value">is the new value of the property</param>
        /// <exception cref="ArgumentNullException">when there is no property to update</exception>
        /// <exception cref="ArgumentException">when the property does not exist on <see cref="T"/></exception>
        /// <exception cref="ArgumentException">when the value is of the wrong type</exception>
        /// <exception cref="MongoException">when the query was not acknowledged</exception>
        /// <exception cref="NotFoundException">when there was no item with the same id as the newItem</exception>
        public async Task UpdatePropertyAsync(ObjectId id, Expression<Func<T, object>> propertyToUpdate,
            object value)
        {
            // if there is no property to update, throw exception
            if (propertyToUpdate == null)
            {
                Throw.NullArgument(nameof(propertyToUpdate));
                return;
            }

            // get the property to update
            var property = propertyToUpdate.Body is MemberExpression expression
                // via member expression
                ? expression.Member as PropertyInfo
                // via unary expression
                : ((MemberExpression) ((UnaryExpression) propertyToUpdate.Body).Operand).Member as PropertyInfo;

            // if the property doesn't exist, throw exception
            if (property == null)
            {
                Throw.PropertyNotKnown<T>("");
                return;
            }

            // if the value is not of the correct type, throw exception
            if (!property.PropertyType.IsInstanceOfType(value))
            {
                Throw.WrongTypeArgument(value.GetType(), property.PropertyType);
            }

            // create a filter that filters on id
            var filter = Builders<T>.Filter.Eq(x => x.Id, id);
            // create an update definition.
            var update = Builders<T>.Update.Set(propertyToUpdate, value);

            // update the document
            var updateResult = await MongoCollection.UpdateOneAsync(filter, update);

            // if the query is not acknowledged, throw exception
            if (!updateResult.IsAcknowledged)
            {
                Throw.Database<T>(EDatabaseMethod.Update, id);
                return;
            }

            // if there is no item with the given id, throw exception
            if (updateResult.MatchedCount <= 0)
                Throw.NotFound<T>(id);
        }

        #endregion update

        #region delete

        /// <inheritdoc cref="IDataService{T}.RemoveAsync" />
        /// <summary>
        /// Remove removes the <see cref="T"/> with the given id from the database.
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/> to remove in the database</param>
        /// <exception cref="MongoException">when the query was not acknowledged</exception>
        /// <exception cref="NotFoundException">when there was no item to remove</exception>
        public async Task RemoveAsync(ObjectId id)
        {
            // remove the document from the database with the given id
            var deleteResult = await MongoCollection.DeleteOneAsync(x => x.Id == id);

            // if the query is not acknowledged, throw exception
            if (!deleteResult.IsAcknowledged)
            {
                Throw.Database<T>(EDatabaseMethod.Update, id);
                return;
            }

            // if there is no item with the given id, throw exception
            if (deleteResult.DeletedCount <= 0)
                Throw.NotFound<T>(id);
        }

        #endregion delete

        #endregion METHODS
    }
}