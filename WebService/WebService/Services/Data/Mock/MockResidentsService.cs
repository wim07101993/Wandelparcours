using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Helpers.Exceptions;
using WebService.Models;
using WebService.Models.Bases;
using WebService.Services.Exceptions;

namespace WebService.Services.Data.Mock
{
#pragma warning disable 1998 // disable warning async method without await
    /// <inheritdoc cref="IDataService{T}"/>
    /// <summary>
    /// MockResidentsService is a class that implements the <see cref="IDataService{T}"/> interface.
    /// <para/>
    /// It handles the saving and retrieving data to and from a list of Residents in memory. It does not store anything in a database.
    /// </summary>
    public partial class MockResidentsService : AMockDataService<Resident>, IResidentsService
    {
        public MockResidentsService(IThrow iThrow) : base(iThrow)
        {
        }

        /// <inheritdoc cref="AMockDataService{T}" />
        /// <summary>
        /// CreateNewItems returns a new item of the type <see cref="Resident" /> with as Id, <see cref="id" />.
        /// </summary>
        /// <param name="id">is the id for the new object</param>
        /// <returns>A new object of type <see cref="Resident" /></returns>
        public override Resident CreateNewItem(ObjectId id)
            => new Resident {Id = id};

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
            // search for the resident index
            var residentIndex = MockData.FindIndex(x => x.Tags != null && x.Tags.Contains(tag));

            // if there is no resident with the given id, throw exception
            if (residentIndex < 0)
                throw new NotFoundException($"{typeof(Resident).Name} with tag {tag} was not found");

            // get the resident
            var resident = MockData[residentIndex];

            // if the properties to include are null, return all properties
            if (propertiesToInclude == null)
                return resident;

            // create new item to return with the id filled in
            var itemToReturn = CreateNewItem(resident.Id);

            // ReSharper disable once PossibleNullReferenceException
            // go over each property selector that should be included
            foreach (var selector in propertiesToInclude)
            {
                // get property
                var prop = selector.Body is MemberExpression expression
                    // via member expression
                    ? expression.Member as PropertyInfo
                    // via unary expression
                    : ((MemberExpression) ((UnaryExpression) selector.Body).Operand).Member as PropertyInfo;

                // set the value of the property with the value of the mockItem
                prop?.SetValue(itemToReturn, prop.GetValue(resident));
            }

            // return the item
            return itemToReturn;
        }

        /// <inheritdoc cref="IResidentsService.AddMediaAsync(ObjectId,byte[],EMediaType)" />
        /// <summary>
        /// AddMediaAsync adds the <see cref="data"/> as mediaData of the type <see cref="mediaType"/> to the <see cref="Resident"/>
        /// with as <see cref="Resident.Id"/> the passed <see cref="residentId"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="data">is the data of the mediaData to add</param>
        /// <param name="mediaType">is the type of mediaData to add</param>
        /// <exception cref="ArgumentNullException">when the data is null</exception>
        /// <exception cref="NotFoundException">when there is no <see cref="Resident"/> found with the given <see cref="AModelWithID.Id"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">when the mediaData type doesn't exist</exception>
        public async Task AddMediaAsync(ObjectId residentId, byte[] data, EMediaType mediaType)
        {
            // if the data is null, throw an exception
            if (data == null)
                throw new ArgumentNullException(nameof(data), "data to add cannot be null");

            // add the mediaData
            AddMedia(residentId, new MediaUrl {Id = ObjectId.GenerateNewId()}, mediaType);
        }

        /// <inheritdoc cref="IResidentsService.AddMediaAsync(ObjectId,string,EMediaType)" />
        /// <summary>
        /// AddMediaAsync adds the <see cref="url"/> as mediaData of the type <see cref="mediaType"/> to the <see cref="Resident"/>
        /// with as <see cref="Resident.Id"/> the passed <see cref="residentId"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/> add the mediaData to</param>
        /// <param name="url">is the url to the mediaData to add</param>
        /// <param name="mediaType">is the type of mediaData to add</param>
        /// <exception cref="ArgumentNullException">when the url is null</exception>
        /// <exception cref="NotFoundException">when there is no <see cref="Resident"/> found with the given <see cref="AModelWithID.Id"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">when the mediaData type doesn't exist</exception>
        public async Task AddMediaAsync(ObjectId residentId, string url, EMediaType mediaType)
        {
            // if the url is null, throw an exception
            if (url == null)
                throw new ArgumentNullException(nameof(url), "url to add cannot be null");

            // add the mediaData
            AddMedia(residentId, new MediaUrl {Id = ObjectId.GenerateNewId(), Url = url}, mediaType);
        }

        /// <summary>
        /// AddMediaAsync adds the <see cref="media"/> of the type <see cref="mediaType"/> to the <see cref="Resident"/>
        /// with as <see cref="Resident.Id"/> the passed <see cref="residentId"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/> add the mediaData to</param>
        /// <param name="mediaData the mediaData to add</param>
        /// <param name="mediaType">is the type of mediaData to add</param>
        /// <exception cref="NotFoundException">when there is no <see cref="Resident"/> found with the given <see cref="AModelWithID.Id"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">when the mediaData type doesn't exist</exception>
        private void AddMedia(ObjectId residentId, MediaUrl mediaData, EMediaType mediaType)
        {
            // search for the resident index
            var index = MockData.FindIndex(x => x.Id == residentId);

            // if there is no resident with the given id, throw exception
            if (index < 0)
                throw new NotFoundException($"{typeof(Resident).Name} with id {residentId} was not found");

            // check the mediaData type and add the respectively mediaData.
            switch (mediaType)
            {
                case EMediaType.Audio:
                    MockData[index].Music.Add(mediaData);
                    break;
                case EMediaType.Video:
                    MockData[index].Videos.Add(mediaData);
                    break;
                case EMediaType.Image:
                    MockData[index].Images.Add(mediaData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, null);
            }
        }

        /// <inheritdoc cref="IResidentsService.RemoveMediaAsync" />
        /// <summary>
        /// RemoveMediaAsync removes the mediaData of type <see cref="mediaType"/> with as id <see cref="mediaId"/> of the
        /// <see cref="Resident"/> with as id <see cref="residentId"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/> to remove the mediaData from</param>
        /// <param name="mediaId">is the id of mediaData to remove</param>
        /// <param name="mediaType">is the type of mediaData to remove</param>
        /// <exception cref="NotFoundException">when there is no <see cref="Resident"/> found with the given <see cref="AModelWithID.Id"/></exception>
        /// <exception cref="NotFoundException">when there is no <see cref="MediaData"/> found with the given <see cref="AModelWithID.Id"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">when the mediaData type doesn't exist</exception>
        public async Task RemoveMediaAsync(ObjectId residentId, ObjectId mediaId, EMediaType mediaType)
        {
            // search for the resident index
            var residentIndex = MockData.FindIndex(x => x.Id == residentId);

            // if there is no resident with the given id, throw exception
            if (residentIndex < 0)
                throw new NotFoundException($"{typeof(Resident).Name} with id {residentId} was not found");

            int mediaIndex;
            // check the mediaData type and remove the respectively mediaData.
            switch (mediaType)
            {
                case EMediaType.Audio:
                    // if the music is null, there is no music with the given id => exception
                    if (MockData[residentIndex].Music == null)
                        throw new NotFoundException(
                            $"the {typeof(Resident).Name} with id {residentId} has no {mediaType.ToString()}");

                    // check if there is music with the given id, if there isn't, throw exception
                    mediaIndex = MockData[residentIndex].Music.FindIndex(x => x.Id == mediaId);
                    if (mediaIndex < 0)
                        throw new NotFoundException($"{mediaType.ToString()} with id {mediaId} was not found");

                    // remove the mediaData
                    MockData[residentIndex].Music.RemoveAt(mediaIndex);
                    break;
                case EMediaType.Video:
                    if (MockData[residentIndex].Videos == null)
                        throw new NotFoundException(
                            $"the {typeof(Resident).Name} with id {residentId} has no {mediaType.ToString()}");

                    mediaIndex = MockData[residentIndex].Videos.FindIndex(x => x.Id == mediaId);
                    if (mediaIndex < 0)
                        throw new NotFoundException($"{mediaType.ToString()} with id {mediaId} was not found");

                    MockData[residentIndex].Videos.RemoveAt(mediaIndex);
                    break;
                case EMediaType.Image:
                    if (MockData[residentIndex].Images == null)
                        throw new NotFoundException(
                            $"the {typeof(Resident).Name} with id {residentId} has no {mediaType.ToString()}");

                    mediaIndex = MockData[residentIndex].Images.FindIndex(x => x.Id == mediaId);
                    if (mediaIndex < 0)
                        throw new NotFoundException($"{mediaType.ToString()} with id {mediaId} was not found");

                    MockData[residentIndex].Images.RemoveAt(mediaIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, null);
            }
        }
    }
#pragma warning restore 1998
}