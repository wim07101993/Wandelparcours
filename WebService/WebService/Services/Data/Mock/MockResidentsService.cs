using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models;

namespace WebService.Services.Data.Mock
{
#pragma warning disable 1998 // disable warning async method without await
    /// <inheritdoc cref="IDataService{T}"/>
    /// <summary>
    /// MockResidentsService is a class that implements the <see cref="IDataService{T}"/> interface.
    /// <para/>
    /// It handles the saving and retreiving data to and from a list of Residents in memory. It does not store anything in a database.
    /// <para/>
    /// The connectionstring, db name and collections that are used are stored in the IConfiguration dependency under the Database object.
    /// </summary>
    public partial class MockResidentsService : AMockDataService<Resident>, IResidentsService
    {
        /// <inheritdoc cref="AMockDataService{T}" />
        /// <summary>
        /// CreateNewItems should return a new item of the given type <see cref="Resident" /> with as Id, <see cref="id" />.
        /// </summary>
        /// <param name="id">is the id for the new object</param>
        /// <returns>A new object of type <see cref="Resident" /></returns>
        public override Resident CreateNewItem(ObjectId id)
            => new Resident {Id = id};

        /// <inheritdoc cref="IResidentsService.GetAsync(int,IEnumerable{Expression{Func{Resident,object}}})" />
        /// <summary>
        /// GetAsync returns the <see cref="Resident" /> with the given id from the database. 
        /// <para />
        /// It should only fill the properties passed in the <see cref="!:propertiesToInclude" /> parameter. The id is always passed and 
        /// if the <see cref="!:propertiesToInclude" /> parameter is null (which it is by default), all the properties are included. 
        /// </summary>
        /// <param name="tag">is the id of the item that needs to be fetched</param>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> filled with all the ts in the database.</returns>
        public async Task<Resident> GetAsync(int tag,
            IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null)
        {
            var propertiesToIncludeList = propertiesToInclude?.ToList();
            if (EnumerableExtensions.IsNullOrEmpty(propertiesToIncludeList))
                return MockData.FirstOrDefault(x => x.Tags != null && x.Tags.Contains(tag));

            foreach (var mockItem in MockData)
            {
                if (mockItem.Tags == null || !mockItem.Tags.Contains(tag))
                    continue;

                // create new newItem to return with the ide filled in
                var itemToReturn = CreateNewItem(mockItem.Id);

                // ReSharper disable once PossibleNullReferenceException
                // go over each property selector that should be included
                foreach (var selector in propertiesToIncludeList)
                {
                    // get property
                    var prop = selector.Body is MemberExpression expression
                        // via member expression
                        ? expression.Member as PropertyInfo
                        // via unary expression
                        : ((MemberExpression) ((UnaryExpression) selector.Body).Operand).Member as PropertyInfo;

                    // set the value of the property with the value of the mockItem
                    prop?.SetValue(itemToReturn, prop.GetValue(mockItem));
                }

                // return the newItem
                return itemToReturn;
            }

            // if no item is found, return the default value
            return default(Resident);
        }

        /// <inheritdoc cref="IResidentsService.AddMediaAsync(ObjectId,byte[],EMediaType)"/>
        /// <summary>
        /// AddMediaAsync adds the <see cref="data"/> as media of the type <see cref="mediaType"/> to the <see cref="Resident"/>
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
                throw new ArgumentNullException(nameof(data), "data to add cannot be null");

            var index = MockData.FindIndex(x => x.Id == residentId);

            if (index < 0)
                throw new NotFoundException($"{typeof(Resident).Name} with id {residentId} was not found");

            return AddMedia(index, new MediaWithId {Id = ObjectId.GenerateNewId(), Data = data}, mediaType);
        }

        /// <inheritdoc cref="IResidentsService.AddMediaAsync(ObjectId,string,EMediaType)"/>
        /// <summary>
        /// AddMediaAsync adds the <see cref="url"/> as media of the type <see cref="mediaType"/> to the <see cref="Resident"/>
        /// with as <see cref="Resident.Id"/> the passed <see cref="residentId"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/> add the media to</param>
        /// <param name="url">is the url to the media to add</param>
        /// <param name="mediaType">is the type of media to add</param>
        /// <returns>
        /// - true if the media was added
        /// - false if the media was not added
        /// </returns>
        public async Task<bool> AddMediaAsync(ObjectId residentId, string url, EMediaType mediaType)
        {
            var index = MockData.FindIndex(x => x.Id == residentId);

            return index >= 0 && AddMedia(index, new MediaWithId {Id = ObjectId.GenerateNewId(), Url = url}, mediaType);
        }

        private bool AddMedia(int index, MediaWithId media, EMediaType mediaType)
        {
            switch (mediaType)
            {
                case EMediaType.Audio:
                    if (MockData[index].Music == null)
                        MockData[index].Music = new List<MediaWithId>();
                    MockData[index].Music.Add(media);
                    break;
                case EMediaType.Video:
                    if (MockData[index].Videos == null)
                        MockData[index].Videos = new List<MediaWithId>();
                    MockData[index].Videos.Add(media);
                    break;
                case EMediaType.Image:
                    if (MockData[index].Images == null)
                        MockData[index].Images = new List<MediaWithId>();
                    MockData[index].Images.Add(media);
                    break;
                case EMediaType.Color:
                    if (MockData[index].Colors == null)
                        MockData[index].Colors = new List<MediaWithId>();
                    MockData[index].Colors.Add(media);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, null);
            }

            return true;
        }

        /// <inheritdoc cref="IResidentsService.RemoveMediaAsync"/>
        /// <summary>
        /// RemoveMediaAsync removes the media of type <see cref="mediaType"/> with as id <see cref="mediaId"/> of the
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
            var residentIndex = MockData.FindIndex(x => x.Id == residentId);

            if (residentIndex < 0)
                return false;

            var mediaIndex = -1;
            switch (mediaType)
            {
                case EMediaType.Audio:
                    if (MockData[residentIndex].Music == null)
                        return false;

                    mediaIndex = MockData[residentIndex].Music.FindIndex(x => x.Id == mediaId);
                    if (mediaIndex < 0)
                        return false;

                    MockData[residentIndex].Music.RemoveAt(mediaIndex);
                    return true;
                case EMediaType.Video:
                    if (MockData[residentIndex].Videos == null)
                        return false;

                    mediaIndex = MockData[residentIndex].Music.FindIndex(x => x.Id == mediaId);
                    if (mediaIndex < 0)
                        return false;

                    MockData[residentIndex].Videos.RemoveAt(mediaIndex);
                    break;
                case EMediaType.Image:
                    if (MockData[residentIndex].Images == null)
                        return false;

                    mediaIndex = MockData[residentIndex].Music.FindIndex(x => x.Id == mediaId);
                    if (mediaIndex < 0)
                        return false;

                    MockData[residentIndex].Images.RemoveAt(mediaIndex);
                    break;
                case EMediaType.Color:
                    if (MockData[residentIndex].Colors == null)
                        return false;

                    mediaIndex = MockData[residentIndex].Music.FindIndex(x => x.Id == mediaId);
                    if (mediaIndex < 0)
                        return false;

                    MockData[residentIndex].Colors.RemoveAt(mediaIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, null);
            }

            return true;
        }
    }
#pragma warning restore 1998
}