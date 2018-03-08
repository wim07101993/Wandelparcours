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
        /// Residentscontroller creates an instance of the <see cref="T:WebService.Controllers.ResidentsController" /> class. 
        /// </summary>
        /// <param name="dataService">is a service to handle the database connection</param>
        /// <param name="logger">is a service to hanlde the logging of messages</param>
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

        /// <inheritdoc cref="ARestControllerBase{T}.ConvertStringsToSelectors" />
        /// <summary>
        /// ConvertStringsToSelectors converts a collection of property names to their selector in the form of 
        /// <see cref="Expression{TDelegate}"/> of type <see cref="Func{TInput,TResult}"/>
        /// </summary>
        /// <param name="strings">are the property names to convert to selectors</param>
        /// <returns>An <see cref="IEnumerable{TDelegate}"/> that contains the converted selectors</returns>
        /// <exception cref="WebArgumentException">When one ore more properties could not be converted to selectors</exception>
        public override IEnumerable<Expression<Func<Resident, object>>> ConvertStringsToSelectors(
            IEnumerable<string> strings)
        {
            // create a new list of selectors
            var selectors = new List<Expression<Func<Resident, object>>>();

            // fill the list of selectors by iterating over the properties to update
            foreach (var propertyName in strings)
            {
                // if the name of a properties matches a property of a Value, 
                // add the corresponding selector
                if (propertyName.EqualsWithCamelCasing(nameof(Resident.Birthday)))
                    selectors.Add(x => x.Birthday);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.Colors)))
                    selectors.Add(x => x.Colors);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.Doctor)))
                    selectors.Add(x => x.Doctor);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.FirstName)))
                    selectors.Add(x => x.FirstName);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.Images)))
                    selectors.Add(x => x.Images);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.LastName)))
                    selectors.Add(x => x.LastName);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.LastRecordedPosition)))
                    selectors.Add(x => x.LastRecordedPosition);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.Locations)))
                    selectors.Add(x => x.Locations);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.Music)))
                    selectors.Add(x => x.Music);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.Picture)))
                    selectors.Add(x => x.Picture);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.Room)))
                    selectors.Add(x => x.Room);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.Tags)))
                    selectors.Add(x => x.Tags);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.Videos)))
                    selectors.Add(x => x.Videos);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.Id)))
                    // the id is always passed on get and ignored on update
                    // ReSharper disable once RedundantJumpStatement
                    continue;
                else
                    throw new WebArgumentException(
                        $"Property {propertyName} cannot be found on {typeof(Resident).Name}", nameof(strings));
            }

            return selectors;
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
        [HttpPost("{residentId}/Music")]
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
        [HttpPost("{residentId}/Music")]
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
        [HttpPost("{residentId}/Videos")]
        public async Task AddVideoAsync(string residentId, byte[] videoData)
            => await AddMediaAsync(residentId, videoData, EMediaType.Video);

        /// <inheritdoc cref="IResidentsController.AddVideoAsync(string,string)"/>
        /// <summary>
        /// AddVideoAsymc adds video to the video list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="url">is the url to the video to add to the <see cref="Resident"/>'s video list</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        [HttpPost("{residentId}/Videos")]
        public async Task AddVideoAsync(string residentId, string url)
            => await AddMediaAsync(residentId, url, EMediaType.Video);

        /// <inheritdoc cref="IResidentsController.AddImageAsync(string,byte[])"/>
        /// <summary>
        /// AddImageAsymc adds image to the image list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="imageData">is the image to add to the <see cref="Resident"/>'s image list</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        [HttpPost("{residentId}/Images")]
        public async Task AddImageAsync(string residentId, byte[] imageData)
            => await AddMediaAsync(residentId, imageData, EMediaType.Image);

        /// <inheritdoc cref="IResidentsController.AddImageAsync(string,string)"/>
        /// <summary>
        /// AddImageAsymc adds image to the image list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="url">is the url to the image to add to the <see cref="Resident"/>'s image list</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        [HttpPost("{residentId}/Images")]
        public async Task AddImageAsync(string residentId, string url)
            => await AddMediaAsync(residentId, url, EMediaType.Image);

        /// <inheritdoc cref="IResidentsController.AddColorAsync(string,byte[])"/>
        /// <summary>
        /// AddColorAsymc adds color to the color list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="colorData">is the color to add to the <see cref="Resident"/>'s color list</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        [HttpPost("{residentId}/Colors")]
        public async Task AddColorAsync(string residentId, byte[] colorData)
            => await AddMediaAsync(residentId, colorData, EMediaType.Color);

        /// <inheritdoc cref="IResidentsController.AddColorAsync(string,string)"/>
        /// <summary>
        /// AddColorAsymc adds color to the color list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="url">is the url to the color to add to the <see cref="Resident"/>'s color list</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        [HttpPost("{residentId}/Colors")]
        public async Task AddColorAsync(string residentId, string url)
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

        /// <inheritdoc cref="ARestControllerBase{T}.GetAsync(string[])" />
        /// <summary>
        /// Get is supposed to return all the Items in the database wrapped in an <see cref="IActionResult"/>. 
        /// To limit data traffic it is possible to select only a number of propertie.
        /// <para/>
        /// By default only the properties in the selector <see cref="PropertiesToSendOnGetAll"/> are returned.
        /// </summary>
        /// <param name="propertiesToInclude">are the properties of which the values should be returned</param>
        /// <returns>All <see cref="Resident"/>s in the database but only the given properties are filled in</returns>
        /// <exception cref="WebArgumentException">When one ore more properties could not be converted to selectors</exception>
        [HttpGet]
        public override async Task<IEnumerable<Resident>> GetAsync([FromQuery] string[] propertiesToInclude)
            => await base.GetAsync(propertiesToInclude);

        #endregion get (read)

        #region put (update)

        /// <inheritdoc cref="ARestControllerBase{T}.UpdateAsync" />
        /// <summary>
        /// Update updates the fields of the <see cref="Resident"/> that are specified in the <see cref="propertiesToUpdate"/> parameter.
        /// If the item doesn't exist, a new is created in the database.
        /// <para/>
        /// By default all properties are updated.
        /// </summary>
        /// <param name="item">is the <see cref="Resident"/> to update</param>
        /// <param name="propertiesToUpdate">contains the properties that should be updated</param>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="WebArgumentException">When one ore more properties could not be converted to selectors</exception>
        [HttpPut]
        public override async Task UpdateAsync([FromBody] Resident item, [FromQuery] string[] propertiesToUpdate)
            => await base.UpdateAsync(item, propertiesToUpdate);

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
        [HttpPost("{residentId}/Music")]
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
        [HttpPost("{residentId}/Videos")]
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
        [HttpPost("{residentId}/Images")]
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
        [HttpPost("{residentId}/Colors")]
        public async Task RemoveColorAsync(string residentId, string colorId)
            => await RemoveMediaAsync(residentId, colorId, EMediaType.Color);

        /// <inheritdoc cref="IResidentsController.RemoveColorAsync" />
        /// <summary>
        /// RemoveColorAsync removes a media ietm from the respectively list of a <see cref="Resident"/>.
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