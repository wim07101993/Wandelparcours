using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using WebService.Helpers.Extensions;
using WebService.Models;

namespace WebService.Services.Data.Mongo
{
    public class MediaService : IMediaService
    {
        private readonly GridFSBucket _mediaBucket;
        private readonly IMongoCollection<Resident> _residentsCollection;


        public MediaService(IConfiguration config)
        {
            var database = new MongoClient(config["Database:ConnectionString"])
                .GetDatabase(config["Database:DatabaseName"]);

            _mediaBucket = new GridFSBucket(
                database, new GridFSBucketOptions
                {
                    BucketName = config["Database:MediaBucket"],
                    ChunkSizeBytes = 1048576,
                });

            _residentsCollection = database.GetCollection<Resident>(config["Database:ResidentsCollectionName"]);
        }


        public async Task<ObjectId> CreateAsync(Stream mediaToAdd, string title)
            => await _mediaBucket .UploadFromStreamAsync(title, mediaToAdd);

        public async Task GetOneAsync(ObjectId id, Stream outStream)
            => await _mediaBucket.DownloadToStreamAsync(id, outStream);

        public async Task RemoveAsync(ObjectId objectId)
            => await _mediaBucket.DeleteAsync(objectId);

        public async Task<ObjectId> GetOwner(ObjectId mediaId)
        {
            var residents = await _residentsCollection
                .Find(FilterDefinition<Resident>.Empty)
                .Select(new Expression<Func<Resident, object>>[] {x => x.Images, x => x.Videos, x => x.Music})
                .ToListAsync();
           
            foreach (var resident in residents)
                if (resident .Images.Any(x => x.Id == mediaId))
                    return resident.Id;
            foreach (var resident in residents)
                if (resident .Videos.Any(x => x.Id == mediaId))
                    return resident.Id;
            foreach (var resident in residents)
                if (resident .Music.Any(x => x.Id == mediaId))
                    return resident.Id;

            return default(ObjectId);
        }
    }
}