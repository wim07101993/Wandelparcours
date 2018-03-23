using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using WebService.Models;
using WebService.Services.Exceptions;

namespace WebService.Services.Data.Mongo
{
    public class MediaService : AMongoDataService<MediaData>, IMediaService
    {
        public MediaService(IConfiguration config, IThrow iThrow) : base(iThrow)
        {
            MongoCollection =
                // create a new client
                new MongoClient(config["Database:ConnectionString"])
                    // get the database from the client
                    .GetDatabase(config["Database:DatabaseName"])
                    // get the residents mongo collection
                    .GetCollection<MediaData>(config["Database:MediaCollectionName"]);
        }

        public override IMongoCollection<MediaData> MongoCollection { get; }

        public override async Task CreateAsync(MediaData item)
        {
            if (item == null)
            {
                Throw?.NullArgument(nameof(item));
                return;
            }

            try
            {
                 await MongoCollection.InsertOneAsync(item);
            }
            catch (Exception)
            {
                Throw.Database<MediaData>(EDatabaseMethod.Create);
            }
        }

        public async Task<byte[]> GetOneAsync(ObjectId id, string extension)
        {
            var find = MongoCollection.Find(x => x.Id == id && x.Extension == extension);

           if (find.Count() <= 0)
                Throw?.NotFound<MediaData>(id);

            var selector = Builders<MediaData>.Projection.Include(x => x.Data);

            return (await find
                    .Project<MediaData>(selector)
                    .FirstOrDefaultAsync())
                .Data;
        }
    }
}