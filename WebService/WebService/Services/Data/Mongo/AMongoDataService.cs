using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using WebService.Helpers;
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

        /// <inheritdoc cref="IDataService{T}.GetAsync" />
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
            // if the proeprties are null or there are none, return all the properties
            if (EnumerableExtensions.IsNullOrEmpty(properties))
                return await foundItems.ToListAsync();

            // create a propertyfilter
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
        public async Task<string> CreateAsync(T item)
        {
            // create a new id for the new item
            item.Id = ObjectId.GenerateNewId();
            // save the new item to the database
            await MongoCollection.InsertOneAsync(item);

            // check if the new item was created
            return MongoCollection
                       .Find(x => x.Id == item.Id)
                       .FirstOrDefaultAsync() != null
                // if it is, return the id
                ? item.Id.ToString()
                // else return null
                : null;
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
            // return true if something acutaly happened
            return result.IsAcknowledged && result.DeletedCount > 0;
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
        public async Task<T> UpdateAsync(T newItem, IEnumerable<Expression<Func<T, object>>> propertiesToUpdate = null)
        {
            // create list of the enumerable to prevent multiple enumerations of enumerable
            var propertiesToUpdateList = propertiesToUpdate?.ToList();

            // check if thereare properties to update.
            if (EnumerableExtensions.IsNullOrEmpty(propertiesToUpdateList))
            {
                // if there are no properties in the liest, replace the document
                var replaceOneResult = await MongoCollection.ReplaceOneAsync(x => x.Id == newItem.Id, newItem);
                // check if something was replaced
                return replaceOneResult.IsAcknowledged && replaceOneResult.ModifiedCount > 0
                    // if something was replaced, return the new newItem
                    ? await MongoCollection
                        .Find(x => x.Id == newItem.Id)
                        .FirstOrDefaultAsync()
                    // else return null
                    : default(T);
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
                    // if that failse, unary expression
                    : ((MemberExpression) ((UnaryExpression) selector.Body).Operand).Member as PropertyInfo;

                // check if the property exists
                if (prop != null)
                    // if it does, add the selector and value to the updateDefinition
                    update = update.Set(selector, prop.GetValue(newItem));
            }

            // update the document
            var updateResult = await MongoCollection.UpdateOneAsync(filter, update);

            // check if something was updated
            return updateResult.IsAcknowledged
                // if something was updated, return the new newItem
                ? await MongoCollection
                    .Find(x => x.Id == newItem.Id)
                    .FirstOrDefaultAsync()
                // else return null;
                : default(T);
        }

        #endregion METHODS
    }
}