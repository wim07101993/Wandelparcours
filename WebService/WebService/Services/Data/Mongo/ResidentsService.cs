using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using WebService.Helpers.Extensions;
using WebService.Models;

namespace WebService.Services.Data.Mongo
{
    /// <inheritdoc cref="AMongoDataService{T}"/>
    /// <summary>
    /// ResidentsService is a class that extends from the <see cref="AMongoDataService{T}"/> class
    /// and by doing that implements the <see cref="IDataService{T}"/> interface.
    /// <para/>
    /// It handles the saving and retreiving residents to and from the mongo database.
    /// <para/>
    /// The connectionstring, db name and collections that are used are stored in the IConfiguration dependency under the Database object.
    /// </summary>
    public class ResidentsService : AMongoDataService<Resident>, IResidentsService
    {
        /// <summary>
        /// ResidentsService is the contsructor to create an instance of the <see cref="ResidentsService"/> class.
        /// <para/>
        /// The connectionstring, db name and collections that are used are stored in the IConfiguration dependency under the Database object.
        /// </summary>
        /// <param name="config"></param>
        public ResidentsService(IConfiguration config)
        {
            // create a new client and get the databas from it
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
            var resident = MongoCollection.Find(x => x.Tags != null && x.Tags.Contains(tag));

            // convert the properties to include to a list (if not null)
            var properties = propertiesToInclude?.ToList();
            // if the proeprties are null or there are none, return all the properties
            if (EnumerableExtensions.IsNullOrEmpty(properties))
                return await resident.FirstOrDefaultAsync();

            // create a propertyfilter
            var selector = Builders<Resident>.Projection.Include(x => x.Id);

            //ReSharper disable once PossibleNullReferenceException
            // iterate over all the properties and add them to the filter
            foreach (var property in properties)
                selector = selector.Include(property);

            // return the item
            return await resident
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
            => await AddMediaAsync(
                residentId,
                new MediaWithId {Id = ObjectId.GenerateNewId(), Data = data},
                mediaType);

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
            => await AddMediaAsync(
                residentId,
                new MediaWithId {Id = ObjectId.GenerateNewId(), Url = url},
                mediaType);

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
            Resident resident;

            switch (mediaType)
            {
                case EMediaType.Audio:
                    resident = await AddMediaAsync(residentId, x => x.Music, media);
                    return resident.Music.Any(x => x.Id == media.Id);
                case EMediaType.Video:
                    resident = await AddMediaAsync(residentId, x => x.Videos, media);
                    return resident.Videos.Any(x => x.Id == media.Id);
                case EMediaType.Image:
                    resident = await AddMediaAsync(residentId, x => x.Images, media);
                    return resident.Images.Any(x => x.Id == media.Id);
                case EMediaType.Color:
                    resident = await AddMediaAsync(residentId, x => x.Colors, media);
                    return resident.Colors.Any(x => x.Id == media.Id);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, null);
            }
        }

        /// <summary>
        /// AddMediaAsync is supposed to add the <see cref="media"/> to the <see cref="Resident"/> with as <see cref="Resident.Id"/> 
        /// the passed <see cref="residentId"/>.
        /// <para/>
        /// The propertie to add the media to is selected using the <see cref="selector"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/> add the media to</param>
        /// <param name="selector">is the selector to select what property the media should be added to</param>
        /// <param name="media">is the media to dadd</param>
        /// <returns>
        /// - true if the media was added
        /// - false if the media was not added
        /// </returns>
        private async Task<Resident> AddMediaAsync(ObjectId residentId,
            Expression<Func<Resident, IEnumerable<MediaWithId>>> selector, MediaWithId media)
        {
            var filter = Builders<Resident>.Filter.Eq(x => x.Id, residentId);

            var updater = Builders<Resident>.Update.Push(selector, media);
            return await MongoCollection.FindOneAndUpdateAsync(filter, updater);
        }

        /// <inheritdoc cref="IResidentsService.RemoveMediaAsync" />
        /// <summary>
        /// RemoveMediaAsync to removes the media of type <see cref="mediaType"/> with as id <see cref="mediaId"/> of the
        /// <see cref="Resident"/> with as id <see cref="residentId"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/> to remove the media from</param>
        /// <param name="mediaId">is the id to the media to remove</param>
        /// <param name="mediaType">is the type fo media to remove</param>
        /// <returns>
        /// - true if the media was removed
        /// - false if the media was not removed
        /// </returns>
        public async Task<bool> RemoveMediaAsync(ObjectId residentId, ObjectId mediaId, EMediaType mediaType)
        {
            Resident resident;

            switch (mediaType)
            {
                case EMediaType.Audio:
                    resident = await RemoveMediaAsync(residentId, x => x.Music, mediaId);
                    return resident.Music.Any(x => x.Id == mediaId);
                case EMediaType.Video:
                    resident = await RemoveMediaAsync(residentId, x => x.Videos, mediaId);
                    return resident.Music.Any(x => x.Id == mediaId);
                case EMediaType.Image:
                    resident = await RemoveMediaAsync(residentId, x => x.Images, mediaId);
                    return resident.Music.Any(x => x.Id == mediaId);
                case EMediaType.Color:
                    resident = await RemoveMediaAsync(residentId, x => x.Colors, mediaId);
                    return resident.Music.Any(x => x.Id == mediaId);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, null);
            }
        }

        /// <inheritdoc cref="IResidentsService.RemoveMediaAsync" />
        /// <summary>
        /// RemoveMediaAsync to removes the media with as id <see cref="mediaId"/> of the <see cref="Resident"/> with as id <see cref="residentId"/>.
        /// <para/>
        /// The propertie to remove the media from is selected using the <see cref="selector"/>.
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
            return await MongoCollection.FindOneAndUpdateAsync(filter, updater);
        }
    }
}