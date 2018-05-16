using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Models;

namespace WebService.Services.Data
{
    /// <inheritdoc/>
    /// <summary>
    /// An interface that describes a class to communicate with a database for performing CRUD operations on
    /// <see cref="T:WebService.Models.ResidentLocation" />.
    /// </summary>
    public interface ILocationsService : IDataService<ResidentLocation>
    {
        /// <summary>
        /// Gets all the locations of a resident since a point in time specified with the <see cref="since"/> parameter.
        /// </summary>
        /// <param name="since">The point in time since when the locations are needed.</param>
        /// <param name="residentId">The id of the resident to get the locations from</param>
        /// <param name="propertiesToInclude">
        /// The properties to include in the query (if it is null, all properties are passed).
        /// </param>
        /// <returns>The locations of the resident since the given point in time.</returns>
        Task<IEnumerable<ResidentLocation>> GetSinceAsync(DateTime since, ObjectId residentId,
            IEnumerable<Expression<Func<ResidentLocation, object>>> propertiesToInclude = null);

        /// <summary>
        /// Gets all the locations of all residents since a point in time specified with the <see cref="since"/>
        /// paramter
        /// </summary>
        /// <param name="since">The point in time since when the locations are needed.</param>
        /// <param name="propertiesToInclude">
        /// The properties to include in the query (if it is null, all properties are passed).
        /// </param>
        /// <returns>The locations of all residents since the given point in time.</returns>
        Task<IEnumerable<ResidentLocation>> GetSinceAsync(DateTime since,
            IEnumerable<Expression<Func<ResidentLocation, object>>> propertiesToInclude = null);
    }
}