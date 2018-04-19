using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models.Bases;
using ArgumentNullException = WebService.Helpers.Exceptions.ArgumentNullException;

namespace WebService.Services.Data.Mongo
{
    public abstract class AMongoDataService<T> : IDataService<T> where T : IModelWithID
    {
        #region CONSTRUCTORS

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

        public virtual async Task CreateAsync(T item)
        {
            await CreateAsync(item, true);
        }

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

        public virtual async Task AddItemToListProperty<TValue>(ObjectId id,
            Expression<Func<T, IEnumerable<TValue>>> propertyToAddItemTo, TValue itemToAdd)
        {
            if (itemToAdd == null)
                throw new ArgumentNullException(nameof(itemToAdd));
            if (itemToAdd is IModelWithID modelWithID)
                modelWithID.Id = ObjectId.GenerateNewId();

            var updater = Builders<T>.Update.Push(propertyToAddItemTo, itemToAdd);
            var result = await MongoCollection.FindOneAndUpdateAsync(x => x.Id == id, updater);

            if (result == null)
                throw new NotFoundException<T>(nameof(IModelWithID.Id), id.ToString());
        }

        #endregion create


        #region read

        public virtual async Task<IEnumerable<T>> GetAsync(
            IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null)
        {
            return await MongoCollection
                .Find(FilterDefinition<T>.Empty)
                .Select(propertiesToInclude)
                .ToListAsync();
        }

        public virtual async Task<T> GetOneAsync(ObjectId id,
            IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null)
        {
            return await GetByAsync(x => x.Id == id, propertiesToInclude);
        }

        public virtual async Task<TOut> GetPropertyAsync<TOut>(ObjectId id, Expression<Func<T, TOut>> propertyToSelect)
        {
            return await GetPropertyByAsync(x => x.Id == id, propertyToSelect);
        }

        protected async Task<T> GetByAsync(Expression<Func<T, bool>> condition,
            IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null)
        {
            var find = MongoCollection.Find(condition);

            if (find.Count() <= 0)
                throw new NotFoundException<T>();

            return await find
                .Select(propertiesToInclude)
                .FirstOrDefaultAsync();
        }

        protected async Task<TOut> GetPropertyByAsync<TOut>(Expression<Func<T, bool>> condition,
            Expression<Func<T, TOut>> propertyToSelect)
        {
            if (propertyToSelect == null)
                throw new ArgumentNullException(nameof(propertyToSelect));

            var find = MongoCollection.Find(condition);

            if (find.Count() <= 0)
                throw new NotFoundException<T>();

            var selector = Builders<T>.Projection.Include(new ExpressionFieldDefinition<T>(propertyToSelect));
            
            var item = await find
                .Project<T>(selector)
                .FirstOrDefaultAsync();

            return propertyToSelect.Compile()(item);
        }

        #endregion read


        #region update

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

            var update = Builders<T>.Update.Set(x => x.Id, newItem.Id);
            foreach (var selector in propertiesToUpdate)
            {
                var prop = selector.Body is MemberExpression expression
                    ? expression.Member as PropertyInfo
                    : ((MemberExpression) ((UnaryExpression) selector.Body).Operand).Member as PropertyInfo;

                if (prop != null)
                    update = update.Set(selector, prop.GetValue(newItem));
            }

            var updateResult = await MongoCollection.UpdateOneAsync(x => x.Id == newItem.Id, update);

            if (!updateResult.IsAcknowledged)
                throw new DatabaseException(EDatabaseMethod.Update);
            if (updateResult.MatchedCount <= 0)
                throw new NotFoundException<T>(nameof(IModelWithID.Id), newItem.Id.ToString());
        }

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

        public virtual async Task UpdatePropertyAsync<TValue>(ObjectId id, Expression<Func<T, TValue>> propertyToUpdate,
            TValue value)
        {
            if (propertyToUpdate == null)
                throw new ArgumentNullException(nameof(propertyToUpdate));

            var update = Builders<T>.Update.Set(propertyToUpdate, value);
            var updateResult = await MongoCollection.UpdateOneAsync(x => x.Id == id, update);

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

            var update = Builders<T>.Update.Pull(popertyToRemoveItemFrom, itemToRemove);
            var result = await MongoCollection.FindOneAndUpdateAsync(x => x.Id == id, update);

            if (result == null)
                throw new NotFoundException<T>(nameof(IModelWithID.Id), id.ToString());
        }

        #endregion delete

        #endregion METHODS
    }
}