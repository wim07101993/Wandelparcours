using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebService.Controllers.Bases;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Services.Logging;
using WebService.Models;
using WebService.Models.Bases;
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
        /// <see cref="Expression{TDelagete}" /> of type <see cref="Func{TResult}" />
        /// </summary>
        /// <param name="strings">are the property names to convert to selectors</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the converted selectors</returns>
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
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.Id))) continue;
                else
                    throw new WebArgumentException(
                        $"Property {propertyName} cannot be found on {typeof(Resident).Name}", nameof(strings));
            }

            return selectors;
        }

        #region get (read)

        /// <inheritdoc cref="ARestControllerBase{T}.GetAsync(string,string[])" />
        /// <summary>
        /// Get returns the Item with the given id in the database wrapped in an <see cref="IActionResult"/>. 
        /// To limit data traffic it is possible to select only a number of properties
        /// </summary>
        /// <returns>The <see cref="T"/> in the database that has the given id</returns>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="T"/> not found</exception>
        /// <exception cref="WebArgumentException">When the properties could not be converted to selectors</exception>
        [HttpGet("{id}")]
        public override async Task<Resident> GetAsync(string id, [FromQuery] string[] properties)
            => await base.GetAsync(id, properties);

        /// <inheritdoc cref="ARestControllerBase{T}.GetAsync(string[])" />
        /// <summary>
        /// Get returns all the Items in the database wrapped in an <see cref="IActionResult"/>. 
        /// To limit data traffic it is possible to select only a number of properties by default. 
        /// By default the properties in the <see cref="PropertiesToSendOnGetAll"/> are the only ones sent.
        /// </summary>
        /// <returns>
        /// All <see cref="T"/>s in the database but only the given properties are filled in
        /// </returns>
        /// <exception cref="WebArgumentException">When the properties could not be converted to selectors</exception>
        [HttpGet]
        public override async Task<IEnumerable<Resident>> GetAsync([FromQuery] string[] properties)
            => await base.GetAsync(properties);

        /// <inheritdoc cref="IRestController{T}.GetAsync(string,string[])" />
        /// <summary>
        /// Get returns the Item with the given tag in the database wrapped in an <see cref="IActionResult"/>. 
        /// To limit data traffic it is possible to select only a number of properties
        /// </summary>
        /// <returns>The <see cref="Resident"/> in the database that has the given tag</returns>
        /// <exception cref="NotFoundException">When the no <see cref="Resident"/> has the given tag</exception>
        /// <exception cref="WebArgumentException">When the properties could not be converted to selectors</exception>
        [HttpGet("byTag/{tag}")]
        public async Task<Resident> GetAsync(int tag, [FromQuery] string[] properties)
        {
            //create selectors
            IEnumerable<Expression<Func<Resident, object>>> selectors = null;
            // if there are no properties, they don't need to be converted
            if (!EnumerableExtensions.IsNullOrEmpty(properties))
                // convert the property names to selectors
                selectors = ConvertStringsToSelectors(properties);

            // get the item with the tag from the data service
            var item = await ((IResidentsService) DataService).GetAsync(tag, selectors);

            return item ?? throw new NotFoundException($"The {typeof(Resident).Name} with id {tag} could not be found");
        }

        #endregion get (read)

        #region post (create)

        /// <inheritdoc cref="ARestControllerBase{T}.CreateAsync" />
        /// <summary>
        /// Create saves the passed <see cref="Resident"/> to the database.
        /// </summary>
        /// <param name="item">is the <see cref="Resident"/> to save in the database</param>
        /// <returns>A 201 Created status code if the <see cref="Resident"/> is created in the database</returns>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="Resident"/> not found</exception>
        [HttpPost]
        public override async Task CreateAsync([FromBody] Resident item)
            => await base.CreateAsync(item);

        [HttpPost("{residentId}/Music")]
        public async Task AddMusicAsync(string residentId, [FromBody] byte[] data)
            => await AddMediaAsync(residentId, data, EMediaType.Audio);

        [HttpPost("{residentId}/Music")]
        public async Task AddMusicAsync(string residentId, [FromBody] string url)
            => await AddMediaAsync(residentId, url, EMediaType.Audio);

        [HttpPost("{residentId}/Videos")]
        public async Task AddVideoAsync(string residentId, byte[] data)
            => await AddMediaAsync(residentId, data, EMediaType.Video);

        [HttpPost("{residentId}/Videos")]
        public async Task AddVideoAsync(string residentId, string url)
            => await AddMediaAsync(residentId, url, EMediaType.Video);

        [HttpPost("{residentId}/Images")]
        public async Task AddImageAsync(string residentId, byte[] data)
            => await AddMediaAsync(residentId, data, EMediaType.Image);

        [HttpPost("{residentId}/Images")]
        public async Task AddImageAsync(string residentId, string url)
            => await AddMediaAsync(residentId, url, EMediaType.Image);

        [HttpPost("{residentId}/Colors")]
        public async Task AddColorAsync(string residentId, byte[] data)
            => await AddMediaAsync(residentId, data, EMediaType.Color);

        [HttpPost("{residentId}/Colors")]
        public async Task AddColorAsync(string residentId, string url)
            => await AddMediaAsync(residentId, url, EMediaType.Color);

        private async Task<IActionResult> AddMediaAsync(string residentId, byte[] data, EMediaType mediaType)
        {
            // parse the id
            if (!ObjectId.TryParse(residentId, out var residentObjectId))
                // if it fails, return a 404
                return StatusCode((int) HttpStatusCode.NotFound);

            try
            {
                // use the data service to create a new updater
                return await ((IResidentsService) DataService).AddMediaAsync(residentObjectId, data, mediaType)
                    // if the updater was created return satus created
                    ? StatusCode((int) HttpStatusCode.OK)
                    // if the updater was not created return status not modified
                    : StatusCode((int) HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                // log the error
                Logger.Log(this, ELogLevel.Error, e);
                // return a 500 internal server error code
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        private async Task<IActionResult> AddMediaAsync(string residentId, string url, EMediaType mediaType)
        {
            // parse the id
            if (!ObjectId.TryParse(residentId, out var residentObjectId))
                // if it fails, return a 404
                return StatusCode((int) HttpStatusCode.NotFound);

            try
            {
                // use the data service to create a new updater
                return await ((IResidentsService) DataService).AddMediaAsync(residentObjectId, url, mediaType)
                    // if the updater was created return satus created
                    ? StatusCode((int) HttpStatusCode.OK)
                    // if the updater was not created return status not modified
                    : StatusCode((int) HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                // log the error
                Logger.Log(this, ELogLevel.Error, e);
                // return a 500 internal server error code
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        #endregion post (create)

        #region delete

        /// <inheritdoc cref="ARestControllerBase{T}.DeleteAsync" />
        /// <summary>
        /// Delete deletes the passed <see cref="Resident" /> from the database.
        /// </summary>
        /// <param name="id">is the id of the <see cref="Resident" /> to remove from the database</param>
        /// <returns>
        /// - Status created (201) if succes
        /// - Status not found (40) if there was no erro but also no object to remove
        /// - Status internal server error (500) on error
        /// </returns>
        [HttpDelete("{id}")]
        public override async Task DeleteAsync(string id)
            => await base.DeleteAsync(id);

        /// <summary>
        /// RemoveVideoAsync is the method corre
        /// </summary>
        /// <param name="residentId"></param>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        [HttpPost("{residentId}/Music")]
        public async Task RemoveVideoAsync(string residentId, string mediaId)
            => await RemoveMediaAsync(residentId, mediaId, EMediaType.Audio);

        [HttpPost("{residentId}/Videos")]
        public async Task RemoveMusicAsync(string residentId, string mediaId)
            => await RemoveMediaAsync(residentId, mediaId, EMediaType.Video);

        [HttpPost("{residentId}/Images")]
        public async Task RemoveImageAsync(string residentId, string mediaId)
            => await RemoveMediaAsync(residentId, mediaId, EMediaType.Image);

        [HttpPost("{residentId}/Colors")]
        public async Task RemoveColorAsync(string residentId, string mediaId)
            => await RemoveMediaAsync(residentId, mediaId, EMediaType.Color);

        private async Task RemoveMediaAsync(string residentId, string mediaId, EMediaType mediaType)
        {
            // parse the id
            if (!ObjectId.TryParse(residentId, out var residentObjectId))
                // if it fails, return a 404
                throw new NotFoundException();

            if (!ObjectId.TryParse(mediaId, out var mediaObjectId))
                // if it fails, return a 404
                throw new NotFoundException();

            // use the data service to create a new updater
            if (!(await ((IResidentsService) DataService).RemoveMediaAsync(residentObjectId, mediaObjectId,
                mediaType)))
                throw new NotFoundException();
        }

        #endregion delete

        #region put (update)

        [HttpPut]
        public override async Task UpdateAsync([FromBody] Resident item, [FromQuery] string[] properties)
            => await base.UpdateAsync(item, properties);

        #endregion put (update)

        #endregion METHODS
    }
}