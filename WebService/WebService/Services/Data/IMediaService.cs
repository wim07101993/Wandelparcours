using System.IO;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Models;

namespace WebService.Services.Data
{
    /// <summary>
    /// Interface that describes a class that provides basic CRUD operations for <see cref="MediaData"/> in the database.
    /// </summary>
    public interface IMediaService
    {
        Task<ObjectId> CreateAsync(Stream mediaToAdd, string title);

        Task GetOneAsync(ObjectId id, Stream outStream);
        
        Task RemoveAsync(ObjectId mediaId);

        Task<ObjectId> GetOwner(ObjectId mediaId);
    }
}