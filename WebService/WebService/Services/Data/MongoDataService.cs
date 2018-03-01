using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            // if the proeprteis are null or there are none, return all the properties
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
        /// If the resident is created, the method returns the id of the new <see cref="Resident"/>, else null.
        /// </summary>
        /// <param name="resident">is the <see cref="Resident"/> to save in the database</param>
        /// <returns>
        /// - the new id if the <see cref="Resident"/> was created in the database
        /// - null if the resident was not created
        /// </returns>
        public string CreateResident(Resident resident)
        {
            // create a new ide for the resident
            resident.ID = ObjectId.GenerateNewId();
            // save the new resident to the database
            _collection.InsertOne(resident);

            // check if the resident was created
            return _collection
                       .Find(x => x.ID == resident.ID)
                       .FirstOrDefault() != null
                // if it is, return the id
                ? resident.ID.ToString()
                // else return null
                : null;
        }
    }
}