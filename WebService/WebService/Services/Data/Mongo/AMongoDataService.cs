using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models.Bases;

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

        /// <inheritdoc cref="IDataService{T}.GetPropertyAsync"/>
        /// <summary>
        /// GetPropertyAsync is supposed to return a single property of the <see cref="T"/> with the given id
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/> to get the property from</param>
        /// <param name="propertyToSelect">is the selector to select the property to return</param>
        /// <returns>The value of the asked property</returns>
        public async Task<object> GetPropertyAsync(ObjectId id, Expression<Func<T, object>> propertyToSelect)
        {
            if (propertyToSelect == null)
                throw new ArgumentNullException(nameof(propertyToSelect),
                    "the property to select selector cannot be null");

            // get the item with the given id
            var foundItems = MongoCollection.Find(x => x.Id == id);

            if (foundItems.Count() <= 0)
                throw new NotFoundException($"no {typeof(T).Name} could be found with the id {id}");

            // create a property filter
            var selector = Builders<T>.Projection.Include(propertyToSelect);

            // execute the query
            var items = await foundItems
                .Project<T>(selector)
                .ToListAsync();

            // return the asked property
            return items
                .Select(propertyToSelect.Compile())
                .FirstOrDefault();
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
        public async Task<T> GetAsync(ObjectId id, IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null)
        {
            // get the item with the given id
            var foundItem = MongoCollection.Find(x => x.Id == id);

            if (foundItem.Count() <= 0)
                throw new NotFoundException($"there is no {typeof(T).Name} with the id {id}");

            // convert the properties to include to a list (if not null)
            var properties = propertiesToInclude?.ToList();
            // if the properties are null or there are none, return all the properties
            if (EnumerableExtensions.IsNullOrEmpty(properties))
                return await foundItem.FirstOrDefaultAsync();

            // create a property filter
            var selector = Builders<T>.Projection.Include(x => x.Id);

            //ReSharper disable once PossibleNullReferenceException
            // iterate over all the properties and add them to the filter
            foreach (var property in properties)
                selector = selector.Include(property);

            // return the item
            return await foundItem
                // filter the properties
                .Project<T>(selector)
                // execute the query
                .FirstOrDefaultAsync();
        }

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

            // convert the properties to include to a list (if not null)
            var properties = propertiesToInclude?.ToList();
            // if the properties are null or there are none, return all the properties
            if (EnumerableExtensions.IsNullOrEmpty(properties))
                return await foundItems.ToListAsync();

            // create a property filter
            var selector = Builders<T>.Projection.Include(x => x.Id);

            //ReSharper disable once PossibleNullReferenceException
            // iterate over all the properties and add them to the filter
            foreach (var property in properties)
                selector = selector.Include(property);

            // return the items
            return foundItems
                // filter the properties
                .Project<T>(selector)
                // execute the query
                .ToList();
        }

        /// <inheritdoc cref="IDataService{T}.CreateAsync" />
        /// <summary>
        /// Create saves the passed <see cref="T"/> to the database.
        /// <para/>
        /// If the new item is created, the method returns the id of the new <see cref="T"/>, else null.
        /// </summary>
        /// <param name="item">is the <see cref="T"/> to save in the database</param>
        /// <returns>
        /// - the new id if the <see cref="T"/> was created in the database
        /// - null if the new item was not created
        /// </returns>
        public async Task<bool> CreateAsync(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "the item to create cannot be null");

            // create a new id for the new item
            item.Id = ObjectId.GenerateNewId();
            // save the new item to the database
            await MongoCollection.InsertOneAsync(item);

            // check if the new item was created
            return MongoCollection
                .Find(x => x.Id == item.Id)
                .Any();
        }

        /// <inheritdoc cref="IDataService{T}.RemoveAsync" />
        /// <summary>
        /// Remove removes the <see cref="T"/> with the given id from the database.
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/> to remove in the database</param>
        /// <returns>
        /// - true if the <see cref="T"/> was removed from the database
        /// - false if the item was not removed
        /// </returns>
        public async Task<bool> RemoveAsync(ObjectId id)
        {
            // remove the document from the database with the given id
            var result = await MongoCollection.DeleteOneAsync(x => x.Id == id);

            if (!result.IsAcknowledged)
                throw new MongoException("the delete request could not be acknowledged by the database");

            if (result.DeletedCount <= 0)
                throw new NotFoundException($"there is no {typeof(T).Name} id {id}");

            return true;
        }

        /// <inheritdoc cref="IDataService{T}.UpdateAsync" />
        /// <summary>
        /// Update updates the <see cref="T" /> with the id of the given <see cref="T" />.
        /// <para />
        /// The updated properties are defined in the <see cref="propertiesToUpdate" /> parameter.
        /// If the <see cref="propertiesToUpdate" /> parameter is null or empty (which it is by default), all properties are updated.
        /// </summary>
        /// <param name="newItem">is the <see cref="T" /> to update</param>
        /// <param name="propertiesToUpdate">are the properties that need to be updated</param>
        /// <returns>The updated newItem</returns>
        public async Task<bool> UpdateAsync(T newItem,
            IEnumerable<Expression<Func<T, object>>> propertiesToUpdate = null)
        {
            if (newItem == null)
                throw new ArgumentNullException(nameof(newItem), "the item to update cannot be null");

            // create list of the enumerable to prevent multiple enumerations of enumerable
            var propertiesToUpdateList = propertiesToUpdate?.ToList();

            // check if there are properties to update.
            if (EnumerableExtensions.IsNullOrEmpty(propertiesToUpdateList))
            {
                // if there are no properties in the list, replace the document
                var replaceOneResult = await MongoCollection.ReplaceOneAsync(x => x.Id == newItem.Id, newItem);
             
                if (!replaceOneResult.IsAcknowledged)
                    throw new MongoException(
                        $"the database could not update property of the {typeof(T).Name} with id {newItem.Id}");

                if (replaceOneResult.MatchedCount <= 0)
                    throw new NotFoundException($"the {typeof(T).Name} with id {newItem.Id} could not be found");

                // return whether something was updated
                return true;
            }

            // create a filter that filters on id
            var filter = Builders<T>.Filter.Eq(x => x.Id, newItem.Id);

            // create an update definition.
            // since there needs to be an updateDefinition to start from, update the id, that is the same for the old an new object
            var update = Builders<T>.Update.Set(x => x.Id, newItem.Id);

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
                    update = update.Set(selector, prop.GetValue(newItem));
            }

            // update the document
            var updateResult = await MongoCollection.UpdateOneAsync(filter, update);

            if (!updateResult.IsAcknowledged)
                throw new MongoException(
                    $"the database could not update property of the {typeof(T).Name} with id {newItem.Id}");

            if (updateResult.MatchedCount <= 0)
                throw new NotFoundException($"the {typeof(T).Name} with id {newItem.Id} could not be found");

            // return whether something was updated
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
                throw new ArgumentNullException(nameof(propertyToUpdate),
                    "the property to update selector cannot be null");

            var property = propertyToUpdate.Body is MemberExpression expression
                // via member expression
                ? expression.Member as PropertyInfo
                // via unary expression
                : ((MemberExpression) ((UnaryExpression) propertyToUpdate.Body).Operand).Member as PropertyInfo;

            if (property == null)
                throw new ArgumentException($"the property could nod be found on object {typeof(T).Name}",
                    nameof(propertyToUpdate));

            if (!property.PropertyType.IsInstanceOfType(value))
                throw new ArgumentException(
                    $"the value of type {value.GetType().Name} cannot be assigned to the property of type {property.PropertyType}",
                    nameof(value));

            // create a filter that filters on id
            var filter = Builders<T>.Filter.Eq(x => x.Id, id);
            // create an update definition.
            var update = Builders<T>.Update.Set(propertyToUpdate, value);

            // update the document
            var updateResult = await MongoCollection.UpdateOneAsync(filter, update);

            if (!updateResult.IsAcknowledged)
                throw new MongoException(
                    $"the database could not update property of the {typeof(T).Name} with id {id}");

            if (updateResult.MatchedCount <= 0)
                throw new NotFoundException($"the {typeof(T).Name} with id {id} could not be found");

            // return whether something was updated
            return true;
        }

        #endregion METHODS
    }
}