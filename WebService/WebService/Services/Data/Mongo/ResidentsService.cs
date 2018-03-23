using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using WebService.Models;
using WebService.Services.Exceptions;

namespace WebService.Services.Data.Mongo
{
    /// <inheritdoc cref="AMongoDataService{T}"/>
    /// <summary>
    /// ResidentsService is a class that extends from the <see cref="AMongoDataService{T}"/> class
    /// and by doing that implements the <see cref="IDataService{T}"/> interface.
    /// <para/>
    /// It handles the saving and retrieving residents to and from the mongo database.
    /// <para/>
    /// The connection string, db name and collections that are used are stored in the IConfiguration dependency under the Database object.
    /// </summary>
    public class ResidentsService : AMongoDataService<Resident>, IResidentsService
    {
        private readonly IDataService<MediaData> _mediaService;

        public ResidentsService(IConfiguration config, IThrow iThrow, IMediaService mediaService)
            : base(iThrow)
        {
            _mediaService = mediaService;

            MongoCollection =
                // create a new client
                new MongoClient(config["Database:ConnectionString"])
                    // get the database 
                    .GetDatabase(config["Database:DatabaseName"])
                    // get the residents mongo collection
                    .GetCollection<Resident>(config["Database:ResidentsCollectionName"]);
        }

        public override IMongoCollection<Resident> MongoCollection { get; }


        #region CREATE

        public async Task AddMediaAsync(ObjectId residentId, string title, byte[] data, EMediaType mediaType,
            string extension = null)
        {
            // if the data is null, throw an exception
            if (data == null)
            {
                Throw.NullArgument(nameof(data));
                return;
            }

            var mediaId = ObjectId.GenerateNewId();
            // add media to the database
            await _mediaService.CreateAsync(new MediaData {Id = mediaId, Data = data, Extension = extension});

            // add the mediaData to the resident
            await AddMediaAsync(
                residentId,
                new MediaUrl {Id = mediaId, Title = title, Extension = extension},
                mediaType);
        }

        public async Task AddMediaAsync(ObjectId residentId, string url, EMediaType mediaType)
        {
            // if the url is null, throw an exception
            if (url == null)
            {
                Throw.NullArgument(nameof(url));
                return;
            }

            // add the mediaData
            await AddMediaAsync(residentId, new MediaUrl {Id = ObjectId.GenerateNewId(), Url = url}, mediaType);
        }

        private async Task AddMediaAsync(ObjectId residentId, MediaUrl mediaUrl, EMediaType mediaType)
        {
            // check the mediaData type and add the respectively mediaData.
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
            // create a filter to get the right resident
            var filter = Builders<Resident>.Filter.Eq(x => x.Id, residentId);

            // create updater to add mediaData to the resident
            var updater = Builders<Resident>.Update.Push(selector, mediaUrl);

            // execute the query with the filter and updater
            var resident = await MongoCollection.FindOneAndUpdateAsync(filter, updater);

            // if there was no resident that matched, throw exception
            if (resident == null)
            {
                Throw.NotFound<Resident>(residentId);
                return null;
            }

            // return the resident
            return resident;
        }

        #endregion CREATE


        #region READ

        public async Task<Resident> GetAsync(int tag,
            IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null)
        {
            // get the resident with the given tag
            var findResult = MongoCollection.Find(x => x.Tags != null && x.Tags.Contains(tag));

            // if there is no resident with the given tag, throw NotFoundException
            if (findResult.Count() <= 0)
            {
                Throw?.NotFound<Resident>(tag);
                return default(Resident);
            }

            // if the properties to include are null, return all the properties
            if (propertiesToInclude == null)
                return await findResult.FirstOrDefaultAsync();

            // create a property filter (always include the id)
            var selector = Builders<Resident>.Projection.Include(x => x.Id);

            // iterate over all the properties to include and add them to the filter
            selector = propertiesToInclude.Aggregate(selector, (current, property) => current.Include(property));

            // return the item
            return await findResult
                // filter the properties
                .Project<Resident>(selector)
                // execute the query
                .FirstOrDefaultAsync();
        }

        public virtual async Task<object> GetPropertyAsync(int tag, Expression<Func<Resident, object>> propertyToSelect)
        {
            // if the property to select is null, throw exception
            if (propertyToSelect == null)
            {
                Throw?.NullArgument(nameof(propertyToSelect));
                return null;
            }

            // get the item with the given id
            var find = MongoCollection.Find(x => x.Tags.Contains(tag));

            // if there is no item with the given id, throw exception
            if (find.Count() <= 0)
            {
                Throw?.NotFound<Resident>(tag);
                return null;
            }

            // create a property filter
            var selector = Builders<Resident>.Projection.Include(propertyToSelect);

            // execute the query
            var item = await find
                .Project<Resident>(selector)
                .FirstOrDefaultAsync();

            // return only the asked property
            return propertyToSelect.Compile()(item);
        }

        #endregion READ


        #region DELETE

        public async Task RemoveMediaAsync(ObjectId residentId, ObjectId mediaId, EMediaType mediaType)
        {
            // declare variables
            Resident resident;
            bool containsMedia;

            // check the mediaData type and remove the respectively mediaData
            switch (mediaType)
            {
                case EMediaType.Audio:
                    // remove the mediaData
                    resident = await RemoveMediaAsync(residentId, x => x.Music, mediaId);
                    // check if the original resident had the mediaData with the given id
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

            // if the resident did not have the mediaData, throw exception
            if (!containsMedia)
                Throw.NotFound<MediaData>(mediaId);
        }

        private async Task<Resident> RemoveMediaAsync(ObjectId residentId,
            Expression<Func<Resident, IEnumerable<MediaUrl>>> selector, ObjectId mediaId)
        {
            await _mediaService.RemoveAsync(mediaId);

            // create filter to select the correct resident
            var filter = Builders<Resident>.Filter.Eq(x => x.Id, residentId);

            // create updater to remove the correct mediaData
            var updater = Builders<Resident>.Update.PullFilter(
                selector, Builders<MediaUrl>.Filter.Eq(x => x.Id, mediaId));

            // execute the query
            var resident = await MongoCollection.FindOneAndUpdateAsync(filter, updater);

            // if there was no resident to match, throw exception
            if (resident == null)
            {
                Throw.NotFound<Resident>(residentId);
                return null;
            }

            // return the original resident
            return resident;
        }

        public Task RemoveSubItemAsync(ObjectId residentId, Expression<Func<Resident, IEnumerable>> selector,
            object item)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveSubItemAsync(ObjectId residentId,
            Expression<Func<Resident, IEnumerable<object>>> selector, object item)
        {
            // create filter to select the correct resident
            var filter = Builders<Resident>.Filter.Eq(x => x.Id, residentId);

            // create updater to remove the correct mediaData
            var updater = Builders<Resident>.Update.PullFilter(
                selector, Builders<object>.Filter.Eq(x => x, item));

            // execute the query
            var resident = await MongoCollection.FindOneAndUpdateAsync(filter, updater);

            // if there was no resident to match, throw exception
            if (resident == null)
            {
                Throw.NotFound<Resident>(residentId);
            }
        }

        #endregion DELETE
    }
}