using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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
            // if the item is null, throw exception
            if (item == null)
            {
                Throw?.NullArgument(nameof(item));
                return;
            }

            try
            {
                // save the new item to the database
                await MongoCollection.InsertOneAsync(item);
            }
            catch (Exception)
            {
                Throw.Database<MediaData>(EDatabaseMethod.Create);
            }
        }
    }
}