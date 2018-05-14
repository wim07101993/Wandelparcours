using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Models.Bases;
using ArgumentNullException = System.ArgumentNullException;

namespace WebService.Services.Data.Mongo
{
    public class ResidentsService : AMongoDataService<Resident>, IResidentsService
    {
        private readonly IMediaService _mediaService;

        public ResidentsService(IConfiguration config, IMediaService mediaService)
            : base(
                config["Database:ConnectionString"],
                config["Database:DatabaseName"],
                config["Database:ResidentsCollectionName"])
        {
            _mediaService = mediaService;
        }


        #region CREATE

        public async Task AddMediaAsync(ObjectId residentId, string title, byte[] data, EMediaType mediaType,
            string extension = null)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var mediaId = ObjectId.GenerateNewId();
            await _mediaService.CreateAsync(
                new MediaData
                {
                    Id = mediaId,
                    Data = data,
                    Extension = extension,
                    OwnerId = residentId
                });

            await AddMediaAsync(
                residentId,
                new MediaUrl {Id = mediaId, Title = title, Extension = extension},
                mediaType);
        }

        public async Task AddMediaAsync(ObjectId residentId, string url, EMediaType mediaType)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            await AddMediaAsync(residentId, new MediaUrl {Id = ObjectId.GenerateNewId(), Url = url}, mediaType);
        }

        private async Task AddMediaAsync(ObjectId residentId, MediaUrl mediaUrl, EMediaType mediaType)
        {
            switch (mediaType)
            {
                case EMediaType.Audio:
                    await AddMediaAsync(residentId, x => x.Music, mediaUrl);
                    break;
                case EMediaType.Video:
                    await AddMediaAsync(residentId, x => x.Videos, mediaUrl);
                    break;
                case EMediaType.Image:
                    await AddMediaAsync(residentId, x => x.Images, mediaUrl);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, null);
            }
        }

        private async Task<Resident> AddMediaAsync(ObjectId residentId,
            Expression<Func<Resident, IEnumerable<MediaUrl>>> selector, MediaUrl mediaUrl)
        {
            var updater = Builders<Resident>.Update.Push(selector, mediaUrl);
            var resident = await MongoCollection.FindOneAndUpdateAsync(x => x.Id == residentId, updater);

            if (resident == null)
                throw new NotFoundException<Resident>(nameof(IModelWithID.Id), residentId.ToString());

            return resident;
        }

        #endregion CREATE


        #region READ

        public async Task<Resident> GetOneAsync(int tag,
            IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null)
            => await GetByAsync(x => x.Tags != null && x.Tags.Contains(tag), propertiesToInclude);

        public async Task<IEnumerable<Resident>> GetMany(IEnumerable<ObjectId> objectIds,
            IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null)
        {
            var filter = Builders<Resident>.Filter.In(x => x.Id, objectIds);

            return objectIds == null
                ? null
                : await MongoCollection
                    .Find(filter)
                    .Select(propertiesToInclude)
                    .ToListAsync();
        }

        
        public virtual async Task<TValue> GetPropertyAsync<TValue>(int tag,
            Expression<Func<Resident, TValue>> propertyToSelect)
            => await GetPropertyByAsync(x => x.Tags.Contains(tag), propertyToSelect);

        public async Task<int> GetHighestTagNumberAsync()
        {
            var result = await MongoCollection
                .AggregateAsync(
                    PipelineDefinition<Resident, BsonDocument>.Create(
                        new BsonDocument("$unwind", "$tags"),
                        new BsonDocument(
                            "$group", new BsonDocument
                            {
                                {"_id", "$_id"},
                                {"max", new BsonDocument("$max", "$tags")}
                            }),
                        new BsonDocument("$sort", new BsonDocument("max", -1)),
                        new BsonDocument("$limit", 1)));

            return result.Single()["max"].AsInt32;
        }

        #endregion READ


        #region UPDATE

        public async Task UpdatePropertyAsync<TValue>(int tag, Expression<Func<Resident, TValue>> propertyToUpdate,
            TValue value)
        {
            if (propertyToUpdate == null)
                throw new ArgumentNullException(nameof(propertyToUpdate));

            var update = Builders<Resident>.Update.Set(propertyToUpdate, value);

            var updateResult = await MongoCollection.UpdateOneAsync(
                x => x.Tags != null && x.Tags.Contains(tag),
                update);

            if (!updateResult.IsAcknowledged)
                throw new DatabaseException(EDatabaseMethod.Update);

            if (updateResult.MatchedCount <= 0)
                throw new NotFoundException<Resident>(nameof(Resident.Tags), tag.ToString());
        }

        #endregion UPDATE


        #region DELETE

        public async Task RemoveMediaAsync(ObjectId residentId, ObjectId mediaId, EMediaType mediaType)
        {
            Resident resident;
            bool containsMedia;

            switch (mediaType)
            {
                case EMediaType.Audio:
                    resident = await RemoveMediaAsync(residentId, x => x.Music, mediaId);
                    containsMedia = resident.Music.Any(x => x.Id == mediaId);
                    break;
                case EMediaType.Video:
                    resident = await RemoveMediaAsync(residentId, x => x.Videos, mediaId);
                    containsMedia = resident.Videos.Any(x => x.Id == mediaId);
                    break;
                case EMediaType.Image:
                    resident = await RemoveMediaAsync(residentId, x => x.Images, mediaId);
                    containsMedia = resident.Images.Any(x => x.Id == mediaId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, null);
            }

            if (!containsMedia)
                throw new ElementNotFoundException<Resident>(mediaType.ToString(), "media");
        }

        private async Task<Resident> RemoveMediaAsync(ObjectId residentId,
            Expression<Func<Resident, IEnumerable<MediaUrl>>> selector, ObjectId mediaId)
        {
            try
            {
                await _mediaService.RemoveAsync(mediaId);
            }
            catch (NotFoundException)
            {
                // IGNORED
            }

            var updater = Builders<Resident>.Update.PullFilter(selector, x => x.Id == mediaId);
            var resident = await MongoCollection.FindOneAndUpdateAsync(x => x.Id == residentId, updater);

            if (resident == null)
                throw new NotFoundException<Resident>(nameof(IModelWithID.Id), residentId.ToString());

            return resident;
        }

        public async Task RemoveColor(ObjectId residentId, Color color)
        {
            var updater = Builders<Resident>.Update
                .PullFilter(x => x.Colors, x => x.R == color.R && x.G == color.G && x.B == color.B);
            var resident = await MongoCollection.FindOneAndUpdateAsync(x => x.Id == residentId, updater);

            if (resident == null)
                throw new NotFoundException<Resident>(nameof(IModelWithID.Id), residentId.ToString());
        }

        public override Task RemoveAsync(ObjectId id)
        {
            _mediaService.RemoveByResident(id);
            return base.RemoveAsync(id);
        }

        #endregion DELETE
    }
}