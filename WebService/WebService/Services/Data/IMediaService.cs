using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Models;

namespace WebService.Services.Data
{
    /// <inheritdoc />
    /// <summary>
    /// Interface that describes a class that provides basic CRUD operations for <see cref="MediaData"/> in the database.
    /// </summary>
    public interface IMediaService : IDataService<MediaData>
    {
        /// <summary>
        /// Gets the data of an item in the database with a given id and extension.
        /// </summary>
        /// <param name="id">Id of the media to get</param>
        /// <param name="extension">Extensions of the media to get</param>
        /// <returns>The data of the media</returns>
        Task<byte[]> GetOneAsync(ObjectId id, string extension);
        
        /// <summary>
        /// Removes all media of a resident from the database. 
        /// </summary>
        /// <param name="residentId">Id of the resident to remove all data from</param>
        Task RemoveByResident(ObjectId residentId);
    }
}