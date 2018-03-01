using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Bson;
using WebService.Models;

namespace WebService.Services.Data
{
    /// <summary>
    /// IDataService that defines the methods to communicate with a database.
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// GetResidents is supposed to return all the residents from the database. 
        /// <para/>
        /// It should only fill the properties passed in the <see cref="propertiesToInclude"/> parameter. The id is always passed and 
        /// if the <see cref="propertiesToInclude"/> parameter is null (which it is by default), all the properties are included. 
        /// </summary>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>An <see cref="IEnumerable{T}"/> filled with all the residents in the database.</returns>
        IEnumerable<Resident> GetResidents(IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null);

        /// <summary>
        /// CreateResident is supposed to save the passed <see cref="Resident"/> to the database.
        /// <para/>
        /// If the resident is created, the method should return the id of the new <see cref="Resident"/>, else null.
        /// </summary>
        /// <param name="resident">is the <see cref="Resident"/> to save in the database</param>
        /// <returns>
        /// - the new id if the <see cref="Resident"/> was created in the database
        /// - null if the resident was not created
        /// </returns>
        string CreateResident(Resident resident);

        /// <summary>
        /// RemoveResident is supposed to remove the <see cref="Resident"/> with the given id from the database.
        /// </summary>
        /// <param name="id">is the id of the <see cref="Resident"/> to remove in the database</param>
        /// <returns>
        /// - true if the <see cref="Resident"/> was removed from the database
        /// - false if the resident was not removed
        /// </returns>
        bool RemoveResident(ObjectId id);
    }
}