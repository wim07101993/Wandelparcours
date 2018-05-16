using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models.Bases;
using ArgumentNullException = WebService.Helpers.Exceptions.ArgumentNullException;

namespace WebService.Services.Data.Mongo
{
    /// <inheritdoc />
    /// <summary>
    /// An abstract class to preform the basic CRUD operations on a Mongo Database.
    /// </summary>
    /// <typeparam name="T">Type of the entities to preform the CRUD operations on</typeparam>
    public abstract class AMongoDataService<T> : IDataService<T>
        where T : IModelWithID
    {
        #region CONSTRUCTORS

        /// <param name="connectionString">The connection string to connect with the database</param>
        /// <param name="databaseName">Name of the database to connect to</param>
        /// <param name="collectionName">Collection in the database to get entities from</param>
        protected AMongoDataService(string connectionString, string databaseName, string collectionName)
        {
            MongoCollection = new MongoClient(connectionString)
                .GetDatabase(databaseName)
                .GetCollection<T>(collectionName);
        }

        #endregion CONSTRCUTORS


        #region PROPERTIES

        public IMongoCollection<T> MongoCollection { get; }

        #endregion PROPERTIES


        #region METHDOS

        #region create

        /// <inheritdoc />
        public virtual async Task CreateAsync(T item)
            => await CreateAsync(item, true);

        /// <summary>
        /// Creates a new <see cref="T"/> in the database.
        /// </summary>
        /// <param name="item">The <see cref="T"/> to create in the database</param>
        /// <param name="generateNewId">
        /// Boolean to indicate whether a new id should be generated for the <see cref="T"/>
        /// </param>
        /// <exception cref="ArgumentNullException">If <see cref="item"/> is null, it can't be created</exception>
        /// <exception cref="DatabaseException">Throws when the database throws an exception.</exception>
        protected virtual async Task CreateAsync(T item, bool generateNewId)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (generateNewId)
                item.Id = ObjectId.GenerateNewId();

            try
            {
                await MongoCollection.InsertOneAsync(item);
            }
            catch (Exception e)
            {
                throw new DatabaseException(EDatabaseMethod.Create, e);
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">If <see cref="itemToAdd"/> is null, it can't be added</exception>
        /// <exception cref="NotFoundException{T}">
        /// If there is no item with the id <see cref="id"/>, it cant'be modified.
        /// </exception>
        public virtual async Task AddItemToListProperty<TValue>(ObjectId id,
            Expression<Func<T, IEnumerable<TValue>>> propertyToAddItemTo, TValue itemToAdd)
        {
            if (itemToAdd == null)
                throw new ArgumentNullException(nameof(itemToAdd));
            if (itemToAdd is IModelWithID modelWithID)
                modelWithID.Id = ObjectId.GenerateNewId();

            // create an update query to update the nested list
            var updater = Builders<T>.Update.Push(propertyToAddItemTo, itemToAdd);
            var result = await MongoCollection.FindOneAndUpdateAsync(x => x.Id == id, updater);

            if (result == null)
                throw new NotFoundException<T>(nameof(IModelWithID.Id), id.ToString());
        }

        #endregion create


        #region read

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> GetAsync(
            IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null)
            => await MongoCollection
                .Find(FilterDefinition<T>.Empty)
                .Select(propertiesToInclude)
                .ToListAsync();

        /// <inheritdoc />
        public virtual async Task<T> GetOneAsync(ObjectId id,
            IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null)
            => await GetByAsync(x => x.Id == id, propertiesToInclude);

        /// <inheritdoc />
        public virtual async Task<TOut> GetPropertyAsync<TOut>(ObjectId id, Expression<Func<T, TOut>> propertyToSelect)
            => await GetPropertyByAsync(x => x.Id == id, propertyToSelect);

        /// <summary>
        /// Gets a <see cref="T"/> from the database if a condition is met.
        /// </summary>
        /// <param name="condition">The condition that needs to be fullfilled to return the <see cref="T"/></param>
        /// <param name="propertiesToInclude">
        /// The properties to include in the query (if it is null, all properties are passed).
        /// </param>
        /// <returns>The item for which the condition returns true</returns>
        /// <exception cref="NotFoundException{T}">
        /// Throws if there is no <see cref="T"/> for which the condition returns true.
        /// </exception>
        /// <exception cref="DatabaseException">Throws when the database throws an exception.</exception>
        protected async Task<T> GetByAsync(Expression<Func<T, bool>> condition,
            IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null)
        {
            try
            {
                var find = MongoCollection.Find(condition);

                if (!find.Any())
                    throw new NotFoundException<T>();

                return await find
                    .Select(propertiesToInclude)
                    .FirstOrDefaultAsync();
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new DatabaseException(EDatabaseMethod.Read, e);
            }
        }

        /// <summary>
        /// Gets a property of a <see cref="T"/> from the database if a condition is met.
        /// </summary>
        /// <param name="condition">The condition that needs to be fullfilled to return the <see cref="T"/></param>
        /// <param name="propertyToSelect">The property to get the value of</param>
        /// <typeparam name="TOut">Type of the value to get.</typeparam>
        /// <returns>The property of the item for which the condition returns true.</returns>
        /// <exception cref="ArgumentNullException">If the propertyToSelect is null, nothing can't be returned.</exception>
        /// <exception cref="NotFoundException{T}">
        /// Throws if there is no <see cref="T"/> for which the condition returns true.
        /// </exception>
        /// <exception cref="DatabaseException">Throws when the database throws an exception.</exception>
        protected async Task<TOut> GetPropertyByAsync<TOut>(Expression<Func<T, bool>> condition,
            Expression<Func<T, TOut>> propertyToSelect)
        {
            if (propertyToSelect == null)
                throw new ArgumentNullException(nameof(propertyToSelect));

            try
            {
                var find = MongoCollection.Find(condition);

                if (!find.Any())
                    throw new NotFoundException<T>();

                // build a query to get the single property
                var projector = Builders<T>.Projection.Expression(propertyToSelect);

                return await find
                    .Project(projector)
                    .FirstOrDefaultAsync();
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new DatabaseException(EDatabaseMethod.Read, e);
            }
        }

        #endregion read


        #region update

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">If the new item is null, there is no id to compare to.</exception>
        /// <exception cref="NotFoundException{T}">
        /// Throws if there is no <see cref="T"/> with the same id as <see cref="newItem"/>
        /// </exception>
        /// <exception cref="DatabaseException">Throws when the database throws an exception.</exception>
        public virtual async Task UpdateAsync(T newItem,
            IEnumerable<Expression<Func<T, object>>> propertiesToUpdate = null)
        {
            if (propertiesToUpdate == null)
            {
                await ReplaceAsync(newItem);
                return;
            }

            if (newItem == null)
                throw new ArgumentNullException(nameof(newItem));

            // build a query to update only the properties in propertiesToUpdate in the item
            var update = Builders<T>.Update.Set(x => x.Id, newItem.Id);
            update = propertiesToUpdate.Aggregate(
                update, (current, selector) => current.Set(selector, selector.Compile()(newItem)));

            var updateResult = await MongoCollection.UpdateOneAsync(x => x.Id == newItem.Id, update);

            if (!updateResult.IsAcknowledged)
                throw new DatabaseException(EDatabaseMethod.Update);
            if (updateResult.MatchedCount <= 0)
                throw new NotFoundException<T>(nameof(IModelWithID.Id), newItem.Id.ToString());
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">If the new item is null, there is no id to compare to.</exception>
        /// <exception cref="NotFoundException{T}">
        /// Throws if there is no <see cref="T"/> with the same id as <see cref="newItem"/>
        /// </exception>
        /// <exception cref="DatabaseException">Throws when the database throws an exception.</exception>
        public virtual async Task ReplaceAsync(T newItem)
        {
            if (newItem == null)
                throw new ArgumentNullException(nameof(newItem));

            var replaceOneResult = await MongoCollection.ReplaceOneAsync(x => x.Id == newItem.Id, newItem);

            if (!replaceOneResult.IsAcknowledged)
                throw new DatabaseException(EDatabaseMethod.Replace);
            if (replaceOneResult.MatchedCount <= 0)
                throw new NotFoundException<T>(nameof(IModelWithID.Id), newItem.Id.ToString());
        }

        /// <inheritdoc />
        public virtual async Task UpdatePropertyAsync<TValue>(ObjectId id, Expression<Func<T, TValue>> propertyToUpdate,
            TValue value)
        {
            if (propertyToUpdate == null)
                throw new ArgumentNullException(nameof(propertyToUpdate));

            // build a query to only update the propertie in propertyToUpdate
            var updater = Builders<T>.Update.Set(propertyToUpdate, value);
            var updateResult = await MongoCollection.UpdateOneAsync(x => x.Id == id, updater);

            if (!updateResult.IsAcknowledged)
                throw new DatabaseException(EDatabaseMethod.Update);
            if (updateResult.MatchedCount <= 0)
                throw new NotFoundException<T>(nameof(IModelWithID.Id), id.ToString());
        }

        #endregion update


        #region delete

        public virtual async Task RemoveAsync(ObjectId id)
            => await RemoveByAsync(x => x.Id == id);

        protected async Task RemoveByAsync(Expression<Func<T, bool>> condition)
        {
            var deleteResult = await MongoCollection.DeleteOneAsync(condition);

            if (!deleteResult.IsAcknowledged)
                throw new DatabaseException(EDatabaseMethod.Delete);
            if (deleteResult.DeletedCount <= 0)
                throw new NotFoundException<T>();
        }

        public virtual async Task RemoveItemFromList<TValue>(ObjectId id,
            Expression<Func<T, IEnumerable<TValue>>> popertyToRemoveItemFrom, TValue itemToRemove)
        {
            if (itemToRemove == null)
                throw new ArgumentNullException(nameof(itemToRemove));

            // build a query to remove an item from a nested list
            var updater = Builders<T>.Update.Pull(popertyToRemoveItemFrom, itemToRemove);
            var result = await MongoCollection.FindOneAndUpdateAsync(x => x.Id == id, updater);

            if (result == null)
                throw new NotFoundException<T>(nameof(IModelWithID.Id), id.ToString());
        }

        #endregion delete

        #endregion METHODS
    }
}