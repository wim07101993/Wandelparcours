using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using WebService.Helpers;
using WebService.Models;

namespace WebService.Services.Data
{
    /// <inheritdoc cref="IDataService"/>
    /// <summary>
    /// MongoDataService is a class that implements the <see cref="IDataService"/> interface.
    /// <para/>
    /// It handles the saving and retreiving data to and from the mongo database.
    /// <para/>
    /// The connectionstring, db name and collections that are used are stored in the IConfiguration dependency under the Database object.
    /// </summary>
    public class MongoDataService : IDataService
    {
        #region FIELDS

        /// <summary>
        /// _collection is the mongo collection to query residents.
        /// </summary>
        private readonly IMongoCollection<Resident> _collection;

        #endregion FIELDS


        #region CONSTRUCTORS

        /// <summary>
        /// MongoDataService is the contsructor to create an instance of the <see cref="MongoDataService"/> class.
        /// <para/>
        /// The connectionstring, db name and collections that are used are stored in the IConfiguration dependency under the Database object.
        /// </summary>
        /// <param name="config"></param>
        public MongoDataService(IConfiguration config)
        {
            // get the residents mongo collection
            _collection =
                // create a new client
                new MongoClient(config["Database:ConnectionString"])
                    // get the database
                    .GetDatabase(config["Database:DatabaseName"])
                    // get the collection
                    .GetCollection<Resident>(config["Database:ResidentsCollectionName"]);
        }

        #endregion CONSTRUCTORS

        /// <inheritdoc cref="IDataService.GetResidents" />
        /// <summary>
        /// GetResidents returns all the residents from the database. 
        /// <para />
        /// It only fills the properties passed in the <see cref="propertiesToInclude" /> parameter. The id is always passed and 
        /// if the <see cref="propertiesToInclude" /> parameter is null (which it is by default), all the properties are included.
        /// Other properties are given their default value. 
        /// </summary>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>An <see cref="IEnumerable{T}"/> filled with all the residents in the database.</returns>
        public IEnumerable<Resident> GetResidents(
            IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null)
        {
            // get all the items
            var foundItems = _collection.Find(FilterDefinition<Resident>.Empty);

            // convert the properties to include to a list (if not null)
            var properties = propertiesToInclude?.ToList();
            // if the proeprties are null or there are none, return all the properties
            if (EnumerableExtensions.IsNullOrEmpty(properties))
                return foundItems.ToList();

            // create a propertyfilter
            var selector = Builders<Resident>.Projection.Include(x => x.ID);

            //ReSharper disable once PossibleNullReferenceException
            // iterate over all the properties and add them to the filter
            foreach (var property in properties)
                selector = selector.Include(property);

            // return the items
            return foundItems
                // filter the properties
                .Project<Resident>(selector)
                // execute the query
                .ToList();
        }

        /// <inheritdoc cref="IDataService.CreateResident" />
        /// <summary>
        /// CreateResident saves the passed <see cref="Resident"/> to the database.
        /// <para/>
        /// If the newResident is created, the method returns the id of the new <see cref="Resident"/>, else null.
        /// </summary>
        /// <param name="resident">is the <see cref="Resident"/> to save in the database</param>
        /// <returns>
        /// - the new id if the <see cref="Resident"/> was created in the database
        /// - null if the newResident was not created
        /// </returns>
        public string CreateResident(Resident resident)
        {
            // create a new ide for the newResident
            resident.ID = ObjectId.GenerateNewId();
            // save the new newResident to the database
            _collection.InsertOne(resident);

            // check if the newResident was created
            return _collection
                       .Find(x => x.ID == resident.ID)
                       .FirstOrDefault() != null
                // if it is, return the id
                ? resident.ID.ToString()
                // else return null
                : null;
        }

        /// <inheritdoc cref="IDataService.RemoveResident" />
        /// <summary>
        /// RemoveResident removes the <see cref="Resident"/> with the given id from the database.
        /// </summary>
        /// <param name="id">is the id of the <see cref="Resident"/> to remove in the database</param>
        /// <returns>
        /// - true if the <see cref="Resident"/> was removed from the database
        /// - false if the newResident was not removed
        /// </returns>
        public bool RemoveResident(ObjectId id)
        {
            // remove the document from the database with the given id
            var result = _collection.DeleteOne(x => x.ID == id);
            // return true if something acutaly happened
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        /// <inheritdoc cref="IDataService.UpdateResident" />
        /// <summary>
        /// UpdateResident updates the <see cref="T:WebService.Models.Resident" /> with the id of the given <see cref="T:WebService.Models.Resident" />.
        /// <para />
        /// The updated properties are defined in the <see cref="!:propertiesToUpdate" /> parameter.
        /// If the <see cref="!:propertiesToUpdate" /> parameter is null or empty (which it is by default), all properties are updated.
        /// </summary>
        /// <param name="newResident">is the <see cref="T:WebService.Models.Resident" /> to update</param>
        /// <param name="propertiesToUpdate">are the properties that need to be updated</param>
        /// <returns>The updated newResident</returns>
        public Resident UpdateResident(Resident newResident,
            IEnumerable<Expression<Func<Resident, object>>> propertiesToUpdate = null)
        {
            // create list of the enumerable to prevent multiple enumerations of enumerable
            var propertiesToUpdateList = propertiesToUpdate?.ToList();

            // check if thereare properties to update.
            if (EnumerableExtensions.IsNullOrEmpty(propertiesToUpdateList))
            {
                // if there are no properties in the liest, replace the document
                var replaceOneResult = _collection.ReplaceOne(x => x.ID == newResident.ID, newResident);
                // check if something was replaced
                return replaceOneResult.IsAcknowledged && replaceOneResult.ModifiedCount > 0
                    // if something was replaced, return the new newResident
                    ? _collection
                        .Find(x => x.ID == newResident.ID)
                        .ToList()
                        .FirstOrDefault()
                    // else return null
                    : null;
            }

            // create a filter that filters on id
            var filter = Builders<Resident>.Filter.Eq(x => x.ID, newResident.ID);

            // create an update definition.
            // since there needs to be an updateDefinition to start from, update the id, that is the same for the old an new object
            var update = Builders<Resident>.Update.Set(x => x.ID, newResident.ID);

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
                    update = update.Set(selector, prop.GetValue(newResident));
            }

            // update the document
            var updateResult = _collection.UpdateOne(filter, update);

            // check if something was updated
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0
                // if something was updated, return the new newResident
                ? _collection
                    .Find(x => x.ID == newResident.ID)
                    .ToList()
                    .FirstOrDefault()
                // else return null;
                : null;
        }
    }
}