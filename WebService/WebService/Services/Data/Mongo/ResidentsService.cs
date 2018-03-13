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
using WebService.Models.Bases;

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
            MongoCollection =
                // create a new client
                new MongoClient(config["Database:ConnectionString"])
                    // get the database 
                    .GetDatabase(config["Database:DatabaseName"])
                    // get the residents mongo collection
                    .GetCollection<Resident>(config["Database:ResidentsCollectionName"]);
        }

        /// <inheritdoc cref="AMongoDataService{T}.MongoCollection" />
        /// <summary>
        /// MongoCollection is the mongo collection to query residents.
        /// </summary>
        public override IMongoCollection<Resident> MongoCollection { get; }


        /// <inheritdoc cref="IResidentsService.GetAsync(int,IEnumerable{Expression{Func{Resident,object}}})" />
        /// <summary>
        /// GetAsync returns the <see cref="Resident"/> with the given tag from the database. 
        /// <para/>
        /// It only fills the properties passed in the <see cref="propertiesToInclude"/> parameter. The id is always passed and 
        /// if the <see cref="propertiesToInclude"/> parameter is null (which it is by default), all the properties are included. 
        /// </summary>
        /// <param name="tag">is the tag of the <see cref="Resident"/> that needs to be fetched</param>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>The <see cref="Resident"/> with the given id</returns>
        /// <exception cref="NotFoundException">when there is no <see cref="Resident"/> that holds the given tag</exception>
        public async Task<Resident> GetAsync(int tag,
            IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null)
        {
            // get the resident with the given tag
            var findResult = MongoCollection.Find(x => x.Tags != null && x.Tags.Contains(tag));

            // if there is no resident with the given tag, throw NotFoundException
            if (findResult.Count() <= 0)
                throw new NotFoundException($"no {typeof(Resident).Name} found with tag {tag}");

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


        /// <inheritdoc cref="IResidentsService.AddMediaAsync(ObjectId,byte[],EMediaType)" />
        /// <summary>
        /// AddMediaAsync adds the <see cref="data"/> as media of the type <see cref="mediaType"/> to the <see cref="Resident"/>
        /// with as <see cref="Resident.Id"/> the passed <see cref="residentId"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="data">is the data of the media to add</param>
        /// <param name="mediaType">is the type of media to add</param>
        /// <exception cref="ArgumentNullException">when the data is null</exception>
        /// <exception cref="NotFoundException">when there is no <see cref="Resident"/> found with the given <see cref="AModelWithID.Id"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">when the media type doesn't exist</exception>
        public async Task AddMediaAsync(ObjectId residentId, byte[] data, EMediaType mediaType)
        {
            // if the data is null, throw an exception
            if (data == null)
                throw new ArgumentNullException(nameof(data), "data cannot be null");

            // add the media
            await AddMediaAsync(residentId, new MediaWithId {Id = ObjectId.GenerateNewId(), Data = data}, mediaType);
        }

        /// <inheritdoc cref="IResidentsService.AddMediaAsync(ObjectId,string,EMediaType)" />
        /// <summary>
        /// AddMediaAsync adds the <see cref="url"/> as media of the type <see cref="mediaType"/> to the <see cref="Resident"/>
        /// with as <see cref="Resident.Id"/> the passed <see cref="residentId"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/> add the media to</param>
        /// <param name="url">is the url to the media to add</param>
        /// <param name="mediaType">is the type of media to add</param>
        /// <exception cref="ArgumentNullException">when the url is null</exception>
        /// <exception cref="NotFoundException">when there is no <see cref="Resident"/> found with the given <see cref="AModelWithID.Id"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">when the media type doesn't exist</exception>
        public async Task AddMediaAsync(ObjectId residentId, string url, EMediaType mediaType)
        {
            // if the url is null, throw an exception
            if (url == null)
                throw new ArgumentNullException(nameof(url), "url cannot be null");

            // add the media
            await AddMediaAsync(residentId, new MediaWithId {Id = ObjectId.GenerateNewId(), Url = url}, mediaType);
        }

        /// <summary>
        /// AddMediaAsync adds the <see cref="media"/> of the type <see cref="mediaType"/> to the <see cref="Resident"/>
        /// with as <see cref="Resident.Id"/> the passed <see cref="residentId"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/> add the media to</param>
        /// <param name="media">is the media to add</param>
        /// <param name="mediaType">is the type of media to add</param>
        /// <exception cref="NotFoundException">when there is no <see cref="Resident"/> found with the given <see cref="AModelWithID.Id"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">when the media type doesn't exist</exception>
        private async Task AddMediaAsync(ObjectId residentId, MediaWithId media, EMediaType mediaType)
        {
            // check the media type and add the respectively media.
            switch (mediaType)
            {
                case EMediaType.Audio:
                    await AddMediaAsync(residentId, x => x.Music, media);
                    break;
                case EMediaType.Video:
                    await AddMediaAsync(residentId, x => x.Videos, media);
                    break;
                case EMediaType.Image:
                    await AddMediaAsync(residentId, x => x.Images, media);
                    break;
                case EMediaType.Color:
                    await AddMediaAsync(residentId, x => x.Colors, media);
                    break;
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
        /// <returns>the <see cref="Resident"/> tho which the media has been added in the state before the change</returns>
        /// <exception cref="NotFoundException">when there is no <see cref="Resident"/> found with the given <see cref="AModelWithID.Id"/></exception>
        private async Task<Resident> AddMediaAsync(ObjectId residentId,
            Expression<Func<Resident, IEnumerable<MediaWithId>>> selector, MediaWithId media)
        {
            // create a filter to get the right resident
            var filter = Builders<Resident>.Filter.Eq(x => x.Id, residentId);

            // create updater to add media to the resident
            var updater = Builders<Resident>.Update.Push(selector, media);

            // execute the query with the filter and updater
            var resident = await MongoCollection.FindOneAndUpdateAsync(filter, updater);

            // if there was no resident that matched, throw exception
            if (resident == null)
                throw new NotFoundException($"no {typeof(Resident).Name} found with id {residentId}");

            // return the resident
            return resident;
        }

        /// <inheritdoc cref="IResidentsService.RemoveMediaAsync" />
        /// <summary>
        /// RemoveMediaAsync removes the media of type <see cref="mediaType"/> with as id <see cref="mediaId"/> of the
        /// <see cref="Resident"/> with as id <see cref="residentId"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/> to remove the media from</param>
        /// <param name="mediaId">is the id of media to remove</param>
        /// <param name="mediaType">is the type of media to remove</param>
        /// <exception cref="NotFoundException">when there is no <see cref="Resident"/> found with the given <see cref="AModelWithID.Id"/></exception>
        /// <exception cref="NotFoundException">when there is no <see cref="MediaWithId"/> found with the given <see cref="AModelWithID.Id"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">when the media type doesn't exist</exception>
        public async Task RemoveMediaAsync(ObjectId residentId, ObjectId mediaId, EMediaType mediaType)
        {
            // declare variables
            Resident resident;
            bool containsMedia;

            // check the media type and remove the respectively media
            switch (mediaType)
            {
                case EMediaType.Audio:
                    // remove the media
                    resident = await RemoveMediaAsync(residentId, x => x.Music, mediaId);
                    // check if the original resident had the media with the given id
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

            // if the resident did not have the media, throw exception
            if (!containsMedia)
                throw new NotFoundException($"there is no media found with id {mediaId}");
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
        /// <returns>the <see cref="Resident"/> as it was before the media was removed</returns>
        private async Task<Resident> RemoveMediaAsync(ObjectId residentId,
            Expression<Func<Resident, IEnumerable<MediaWithId>>> selector, ObjectId mediaId)
        {
            // create filter to select the correct resident
            var filter = Builders<Resident>.Filter.Eq(x => x.Id, residentId);

            // create updater to remove the correct media
            var updater = Builders<Resident>.Update.PullFilter(
                selector, Builders<MediaWithId>.Filter.Eq(x => x.Id, mediaId));

            // execute the query
            var resident = await MongoCollection.FindOneAndUpdateAsync(filter, updater);

            // if there was no resident to match, throw exception
            if (resident == null)
                throw new NotFoundException($"no {typeof(Resident).Name} found with id {residentId}");

            // return the original resident
            return resident;
        }
    }
}