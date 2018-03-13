using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebService.Controllers.Bases;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Services.Logging;
using WebService.Models;
using WebService.Services.Data;

namespace WebService.Controllers
{
    /// <inheritdoc cref="ARestControllerBase{T}"/>
    /// <summary>
    /// ResidentsController handles the reading and writing of residents data to the database.
    /// </summary>
    [Route("api/v1/[controller]")]
    [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
    public class ResidentsController : ARestControllerBase<Resident>, IResidentsController
    {
        #region CONSTRUCTOR

        /// <inheritdoc cref="ARestControllerBase{T}" />
        /// <summary>
        /// ResidentsController creates an instance of the <see cref="T:WebService.Controllers.ResidentsController" /> class. 
        /// </summary>
        /// <param name="dataService">is a service to handle the database connection</param>
        /// <param name="logger">is a service to handle the logging of messages</param>
        public ResidentsController(IDataService<Resident> dataService, ILogger logger)
            : base(dataService, logger)
        {
        }

        #endregion CONSTRUCTOR


        #region PROPERTIES

        /// <inheritdoc cref="ARestControllerBase{T}.PropertiesToSendOnGetAll" />
        /// <summary>
        /// SmallDataProperties is a collection of expressions to select the properties that
        /// consume the least space (FirstName, LastName, Room Birthday and Doctor).
        /// </summary>
        public override IEnumerable<Expression<Func<Resident, object>>> PropertiesToSendOnGetAll { get; }
            = new Expression<Func<Resident, object>>[]
            {
                // specify the fields that need to be returned
                x => x.FirstName,
                x => x.LastName,
                x => x.Room,
                x => x.Birthday,
                x => x.Doctor,
            };

        #endregion PROPERTIES


        #region METHODS

        /// <inheritdoc cref="ARestControllerBase{T}.ConvertStringToSelector" />
        /// <summary>
        /// ConvertStringToSelector converts a property name to it's selector in the form of a <see cref="Func{TInput,TResult}"/>
        /// </summary>
        /// <param name="propertyName">is the property name to convert to a selector</param>
        /// <returns>An <see cref="Func{TInput,TResult}"/> that contains the converted selector</returns>
        /// <exception cref="WebArgumentException">When the property could not be converted to a selector</exception>
        public override Expression<Func<Resident, object>> ConvertStringToSelector(string propertyName)
        {
            // if the name of a properties matches a property of a Value, 
            // add the corresponding selector
            if (propertyName.EqualsWithCamelCasing(nameof(Resident.Birthday)))
                return x => x.Birthday;
            if (propertyName.EqualsWithCamelCasing(nameof(Resident.Colors)))
                return x => x.Colors;
            if (propertyName.EqualsWithCamelCasing(nameof(Resident.Doctor)))
                return x => x.Doctor;
            if (propertyName.EqualsWithCamelCasing(nameof(Resident.FirstName)))
                return x => x.FirstName;
            if (propertyName.EqualsWithCamelCasing(nameof(Resident.Images)))
                return x => x.Images;
            if (propertyName.EqualsWithCamelCasing(nameof(Resident.LastName)))
                return x => x.LastName;
            if (propertyName.EqualsWithCamelCasing(nameof(Resident.LastRecordedPosition)))
                return x => x.LastRecordedPosition;
            if (propertyName.EqualsWithCamelCasing(nameof(Resident.Locations)))
                return x => x.Locations;
            if (propertyName.EqualsWithCamelCasing(nameof(Resident.Music)))
                return x => x.Music;
            if (propertyName.EqualsWithCamelCasing(nameof(Resident.Picture)))
                return x => x.Picture;
            if (propertyName.EqualsWithCamelCasing(nameof(Resident.Room)))
                return x => x.Room;
            if (propertyName.EqualsWithCamelCasing(nameof(Resident.Tags)))
                return x => x.Tags;
            if (propertyName.EqualsWithCamelCasing(nameof(Resident.Videos)))
                return x => x.Videos;
            if (propertyName.EqualsWithCamelCasing(nameof(Resident.Id)))
                return x => x.Id;

            throw new WebArgumentException(
                $"Property {propertyName} cannot be found on {typeof(Resident).Name}", nameof(propertyName));
        }

        #region post (create)

        /// <inheritdoc cref="ARestControllerBase{T}.CreateAsync" />
        /// <summary>
        /// Create is supposed to save the passed <see cref="Resident"/> to the database.
        /// </summary>
        /// <param name="item">is the <see cref="Resident"/> to save in the database</param>
        /// <exception cref="Exception">When the item could not be created</exception>
        [HttpPost]
        public override async Task CreateAsync([FromBody] Resident item)
            => await base.CreateAsync(item);

        /// <inheritdoc cref="IResidentsController.AddMusicAsync(string,byte[])"/>
        /// <summary>
        /// AddMusicAsymc adds music to the music list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="musicData">is the music to add to the <see cref="Resident"/>'s music list</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        [HttpPost("{residentId}/Music/data")]
        public async Task AddMusicAsync(string residentId, [FromBody] byte[] musicData)
            => await AddMediaAsync(residentId, musicData, EMediaType.Audio);

        /// <inheritdoc cref="IResidentsController.AddMusicAsync(string,string)"/>
        /// <summary>
        /// AddMusicAsymc adds music to the music list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="url">is the url to the music to add to the <see cref="Resident"/>'s music list</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        [HttpPost("{residentId}/Music/url")]
        public async Task AddMusicAsync(string residentId, [FromBody] string url)
            => await AddMediaAsync(residentId, url, EMediaType.Audio);

        /// <inheritdoc cref="IResidentsController.AddVideoAsync(string,byte[])"/>
        /// <summary>
        /// AddVideoAsymc adds video to the video list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="videoData">is the video to add to the <see cref="Resident"/>'s video list</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        [HttpPost("{residentId}/Videos/data")]
        public async Task AddVideoAsync(string residentId, [FromBody] byte[] videoData)
            => await AddMediaAsync(residentId, videoData, EMediaType.Video);

        /// <inheritdoc cref="IResidentsController.AddVideoAsync(string,string)"/>
        /// <summary>
        /// AddVideoAsymc adds video to the video list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="url">is the url to the video to add to the <see cref="Resident"/>'s video list</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        [HttpPost("{residentId}/Videos/url")]
        public async Task AddVideoAsync(string residentId, [FromBody] string url)
            => await AddMediaAsync(residentId, url, EMediaType.Video);

        /// <inheritdoc cref="IResidentsController.AddImageAsync(string,byte[])"/>
        /// <summary>
        /// AddImageAsymc adds image to the image list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="imageData">is the image to add to the <see cref="Resident"/>'s image list</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        [HttpPost("{residentId}/Images/data")]
        public async Task AddImageAsync(string residentId, [FromBody] byte[] imageData)
            => await AddMediaAsync(residentId, imageData, EMediaType.Image);

        /// <inheritdoc cref="IResidentsController.AddImageAsync(string,string)"/>
        /// <summary>
        /// AddImageAsymc adds image to the image list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="url">is the url to the image to add to the <see cref="Resident"/>'s image list</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        [HttpPost("{residentId}/Images/url")]
        public async Task AddImageAsync(string residentId, [FromBody] string url)
            => await AddMediaAsync(residentId, url, EMediaType.Image);

        /// <inheritdoc cref="IResidentsController.AddColorAsync(string,byte[])"/>
        /// <summary>
        /// AddColorAsymc adds color to the color list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="colorData">is the color to add to the <see cref="Resident"/>'s color list</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        [HttpPost("{residentId}/Colors/data")]
        public async Task AddColorAsync(string residentId, [FromBody] byte[] colorData)
            => await AddMediaAsync(residentId, colorData, EMediaType.Color);

        /// <inheritdoc cref="IResidentsController.AddColorAsync(string,string)"/>
        /// <summary>
        /// AddColorAsymc adds color to the color list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="url">is the url to the color to add to the <see cref="Resident"/>'s color list</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        [HttpPost("{residentId}/Colors/url")]
        public async Task AddColorAsync(string residentId, [FromBody] string url)
            => await AddMediaAsync(residentId, url, EMediaType.Color);

        /// <summary>
        /// AddColorAsymc adds media to the its respectively list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="data">is the data to add to the <see cref="Resident"/>'s list</param>
        /// <param name="mediaType"></param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        private async Task AddMediaAsync(string residentId, byte[] data, EMediaType mediaType)
        {
            // parse the id
            if (!ObjectId.TryParse(residentId, out var residentObjectId))
                // if it fails, throw not found exception
                throw new NotFoundException($"The {typeof(Resident).Name} with id {residentId} could not be found");

            // use the data service to create a new updater
            var updated = await ((IResidentsService) DataService).AddMediaAsync(residentObjectId, data, mediaType);

            if (!updated)
                throw new Exception(
                    $"Could not add data media of type {mediaType.ToString()} to resident with id {residentId}");
        }

        /// <summary>
        /// AddColorAsymc adds media to the its respectively list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="url">is the url to add to the <see cref="Resident"/>'s list</param>
        /// <param name="mediaType"></param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        private async Task AddMediaAsync(string residentId, string url, EMediaType mediaType)
        {
            // parse the id
            if (!ObjectId.TryParse(residentId, out var residentObjectId))
                // if it fails, throw not found exception
                throw new NotFoundException($"The {typeof(Resident).Name} with id {residentId} could not be found");

            // use the data service to create a new updater
            var updated = await ((IResidentsService) DataService).AddMediaAsync(residentObjectId, url, mediaType);

            if (!updated)
                throw new Exception(
                    $"Could not add url media of type {mediaType.ToString()} to resident with id {residentId}");
        }

        #endregion post (create)

        #region get (read)

        /// <inheritdoc cref="ARestControllerBase{T}.GetAsync(string[])" />
        /// <summary>
        /// Get is supposed to return all the Items in the database wrapped in an <see cref="IActionResult"/>. 
        /// To limit data traffic it is possible to select only a number of properties.
        /// <para/>
        /// By default only the properties in the selector <see cref="PropertiesToSendOnGetAll"/> are returned.
        /// </summary>
        /// <param name="propertiesToInclude">are the properties of which the values should be returned</param>
        /// <returns>All <see cref="Resident"/>s in the database but only the given properties are filled in</returns>
        /// <exception cref="WebArgumentException">When one ore more properties could not be converted to selectors</exception>
        [HttpGet]
        public override async Task<IEnumerable<Resident>> GetAsync([FromQuery] string[] propertiesToInclude)
            => await base.GetAsync(propertiesToInclude);

        /// <inheritdoc cref="ARestControllerBase{T}.GetAsync(string,string[])" />
        /// <summary>
        /// Get is supposed to return the <see cref="Resident"/> with the given id in the database. 
        /// To limit data traffic it is possible to select only a number of properties.
        /// <para/>
        /// By default all properties are returned.
        /// </summary>
        /// <param name="id">is the id of the <see cref="Resident"/> to get</param>
        /// <param name="propertiesToInclude">are the properties of which the values should be returned</param>
        /// <returns>The <see cref="Resident"/> in the database that has the given id</returns>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="WebArgumentException">When one ore more properties could not be converted to selectors</exception>
        [HttpGet("{id}")]
        public override async Task<Resident> GetAsync(string id, [FromQuery] string[] propertiesToInclude)
            => await base.GetAsync(id, propertiesToInclude);

        /// <inheritdoc cref="IResidentsController.GetAsync(int, string[])" />
        /// <summary>
        /// Get returns the <see cref="Resident"/> that holds a given tag in the database. 
        /// To limit data traffic it is possible to select only a number of properties.
        /// <para/>
        /// By default all properties are returned.
        /// </summary>
        /// <param name="tag">is the tag of the <see cref="Resident"/> to get</param>
        /// <param name="propertiesToInclude">are the properties of which the values should be returned</param>
        /// <returns>The <see cref="Resident"/> in the database that has the given id</returns>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="WebArgumentException">When one ore more properties could not be converted to selectors</exception>
        [HttpGet("byTag/{tag}")]
        public async Task<Resident> GetAsync(int tag, [FromQuery] string[] propertiesToInclude)
        {
            // convert the property names to selectors, if there are any
            var selectors = !EnumerableExtensions.IsNullOrEmpty(propertiesToInclude)
                ? ConvertStringsToSelectors(propertiesToInclude)
                : null;

            // return the fetched resident, if it is null, throw a not found exception
            return await ((IResidentsService) DataService).GetAsync(tag, selectors)
                   ?? throw new NotFoundException($"The {typeof(Resident).Name} with id {tag} could not be found");
        }

        /// <inheritdoc cref="ARestControllerBase{T}.GetPropertyAsync" />
        /// <summary>
        /// GetProperty returns the value of the asked property of the asked <see cref="Resident"/>.
        /// </summary>
        /// <param name="id">is the id of the <see cref="Resident"/></param>
        /// <param name="propertyName">is the name of the property to return</param>
        /// <returns>The value of the asked property</returns>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="WebArgumentException">When the property could not be found on <see cref="Resident"/></exception>
        [HttpGet("{id}/{propertyName}")]
        public override async Task<object> GetPropertyAsync(string id, string propertyName)
            => await base.GetPropertyAsync(id, propertyName);

        #endregion get (read)

        #region put (update)

        /// <inheritdoc cref="ARestControllerBase{T}.UpdateAsync" />
        /// <summary>
        /// Update updates the fields of the <see cref="Resident"/> that are specified in the <see cref="properties"/> parameter.
        /// If the item doesn't exist, a new is created in the database.
        /// <para/>
        /// By default all properties are updated.
        /// </summary>
        /// <param name="item">is the <see cref="Resident"/> to update</param>
        /// <param name="propertiesToUpdate">contains the properties that should be updated</param>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="WebArgumentException">When one ore more properties could not be converted to selectors</exception>
        [HttpPut]
        public override async Task UpdateAsync([FromBody] Resident item, [FromQuery] string[] properties)
            => await base.UpdateAsync(item, properties);

        /// <inheritdoc cref="IRestController{T}.UpdatePropertyAsync"/>
        /// <summary>
        /// UpdatePropertyAsync is supposed to update the jsonValue of the asked property of the asked <see cref="Resident"/>.
        /// </summary>
        /// <param name="id">is the id of the <see cref="Resident"/></param>
        /// <param name="propertyName">is the name of the property to update</param>
        /// <param name="jsonValue">is the new jsonValue of the property</param>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="WebArgumentException">When the property could not be found on <see cref="Resident"/> or the jsonValue could not be assigned</exception>
        /// <exception cref="Exception">When the update failed</exception>
        [HttpPut("{id}/{propertyName}")]
        public override async Task UpdatePropertyAsync(string id, string propertyName, [FromBody] string jsonValue)
            => await base.UpdatePropertyAsync(id, propertyName, jsonValue);

        #endregion put (update)

        #region delete

        /// <inheritdoc cref="ARestControllerBase{T}.DeleteAsync" />
        /// <summary>
        /// Delete is supposed to remove the <see cref="Resident"/> with the passed id from the database.
        /// </summary>
        /// <param name="id">is the id of the <see cref="Resident"/> to remove from the database</param>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be removed</exception>
        [HttpDelete("{id}")]
        public override async Task DeleteAsync(string id)
            => await base.DeleteAsync(id);

        /// <inheritdoc cref="IResidentsController.RemoveVideoAsync" />
        /// <summary>
        /// RemoveMusicAsync removes music from the music list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="musicId">is the id of the music to remove from the <see cref="Resident"/>'s music list</param>
        /// <exception cref="NotFoundException">
        /// When the <see cref="residentId"/>/<see cref="musicId"/> cannot be parsed or <see cref="Resident"/>/<see cref="MediaWithId"/> not found
        /// </exception>
        /// <exception cref="Exception">When the item could not be removed</exception>
        [HttpDelete("{residentId}/Music")]
        public async Task RemoveVideoAsync(string residentId, string musicId)
            => await RemoveMediaAsync(residentId, musicId, EMediaType.Audio);

        /// <inheritdoc cref="IResidentsController.RemoveVideoAsync" />
        /// <summary>
        /// RemoveVideoAsync removes video from the video list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="videoId">is the id of the video to remove from the <see cref="Resident"/>'s video list</param>
        /// <exception cref="NotFoundException">
        /// When the <see cref="residentId"/>/<see cref="videoId"/> cannot be parsed or <see cref="Resident"/>/<see cref="MediaWithId"/> not found
        /// </exception>
        /// <exception cref="Exception">When the item could not be removed</exception>
        [HttpDelete("{residentId}/Videos")]
        public async Task RemoveMusicAsync(string residentId, string videoId)
            => await RemoveMediaAsync(residentId, videoId, EMediaType.Video);

        /// <inheritdoc cref="IResidentsController.RemoveImageAsync" />
        /// <summary>
        /// RemoveImageAsync removes image from the image list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="imageId">is the id of the image to remove from the <see cref="Resident"/>'s image list</param>
        /// <exception cref="NotFoundException">
        /// When the <see cref="residentId"/>/<see cref="imageId"/> cannot be parsed or <see cref="Resident"/>/<see cref="MediaWithId"/> not found
        /// </exception>
        /// <exception cref="Exception">When the item could not be removed</exception>
        [HttpDelete("{residentId}/Images")]
        public async Task RemoveImageAsync(string residentId, string imageId)
            => await RemoveMediaAsync(residentId, imageId, EMediaType.Image);

        /// <inheritdoc cref="IResidentsController.RemoveColorAsync" />
        /// <summary>
        /// RemoveColorAsync removes color from the color list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="colorId">is the id of the color to remove from the <see cref="Resident"/>'s color list</param>
        /// <exception cref="NotFoundException">
        /// When the <see cref="residentId"/>/<see cref="colorId"/> cannot be parsed or <see cref="Resident"/>/<see cref="MediaWithId"/> not found
        /// </exception>
        /// <exception cref="Exception">When the item could not be removed</exception>
        [HttpDelete("{residentId}/Colors")]
        public async Task RemoveColorAsync(string residentId, string colorId)
            => await RemoveMediaAsync(residentId, colorId, EMediaType.Color);

        /// <inheritdoc cref="IResidentsController.RemoveColorAsync" />
        /// <summary>
        /// RemoveColorAsync removes a media item from the respectively list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="mediaId">is the id of the color to remove from the <see cref="Resident"/>'s list</param>
        /// <param name="mediaType">is the type of media to remove from the <see cref="Resident"/>'s list</param>
        /// <exception cref="NotFoundException">
        /// When the <see cref="residentId"/>/<see cref="mediaId"/> cannot be parsed or <see cref="Resident"/>/<see cref="MediaWithId"/> not found
        /// </exception>
        /// <exception cref="Exception">When the item could not be removed</exception>
        private async Task RemoveMediaAsync(string residentId, string mediaId, EMediaType mediaType)
        {
            // parse the resident id
            if (!ObjectId.TryParse(residentId, out var residentObjectId))
                // if it fails, throw not found exception
                throw new NotFoundException($"The {typeof(Resident).Name} with id {mediaId} could not be found");

            // parse the media id
            if (!ObjectId.TryParse(mediaId, out var mediaObjectId))
                // if it fails, throw not found exception
                throw new NotFoundException($"The {mediaType.ToString()} with id {mediaId} could not be found");

            // remove the media from the database
            var removed =
                await ((IResidentsService) DataService).RemoveMediaAsync(residentObjectId, mediaObjectId, mediaType);

            // use the data service to create a new updater
            if (!removed)
                throw new Exception($"The {mediaType.ToString()} with id {mediaId} could not be removed");
        }

        #endregion delete

        #endregion METHODS
    }
}