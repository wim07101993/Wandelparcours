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
        #region RESIDENT

        /// <summary>
        /// Get is supposed to return all the residents from the database. 
        /// <para/>
        /// It should only fill the properties passed in the <see cref="propertiesToInclude"/> parameter. The id is always passed and 
        /// if the <see cref="propertiesToInclude"/> parameter is null (which it is by default), all the properties are included. 
        /// </summary>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>An <see cref="IEnumerable{T}"/> filled with all the residents in the database.</returns>
        IEnumerable<Resident> GetResidents(IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null);

        /// <summary>
        /// Create is supposed to save the passed <see cref="Resident"/> to the database.
        /// <para/>
        /// If the newResident is created, the method should return the id of the new <see cref="Resident"/>, else null.
        /// </summary>
        /// <param name="resident">is the <see cref="Resident"/> to save in the database</param>
        /// <returns>
        /// - the new id if the <see cref="Resident"/> was created in the database
        /// - null if the newResident was not created
        /// </returns>
        string CreateResident(Resident resident);

        /// <summary>
        /// RemoveResident is supposed to remove the <see cref="Resident"/> with the given id from the database.
        /// </summary>
        /// <param name="id">is the id of the <see cref="Resident"/> to remove in the database</param>
        /// <returns>
        /// - true if the <see cref="Resident"/> was removed from the database
        /// - false if the newResident was not removed
        /// </returns>
        bool RemoveResident(ObjectId id);

        /// <summary>
        /// UpdateResident is supposed to update the <see cref="Resident"/> with the id of the given <see cref="Resident"/>.
        /// <para/>
        /// The updated properties are defined in the <see cref="propertiesToUpdate"/> parameter.
        /// If the <see cref="propertiesToUpdate"/> parameter is null or empty (which it is by default), all properties are updated.
        /// </summary>
        /// <param name="newResident">is the <see cref="Resident"/> to update</param>
        /// <param name="propertiesToUpdate">are the properties that need to be updated</param>
        /// <returns>The updated newResident</returns>
        Resident UpdateResident(Resident newResident,
            IEnumerable<Expression<Func<Resident, object>>> propertiesToUpdate = null);

        #endregion RESIDENT

        #region RECEIVER MODULE

        /// <summary>
        /// GetReceiverModules is supposed to return all the receivers from the database. 
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> filled with all the receivers in the database.</returns>
        IEnumerable<ReceiverModule> GetReceiverModules();

        /// <summary>
        /// CreateReceiverModule is supposed to save the passed <see cref="ReceiverModule"/> to the database.
        /// <para/>
        /// If the newReceiverModule is created, the method should return the id of the new <see cref="ReceiverModule"/>, else null.
        /// </summary>
        /// <param name="receiver">is the <see cref="ReceiverModule"/> to save in the database</param>
        /// <returns>
        /// - the new id if the <see cref="ReceiverModule"/> was created in the database
        /// - null if the newReceiverModule was not created
        /// </returns>
        string CreateReceiverModule(ReceiverModule receiver);

        /// <summary>
        /// RemoveReceiverModule is supposed to remove the <see cref="ReceiverModule"/> with the given id from the database.
        /// </summary>
        /// <param name="id">is the id of the <see cref="ReceiverModule"/> to remove in the database</param>
        /// <returns>
        /// - true if the <see cref="ReceiverModule"/> was removed from the database
        /// - false if the newReceiverModule was not removed
        /// </returns>
        bool RemoveReceiverModule(ObjectId id);

        /// <summary>
        /// UpdateReceiverModule is supposed to update the <see cref="ReceiverModule"/> with the id of the given <see cref="ReceiverModule"/>.
        /// </summary>
        /// <param name="newReceiverModule">is the <see cref="ReceiverModule"/> to update</param>
        /// <returns>The updated newReceiverModule</returns>
        ReceiverModule UpdateReceiverModule(ReceiverModule newReceiverModule);

        #endregion RECEIVER MODULE
    }
}