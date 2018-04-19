using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using WebService.Helpers.Exceptions;
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

        public async Task RemoveByResident(ObjectId residentId)
        {
            var deleteResult = await MongoCollection.DeleteManyAsync(
                new FilterDefinitionBuilder<MediaData>().Eq(x => x.OwnerId, residentId));

            if (!deleteResult.IsAcknowledged)
                throw new DatabaseException(EDatabaseMethod.Delete);
            if (deleteResult.DeletedCount <= 0)
                throw new NotFoundException<MediaData>();
        }
    }
}