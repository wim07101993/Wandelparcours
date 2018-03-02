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
        /// _residentsCollection is the mongo collection to query residents.
        /// </summary>
        private readonly IMongoCollection<Resident> _residentsCollection;

        /// <summary>
        /// _receiverModulesCollection is the mongo collection to query residents.
        /// </summary>
        private readonly IMongoCollection<ReceiverModule> _receiverModulesCollection;

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
            // create a new client and get the databas from it
            var db = new MongoClient(config["Database:ConnectionString"]).GetDatabase(config["Database:DatabaseName"]);

            // get the residents mongo collection
            _residentsCollection = db.GetCollection<Resident>(config["Database:ResidentsCollectionName"]);
            // get the receiver modules mongo collection
            _receiverModulesCollection = db.GetCollection<ReceiverModule>(config["Database:ReceiverModulesCollectionName"]);
        }

        #endregion CONSTRUCTORS


        #region METHDOS

        #region residents

        /// <inheritdoc cref="IDataService.GetResidents" />
        /// <summary>
        /// Get returns all the residents from the database. 
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
            var foundItems = _residentsCollection.Find(FilterDefinition<Resident>.Empty);

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
        /// Create saves the passed <see cref="Resident"/> to the database.
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
            _residentsCollection.InsertOne(resident);

            // check if the newResident was created
            return _residentsCollection
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
            var result = _residentsCollection.DeleteOne(x => x.ID == id);
            // return true if something acutaly happened
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        /// <inheritdoc cref="IDataService.UpdateResident" />
        /// <summary>
        /// UpdateResident updates the <see cref="T:WebService.Models.Value" /> with the id of the given <see cref="T:WebService.Models.Value" />.
        /// <para />
        /// The updated properties are defined in the <see cref="!:propertiesToUpdate" /> parameter.
        /// If the <see cref="!:propertiesToUpdate" /> parameter is null or empty (which it is by default), all properties are updated.
        /// </summary>
        /// <param name="newResident">is the <see cref="T:WebService.Models.Value" /> to update</param>
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
                var replaceOneResult = _residentsCollection.ReplaceOne(x => x.ID == newResident.ID, newResident);
                // check if something was replaced
                return replaceOneResult.IsAcknowledged && replaceOneResult.ModifiedCount > 0
                    // if something was replaced, return the new newResident
                    ? _residentsCollection
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
            var updateResult = _residentsCollection.UpdateOne(filter, update);

            // check if something was updated
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0
                // if something was updated, return the new newResident
                ? _residentsCollection
                    .Find(x => x.ID == newResident.ID)
                    .ToList()
                    .FirstOrDefault()
                // else return null;
                : null;
        }

        #endregion residents


        #region receiver modules

        /// <inheritdoc cref="IDataService.GetReceiverModules" />
        /// <summary>
        /// GetReceiverModules returns all the receivers from the database. 
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> filled with all the receivers in the database.</returns>
        public IEnumerable<ReceiverModule> GetReceiverModules()
            => _receiverModulesCollection.Find(FilterDefinition<ReceiverModule>.Empty).ToList();

        /// <inheritdoc cref="IDataService.CreateReceiverModule" />
        /// <summary>
        /// CreateReceiverModule saves the passed <see cref="ReceiverModule"/> to the database.
        /// <para/>
        /// If the newReceiverModule is created, the method returns the id of the new <see cref="ReceiverModule"/>, else null.
        /// </summary>
        /// <param name="receiver">is the <see cref="ReceiverModule"/> to save in the database</param>
        /// <returns>
        /// - the new id if the <see cref="ReceiverModule"/> was created in the database
        /// - null if the newReceiverModule was not created
        /// </returns>
        public string CreateReceiverModule(ReceiverModule receiver)
        {
            // create a new ide for the newReceiverModule
            receiver.ID = ObjectId.GenerateNewId();
            // save the new newReceiverModule to the database
            _receiverModulesCollection.InsertOne(receiver);

            // check if the newReceiverModule was created
            return _receiverModulesCollection
                       .Find(x => x.ID == receiver.ID)
                       .FirstOrDefault() != null
                // if it is, return the id
                ? receiver.ID.ToString()
                // else return null
                : null;
        }

        /// <inheritdoc cref="IDataService.RemoveReceiverModule" />
        /// <summary>
        /// RemoveReceiverModule removes the <see cref="ReceiverModule"/> with the given id from the database.
        /// </summary>
        /// <param name="id">is the id of the <see cref="ReceiverModule"/> to remove in the database</param>
        /// <returns>
        /// - true if the <see cref="ReceiverModule"/> was removed from the database
        /// - false if the newReceiverModule was not removed
        /// </returns>
        public bool RemoveReceiverModule(ObjectId id)
        {
            // remove the document from the database with the given id
            var result = _receiverModulesCollection.DeleteOne(x => x.ID == id);
            // return true if something acutaly happened
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        /// <inheritdoc cref="IDataService.UpdateReceiverModule" />
        /// <summary>
        /// UpdateReceiverModule updates the <see cref="ReceiverModule" /> with the id of the given <see cref="ReceiverModule" />.
        /// </summary>
        /// <param name="newReceiverModule">is the <see cref="ReceiverModule" /> to update</param>
        /// <returns>The updated newReceiverModule</returns>
        public ReceiverModule UpdateReceiverModule(ReceiverModule newReceiverModule)
        {
            // replace the document
            var replaceOneResult = _receiverModulesCollection
                .ReplaceOne(x => x.ID == newReceiverModule.ID, newReceiverModule);

            // check if something was replaced
            return replaceOneResult.IsAcknowledged && replaceOneResult.ModifiedCount > 0
                // if something was replaced, return the new newReceiverModule
                ? _receiverModulesCollection
                    .Find(x => x.ID == newReceiverModule.ID)
                    .ToList()
                    .FirstOrDefault()
                // else return null
                : null;
        }

        #endregion receiver modules

        #endregion METHODS
    }
}