using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Models;

namespace WebService.Services.Data
{
    public interface IMediaService : IDataService<MediaData>
    {
        Task<byte[]> GetAsync(ObjectId id, string extension);
    }
}