using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using WebService.Models;

namespace WebService.Services.Data.Mongo
{
    public class MediaService : AMongoDataService<MediaData>, IMediaService
    {
        public MediaService(IConfiguration config)
            : base(config["Database:ConnectionString"],
                config["Database:DatabaseName"],
                config["Database:MediaCollectionName"])
        {
        }


        public override async Task CreateAsync(MediaData item)
            => await CreateAsync(item, false);

        public async Task<byte[]> GetOneAsync(ObjectId id, string extension)
            => (await GetByAsync(x => x.Id == id && x.Extension == extension)).Data;
    }
}