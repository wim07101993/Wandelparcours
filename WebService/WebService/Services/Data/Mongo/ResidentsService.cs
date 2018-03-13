using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using WebService.Helpers.Exceptions;
using WebService.Models;

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
        /// <summary>
        /// ResidentsService is the constructor to create an instance of the <see cref="ResidentsService"/> class.
        /// <para/>
        /// The connection string, db name and collections that are used are stored in the IConfiguration dependency under the Database object.
        /// </summary>
        /// <param name="config"></param>
        public ResidentsService(IConfiguration config)
        {
            // create a new client and get the database from it
            var db = new MongoClient(config["Database:ConnectionString"]).GetDatabase(config["Database:DatabaseName"]);

            // get the residents mongo collection
            MongoCollection = db.GetCollection<Resident>(config["Database:ResidentsCollectionName"]);
        }

        /// <inheritdoc cref="AMongoDataService{T}.MongoCollection" />
        /// <summary>
        /// MongoCollection is the mongo collection to query residents.
        /// </summary>
        public override IMongoCollection<Resident> MongoCollection { get; }


        /// <inheritdoc cref="IResidentsService.GetAsync(int,IEnumerable{Expression{Func{Resident,object}}})" />
        /// <summary>
        /// GetAsync returns the <see cref="Resident"/> with the given id from the database. 
        /// <para/>
        /// It only fills the properties passed in the <see cref="propertiesToInclude"/> parameter. The id is always passed and 
        /// if the <see cref="propertiesToInclude"/> parameter is null (which it is by default), all the properties are included. 
        /// </summary>
        /// <param name="tag">is the tag of the <see cref="Resident"/> that needs to be fetched</param>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>An <see cref="IEnumerable{T}"/> filled with all the ts in the database.</returns>
        public async Task<Resident> GetAsync(int tag,
            IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null)
        {
            // get the resident with the given mac address
            var findResult = MongoCollection.Find(x => x.Tags != null && x.Tags.Contains(tag));

            if (findResult.Count() <= 0)
                throw new NotFoundException($"no {typeof(Resident).Name} found with tag {tag}");

            // convert the properties to include to a list (if not null)
            var properties = propertiesToInclude?.ToList();
            // if the properties are null or there are none, return all the properties
            if (properties == null)
                return await findResult.FirstOrDefaultAsync();

            // create a property filter
            var selector = Builders<Resident>.Projection.Include(x => x.Id);

            //ReSharper disable once PossibleNullReferenceException
            // iterate over all the properties and add them to the filter
            foreach (var property in properties)
                selector = selector.Include(property);

            // return the item
            return await findResult
                // filter the properties
                .Project<Resident>(selector)
                // execute the query
                .FirstOrDefaultAsync();
        }


        /// <inheritdoc cref="IResidentsService.AddMediaAsync(ObjectId,byte[],EMediaType)" />
        /// <summary>
        /// AddMediaAsync to adds the <see cref="data"/> as media of the type <see cref="mediaType"/> to the <see cref="Resident"/>
        ///  with as <see cref="Resident.Id"/> the passed <see cref="residentId"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="data">is the data of the media to add</param>
        /// <param name="mediaType">is the type of media to add</param>
        /// <returns>
        /// - true if the media was added
        /// - false if the media was not added
        /// </returns>
        public async Task<bool> AddMediaAsync(ObjectId residentId, byte[] data, EMediaType mediaType)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "data cannot be null");

            return await AddMediaAsync(
                residentId,
                new MediaWithId {Id = ObjectId.GenerateNewId(), Data = data},
                mediaType);
        }

        /// <inheritdoc cref="IResidentsService.AddMediaAsync(ObjectId,string,EMediaType)" />
        /// <summary>
        /// AddMediaAsync to adds the <see cref="url"/> as media of the type <see cref="mediaType"/> to the <see cref="Resident"/>
        /// with as <see cref="Resident.Id"/> the passed <see cref="residentId"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/> add the media to</param>
        /// <param name="url">is the url to the media to add</param>
        /// <param name="mediaType">is the type of media to add</param>
        /// <returns>
        /// - true if the media was added
        /// - false if the media was not added
        /// </returns>
        async Task<bool> IResidentsService.AddMediaAsync(ObjectId residentId, string url, EMediaType mediaType)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url), "url cannot be null");

            return await AddMediaAsync(
                residentId,
                new MediaWithId {Id = ObjectId.GenerateNewId(), Url = url},
                mediaType);
        }

        /// <summary>
        /// AddMediaAsync adds the <see cref="media"/> of the type <see cref="mediaType"/> to the <see cref="Resident"/>
        /// with as <see cref="Resident.Id"/> the passed <see cref="residentId"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/> add the media to</param>
        /// <param name="media">is the media to add</param>
        /// <param name="mediaType">is the type of media to add</param>
        /// <returns>
        /// - true if the media was added
        /// - false if the media was not added
        /// </returns>
        private async Task<bool> AddMediaAsync(ObjectId residentId, MediaWithId media, EMediaType mediaType)
        {
            switch (mediaType)
            {
                case EMediaType.Audio:
                    return await AddMediaAsync(residentId, x => x.Music, media) != null;
                case EMediaType.Video:
                    return await AddMediaAsync(residentId, x => x.Videos, media) != null;
                case EMediaType.Image:
                    return await AddMediaAsync(residentId, x => x.Images, media) != null;
                case EMediaType.Color:
                    return await AddMediaAsync(residentId, x => x.Colors, media) != null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, null);
            }
        }

        /// <summary>
        /// AddMediaAsync is supposed to add the <see cref="media"/> to the <see cref="Resident"/> with as <see cref="Resident.Id"/> 
        /// the passed <see cref="residentId"/>.
        /// <para/>
        /// The property to add the media to is selected using the <see cref="selector"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/> add the media to</param>
        /// <param name="selector">is the selector to select what property the media should be added to</param>
        /// <param name="media">is the media to add</param>
        /// <returns>
        /// - true if the media was added
        /// - false if the media was not added
        /// </returns>
        private async Task<Resident> AddMediaAsync(ObjectId residentId,
            Expression<Func<Resident, IEnumerable<MediaWithId>>> selector, MediaWithId media)
        {
            var findResult = MongoCollection.Find(x => x.Id == residentId);

            if (findResult.Count() <= 0)
                throw new NotFoundException($"no {typeof(Resident).Name} found with id {residentId}");

            var filter = Builders<Resident>.Filter.Eq(x => x.Id, residentId);

            var updater = Builders<Resident>.Update.Push(selector, media);
            var ret = await MongoCollection.FindOneAndUpdateAsync(filter, updater);
            return ret;
        }

        /// <inheritdoc cref="IResidentsService.RemoveMediaAsync" />
        /// <summary>
        /// RemoveMediaAsync to removes the media of type <see cref="mediaType"/> with as id <see cref="mediaId"/> of the
        /// <see cref="Resident"/> with as id <see cref="residentId"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/> to remove the media from</param>
        /// <param name="mediaId">is the id to the media to remove</param>
        /// <param name="mediaType">is the type of media to remove</param>
        /// <returns>
        /// - true if the media was removed
        /// - false if the media was not removed
        /// </returns>
        public async Task<bool> RemoveMediaAsync(ObjectId residentId, ObjectId mediaId, EMediaType mediaType)
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
                case EMediaType.Color:
                    resident = await RemoveMediaAsync(residentId, x => x.Colors, mediaId);
                    containsMedia = resident.Colors.Any(x => x.Id == mediaId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, null);
            }

            if (!containsMedia)
                throw new NotFoundException($"there is no media found with id {mediaId}");

            return true;
        }

        /// <inheritdoc cref="IResidentsService.RemoveMediaAsync" />
        /// <summary>
        /// RemoveMediaAsync to removes the media with as id <see cref="mediaId"/> of the <see cref="Resident"/> with as id <see cref="residentId"/>.
        /// <para/>
        /// The property to remove the media from is selected using the <see cref="selector"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/> to remove the media from</param>
        /// <param name="selector">is the selector to select what property the media should be removed from</param>
        /// <param name="mediaId">is the id to the media to remove</param>
        /// <returns>
        /// - true if the media was removed
        /// - false if the media was not removed
        /// </returns>
        private async Task<Resident> RemoveMediaAsync(ObjectId residentId,
            Expression<Func<Resident, IEnumerable<MediaWithId>>> selector, ObjectId mediaId)
        {
            var filter = Builders<Resident>.Filter.Eq(x => x.Id, residentId);

            var updater = Builders<Resident>.Update.PullFilter(
                selector, Builders<MediaWithId>.Filter.Eq(x => x.Id, mediaId));

            var resident = await MongoCollection.FindOneAndUpdateAsync(filter, updater);

            if (resident == null)
                throw new NotFoundException($"no {typeof(Resident).Name} found with id {residentId}");

            return resident;
        }
    }
}