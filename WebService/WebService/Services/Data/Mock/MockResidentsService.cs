using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Models.Bases;

namespace WebService.Services.Data.Mock
{
#pragma warning disable 1998 // disable warning async method without await
    /// <inheritdoc cref="IDataService{T}"/>
    /// <summary>
    /// MockResidentsService is a class that implements the <see cref="IDataService{T}"/> interface.
    /// <para/>
    /// It handles the saving and retrieving data to and from a list of Residents in memory. It does not store anything in a database.
    /// </summary>
    public class MockResidentsService : AMockDataService<Resident>, IResidentsService
    {
        public override List<Resident> MockData { get; } = Mock.MockData.MockResidents;

        public override Resident CreateNewItem(ObjectId id)
            => new Resident {Id = id};

        public async Task<Resident> GetOneAsync(int tag,
            IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null)
        {
            // search for the resident index
            var residentIndex = MockData.FindIndex(x => x.Tags != null && x.Tags.Contains(tag));

            // if there is no resident with the given id, throw exception
            if (residentIndex < 0)
                throw new ElementNotFoundException<Resident>(nameof(Resident.Tags), "tag");
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

        public Task<TValue> GetPropertyAsync<TValue>(int tag, Expression<Func<Resident, TValue>> propertyToSelect)
        {
            //TODO
            throw new NotImplementedException();
        }

        public Task<int> GetHighestTagNumberAsync()
        {
            // TODO
            throw new NotImplementedException();
        }

        public async Task AddMediaAsync(ObjectId residentId, string title, byte[] data, EMediaType mediaType,
            string extension = null)
        {
            // if the data is null, throw an exception
            if (data == null)
                throw new System.ArgumentNullException(nameof(data), "data to add cannot be null");

            // add the mediaData
            AddMedia(residentId, new MediaUrl {Id = ObjectId.GenerateNewId(), Title = title, Extension = extension},
                mediaType);
        }

        public async Task AddMediaAsync(ObjectId residentId, string url, EMediaType mediaType)
        {
            // if the url is null, throw an exception
            if (url == null)
                throw new System.ArgumentNullException(nameof(url), "url to add cannot be null");

            // add the mediaData
            AddMedia(residentId, new MediaUrl {Id = ObjectId.GenerateNewId(), Url = url}, mediaType);
        }

        public Task UpdatePropertyAsync<TValue>(int tag, Expression<Func<Resident, TValue>> propertyToUpdate, TValue value)
        {
            throw new NotImplementedException();
        }

        private void AddMedia(ObjectId residentId, MediaUrl mediaData, EMediaType mediaType)
        {
            // search for the resident index
            var index = MockData.FindIndex(x => x.Id == residentId);

            // if there is no resident with the given id, throw exception
            if (index < 0)
                throw new NotFoundException<Resident>(nameof(IModelWithID.Id), residentId.ToString());

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

        public async Task RemoveMediaAsync(ObjectId residentId, ObjectId mediaId, EMediaType mediaType)
        {
            // search for the resident index
            var residentIndex = MockData.FindIndex(x => x.Id == residentId);

            // if there is no resident with the given id, throw exception
            if (residentIndex < 0)
                throw new NotFoundException<Resident>(nameof(IModelWithID.Id), residentId.ToString());

            int mediaIndex;
            // check the mediaData type and remove the respectively mediaData.
            switch (mediaType)
            {
                case EMediaType.Audio:
                    // if the music is null, there is no music with the given id => exception
                    if (MockData[residentIndex].Music == null)
                        throw new ElementNotFoundException<Resident>(nameof(Resident.Music), "track");

                    // check if there is music with the given id, if there isn't, throw exception
                    mediaIndex = MockData[residentIndex].Music.FindIndex(x => x.Id == mediaId);
                    if (mediaIndex < 0)
                        throw new ElementNotFoundException<Resident>(nameof(Resident.Music), "track");

                    // remove the mediaData
                    MockData[residentIndex].Music.RemoveAt(mediaIndex);
                    break;
                case EMediaType.Video:
                    if (MockData[residentIndex].Videos == null)
                        throw new ElementNotFoundException<Resident>(nameof(Resident.Videos), "video");

                    mediaIndex = MockData[residentIndex].Videos.FindIndex(x => x.Id == mediaId);
                    if (mediaIndex < 0)
                        throw new ElementNotFoundException<Resident>(nameof(Resident.Videos), "video");

                    MockData[residentIndex].Videos.RemoveAt(mediaIndex);
                    break;
                case EMediaType.Image:
                    if (MockData[residentIndex].Images == null)
                        throw new ElementNotFoundException<Resident>(nameof(Resident.Images), "image");

                    mediaIndex = MockData[residentIndex].Images.FindIndex(x => x.Id == mediaId);
                    if (mediaIndex < 0)
                        throw new ElementNotFoundException<Resident>(nameof(Resident.Images), "image");

                    MockData[residentIndex].Images.RemoveAt(mediaIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, null);
            }
        }

        public async Task RemoveSubItemAsync(ObjectId residentId,
            Expression<Func<Resident, IEnumerable<object>>> selector, object item)
        {
            // search for the resident index
            var residentIndex = MockData.FindIndex(x => x.Id == residentId);

            // if there is no resident with the given id, throw exception
            if (residentIndex < 0)
                throw new NotFoundException<Resident>(nameof(IModelWithID.Id), residentId.ToString());

            ((IList) selector.Compile()(MockData[residentIndex])).Remove(x => x.Equals(item));
        }
    }
}