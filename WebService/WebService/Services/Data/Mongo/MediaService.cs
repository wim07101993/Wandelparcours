using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using WebService.Helpers.Exceptions;
using WebService.Models;
using WebService.Models.Bases;
using ArgumentNullException = System.ArgumentNullException;

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
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                await MongoCollection.InsertOneAsync(item);
            }
            catch (Exception e)
            {
                throw new DatabaseException(EDatabaseMethod.Create, e);
            }
        }

        public async Task<byte[]> GetOneAsync(ObjectId id, string extension)
        {
            var find = MongoCollection.Find(x => x.Id == id && x.Extension == extension);

            if (find.Count() <= 0)
                throw new NotFoundException<MediaData>(nameof(IModelWithID.Id), id.ToString());

            var selector = Builders<MediaData>.Projection.Include(x => x.Data);

            return (await find
                    .Project<MediaData>(selector)
                    .FirstOrDefaultAsync())
                .Data;
        }
    }
}