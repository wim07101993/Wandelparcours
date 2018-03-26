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
using ArgumentNullException = WebService.Helpers.Exceptions.ArgumentNullException;

namespace WebService.Services.Data.Mongo
{
    public abstract class AMongoDataService<T> : IDataService<T> where T : IModelWithID
    {
        #region PROPERTIES

        /// <summary>
        /// MongoCollection is the mongo collection to query items.
        /// </summary>
        public abstract IMongoCollection<T> MongoCollection { get; }

        #endregion PROPERTIES


        #region METHDOS

        #region create

        public virtual async Task CreateAsync(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            item.Id = ObjectId.GenerateNewId();

            try
            {
                // save the new item to the database
                await MongoCollection.InsertOneAsync(item);
            }
            catch (Exception e)
            {
                throw new DatabaseException(EDatabaseMethod.Create, e);
            }
        }

        public virtual async Task AddItemToListProperty<TValue>(ObjectId id,
            Expression<Func<T, IEnumerable<TValue>>> propertyToAddItemTo, TValue itemToAdd)
        {
            if (itemToAdd == null)
                throw new ArgumentNullException(nameof(itemToAdd));

            var filter = Builders<T>.Filter.Eq(x => x.Id, id);
            var updater = Builders<T>.Update.Push(propertyToAddItemTo, itemToAdd);

            if (itemToAdd is IModelWithID modelWithID)
                modelWithID.Id = ObjectId.GenerateNewId();

            var result = await MongoCollection.FindOneAndUpdateAsync(filter, updater);

            if (result == null)
                throw new NotFoundException<T>(nameof(IModelWithID.Id), id.ToString());
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
        public virtual async Task<IEnumerable<T>> GetAsync(
            IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null)
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

        /// <inheritdoc cref="IDataService{T}.GetOneAsync" />
        /// <summary>
        /// GetOneAsync returns the <see cref="T"/> with the given id from the database. 
        /// <para/>
        /// It only fills the properties passed in the <see cref="propertiesToInclude"/> parameter. The id is always passed and 
        /// if the <see cref="propertiesToInclude"/> parameter is null (which it is by default), all the properties are included. 
        /// </summary>
        /// <param name="id">is the id of the item that needs to be fetched</param>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>An <see cref="IEnumerable{T}"/> filled with all the ts in the database.</returns>
        /// <exception cref="NotFoundException">when there is no item found with the given id</exception>
        public virtual async Task<T> GetOneAsync(ObjectId id,
            IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null)
        {
            // get the item with the given id
            var find = MongoCollection.Find(x => x.Id == id);

            // if there is no item with the given id, throw exception
            if (find.Count() <= 0)
                throw new NotFoundException<T>(nameof(IModelWithID.Id), id.ToString());

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
        /// <typeparam name="TOut">is the type of the value of the property</typeparam>
        /// <returns>The value of the asked property</returns>
        /// <exception cref="System.ArgumentNullException">when the property to select is null</exception>
        /// <exception cref="NotFoundException">when there is no item with the given id</exception>
        public virtual async Task<TOut> GetPropertyAsync<TOut>(ObjectId id, Expression<Func<T, TOut>> propertyToSelect)
        {
            // if the property to select is null, throw exception
            if (propertyToSelect == null)
                throw new ArgumentNullException(nameof(propertyToSelect));

            // get the item with the given id
            var find = MongoCollection.Find(x => x.Id == id);

            // if there is no item with the given id, throw exception
            if (find.Count() <= 0)
                throw new NotFoundException<T>(nameof(IModelWithID.Id), id.ToString());

            var fieldDef = new ExpressionFieldDefinition<T>(propertyToSelect);
            // create a property filter
            var selector = Builders<T>.Projection.Include(fieldDef);

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
        /// <exception cref="System.ArgumentNullException">when the new item is null</exception>
        /// <exception cref="MongoException">when the query was not acknowledged</exception>
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
                throw new ArgumentNullException(nameof(newItem));

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
                throw new DatabaseException(EDatabaseMethod.Update);
            // if there is no item with the given id, throw exception
            if (updateResult.MatchedCount <= 0)
                throw new NotFoundException<T>(nameof(IModelWithID.Id), newItem.Id.ToString());
        }

        /// <summary>
        /// ReplaceAsync replaces an item in the database with the new item. The item to replace is the item with the same id as the newItem
        /// </summary>
        /// <param name="newItem">is the new <see cref="T"/></param>
        /// <exception cref="System.ArgumentNullException">when the new item is null</exception>
        /// <exception cref="MongoException">when the query was not acknowledged</exception>
        /// <exception cref="NotFoundException">when there was no item with the same id as the newItem</exception>
        protected virtual async Task ReplaceAsync(T newItem)
        {
            // if the new item is null, throw exception
            if (newItem == null)
                throw new ArgumentNullException(nameof(newItem));

            // if there are no properties in the list, replace the document
            var replaceOneResult = await MongoCollection.ReplaceOneAsync(x => x.Id == newItem.Id, newItem);

            // if the query is not acknowledged, throw exception
            if (!replaceOneResult.IsAcknowledged)
                throw new DatabaseException(EDatabaseMethod.Replace);

            // if there is no item with the given id, throw exception
            if (replaceOneResult.MatchedCount <= 0)
                throw new NotFoundException<T>(nameof(IModelWithID.Id), newItem.Id.ToString());
        }

        /// <inheritdoc cref="IDataService{T}.UpdatePropertyAsync{TValue}" />
        /// <summary>
        /// GetPropertyAsync updates a single property of the <see cref="T"/> with the given id
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/> to get the property from</param>
        /// <param name="propertyToUpdate">is the selector to select the property to update</param>
        /// <param name="value">is the new value of the property</param>
        /// <exception cref="System.ArgumentNullException">when there is no property to update</exception>
        /// <exception cref="System.ArgumentException">when the property does not exist on <see cref="T"/></exception>
        /// <exception cref="System.ArgumentException">when the value is of the wrong type</exception>
        /// <exception cref="MongoException">when the query was not acknowledged</exception>
        /// <exception cref="NotFoundException">when there was no item with the same id as the newItem</exception>
        public virtual async Task UpdatePropertyAsync<TValue>(ObjectId id, Expression<Func<T, TValue>> propertyToUpdate,
            TValue value)
        {
            // if there is no property to update, throw exception
            if (propertyToUpdate == null)
                throw new ArgumentNullException(nameof(propertyToUpdate));

            // create a filter that filters on id
            var filter = Builders<T>.Filter.Eq(x => x.Id, id);
            // create an update definition.
            var update = Builders<T>.Update.Set(propertyToUpdate, value);

            // update the document
            var updateResult = await MongoCollection.UpdateOneAsync(filter, update);

            // if the query is not acknowledged, throw exception
            if (!updateResult.IsAcknowledged)
                throw new DatabaseException(EDatabaseMethod.Update);

            // if there is no item with the given id, throw exception
            if (updateResult.MatchedCount <= 0)
                throw new NotFoundException<T>(nameof(IModelWithID.Id), id.ToString());
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
        public virtual async Task RemoveAsync(ObjectId id)
        {
            // remove the document from the database with the given id
            var deleteResult = await MongoCollection.DeleteOneAsync(x => x.Id == id);

            // if the query is not acknowledged, throw exception
            if (!deleteResult.IsAcknowledged)
                throw new DatabaseException(EDatabaseMethod.Delete);

            // if there is no item with the given id, throw exception
            if (deleteResult.DeletedCount <= 0)
                throw new NotFoundException<T>(nameof(IModelWithID.Id), id.ToString());
        }

        #endregion delete

        #endregion METHODS
    }
}