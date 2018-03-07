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
    /// ResidentsController is a controller for the REST service.
    /// <para />
    /// It handles the reading and writing of residents data to the database.
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


        #region METHODS

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

        /// <inheritdoc cref="ARestControllerBase{T}.ConvertStringsToSelectors" />
        /// <summary>
        /// ConvertStringsToSelectors should convert a collection of property names to their selector in the form of 
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
                else
                    throw new WebArgumentException(
                        $"Property {propertyName} cannot be found on {typeof(Resident).Name}", nameof(strings));
            }

            return selectors;
        }

        #region get (read)

        /// <inheritdoc cref="ARestControllerBase{T}.GetAsync(string,string[])" />
        /// <summary>
        /// Get is the method corresponding to the GET method of the controller of the REST service.
        /// <para />
        /// It returns the Item with the given id in the database wrapped in an <see cref="IActionResult" />. To limit data traffic it is possible to
        /// select only a number of properties by default. These properties are selected with the <see cref="properties" /> property.
        /// </summary>
        /// <returns>
        /// - Status ok (200) with An IEnumerable of all the Items in the database on success
        /// - Status bad request (400) when there are properties passed that do not exist in a <see cref="Resident" />
        /// - Status not found (404) when there is no <see cref="Resident" /> with the given id found
        /// - Status internal server error (500) when an error occures
        /// </returns>
        [HttpGet("{id}")]
        public override async Task<IActionResult> GetAsync(string id, [FromQuery] string[] properties)
            => await base.GetAsync(id, properties);

        /// <inheritdoc cref="ARestControllerBase{T}.GetAsync()" />
        /// <summary>
        /// Get is the method corresponding to the GET method of the controller of the REST service.
        /// <para />
        /// It returns all the Items in the database wrapped in an <see cref="IActionResult" />. To limit data traffic it is possible to
        /// select only a number of properties by default. These properties are selected with the <see cref="PropertiesToSendOnGetAll" /> property.
        /// </summary>
        /// <returns>
        /// - Status ok (200) with An IEnumerable of all the Items in the database on success
        /// - Status internal server (500) error when an error occures
        /// </returns>
        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] string[] properties)
            => await base.GetAsync(properties);

        /// <inheritdoc cref="IResidentsController.GetAsync(int, string[])" />
        /// <summary>
        /// Get is the method corresponding to the GET method of the controller of the REST service.
        /// <para />
        /// It returns the Item with the given tag in the database wrapped in an <see cref="IActionResult" />. To limit data traffic it is possible to
        /// select only a number of properties by default. These properties are selected with the <see cref="properties" /> property.
        /// </summary>
        /// <returns>
        /// - Status ok (200) with An IEnumerable of all the Items in the database on success
        /// - Status bad request (400) when there are properties passed that do not exist in a <see cref="Resident" />
        /// - Status not found (404) when there is no <see cref="Resident" /> with the given id found
        /// - Status internal server error (500) when an error occures
        /// </returns>
        [HttpGet("byTag/{tag}")]
        public async Task<IActionResult> GetAsync(int tag, [FromQuery] string[] properties)
        {
            //create selectors
            IEnumerable<Expression<Func<Resident, object>>> selectors = null;
            // if there are no properties, they don't need to be converted
            if (!EnumerableExtensions.IsNullOrEmpty(properties))
                try
                {
                    // try converting the propertie names to selectors
                    selectors = ConvertStringsToSelectors(properties);
                }
                catch (ArgumentException)
                {
                    // if it fails because of a bad argument (properties cannot be found)
                    // return a 400 error
                    return StatusCode((int) HttpStatusCode.BadRequest);
                }

            try
            {
                // get the item with the tag from the data service
                var item = await ((IResidentsService) DataService).GetAsync(tag, selectors);

                return item == null
                    // if the item is null, return a 404
                    ? StatusCode((int) HttpStatusCode.NotFound)
                    // else return the values wrapped in a 200 response 
                    : (IActionResult) Ok(item);
            }
            catch (Exception e)
            {
                // log the exception
                Logger.Log(this, ELogLevel.Error, e);
                // return a 500 error to the client
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        #endregion get (read)

        #region post (create)

        /// <inheritdoc cref="ARestControllerBase{T}.CreateAsync" />
        /// <summary>
        /// Create is the method corresonding to the POST method of the controller of the REST service.
        /// <para />
        /// It saves the passed <see cref="Resident" /> to the database.
        /// </summary>
        /// <param name="item">is the <see cref="Resident" /> to save in the database</param>
        /// <returns>
        /// - Status created (201) if succes
        /// - Status internal server error (500) on error or not created
        /// </returns>
        [HttpPost]
        public override async Task<IActionResult> CreateAsync([FromBody] Resident item)
            => await base.CreateAsync(item);

        [HttpPost("{residentId}/Music")]
        public async Task<IActionResult> AddMusicAsync(string residentId, [FromBody] byte[] data)
            => await AddMediaAsync(residentId, data, EMediaType.Audio);

        [HttpPost("{residentId}/Music")]
        public async Task<IActionResult> AddMusicAsync(string residentId, [FromBody] string url)
            => await AddMediaAsync(residentId, url, EMediaType.Audio);

        [HttpPost("{residentId}/Videos")]
        public async Task<IActionResult> AddVideoAsync(string residentId, byte[] data)
            => await AddMediaAsync(residentId, data, EMediaType.Video);

        [HttpPost("{residentId}/Videos")]
        public async Task<IActionResult> AddVideoAsync(string residentId, string url)
            => await AddMediaAsync(residentId, url, EMediaType.Video);

        [HttpPost("{residentId}/Images")]
        public async Task<IActionResult> AddImageAsync(string residentId, byte[] data)
            => await AddMediaAsync(residentId, data, EMediaType.Image);

        [HttpPost("{residentId}/Images")]
        public async Task<IActionResult> AddImageAsync(string residentId, string url)
            => await AddMediaAsync(residentId, url, EMediaType.Image);

        [HttpPost("{residentId}/Colors")]
        public async Task<IActionResult> AddColorAsync(string residentId, byte[] data)
            => await AddMediaAsync(residentId, data, EMediaType.Color);

        [HttpPost("{residentId}/Colors")]
        public async Task<IActionResult> AddColorAsync(string residentId, string url)
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
        public override async Task<IActionResult> DeleteAsync(string id)
            => await base.DeleteAsync(id);

        /// <summary>
        /// RemoveVideoAsync is the method corre
        /// </summary>
        /// <param name="residentId"></param>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        [HttpPost("{residentId}/Music")]
        public async Task<IActionResult> RemoveVideoAsync(string residentId, string mediaId)
            => await RemoveMediaAsync(residentId, mediaId, EMediaType.Audio);

        [HttpPost("{residentId}/Videos")]
        public async Task<IActionResult> RemoveMusicAsync(string residentId, string mediaId)
            => await RemoveMediaAsync(residentId, mediaId, EMediaType.Video);

        [HttpPost("{residentId}/Images")]
        public async Task<IActionResult> RemoveImageAsync(string residentId, string mediaId)
            => await RemoveMediaAsync(residentId, mediaId, EMediaType.Image);

        [HttpPost("{residentId}/Colors")]
        public async Task<IActionResult> RemoveColorAsync(string residentId, string mediaId)
            => await RemoveMediaAsync(residentId, mediaId, EMediaType.Color);

        private async Task<IActionResult> RemoveMediaAsync(string residentId, string mediaId, EMediaType mediaType)
        {
            // parse the id
            if (!ObjectId.TryParse(residentId, out var residentObjectId))
                // if it fails, return a 404
                return StatusCode((int) HttpStatusCode.NotFound);

            if (!ObjectId.TryParse(mediaId, out var mediaObjectId))
                // if it fails, return a 404
                return StatusCode((int) HttpStatusCode.NotFound);

            try
            {
                // use the data service to create a new updater
                return await ((IResidentsService) DataService).RemoveMediaAsync(residentObjectId, mediaObjectId,
                    mediaType)
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

        #endregion delete

        #region put (update)

        /// <summary>
        /// Update is the method corresponding to the PUT method of the controller of the REST service.
        /// <para />
        /// It updates the fields of the <see cref="Resident" /> in the updater.
        /// If the Item doesn't exist, a new is created in the database.
        /// </summary>
        /// <param name="updater">containse the <see cref="Resident" /> to update ande the properties that should be updated</param>
        /// <returns>
        /// - Status ok (200) if the <see cref="Resident" /> was updated
        /// - Status created (201) if a new one was created
        /// - Status bad request (400) if the passed updater is null
        /// - Status internal server error (500) on error or not created
        /// </returns>
        [HttpPut]
        [Obsolete]
        public async Task<IActionResult> UpdateAsync([FromBody] AUpdater<Resident> updater)
            => await UpdateAsync(updater.Value, updater.PropertiesToUpdate);

        /// <inheritdoc cref="ARestControllerBase{T}.UpdateAsync" />
        /// <summary>
        /// Update is the method corresponding to the PUT method of the controller of the REST service.
        /// <para />
        /// It updates the fields of the <see cref="Resident" /> in the updater.
        /// If the Item doesn't exist, a new is created in the database.
        /// </summary>
        /// <param name="item">is the <see cref="Resident" /> to update</param>
        /// <param name="properties">contains the properties that should be updated</param>
        /// <returns>
        /// - Status ok (200) if the <see cref="Resident" /> was updated
        /// - Status created (201) if a new one was created
        /// - Status bad request (400) if the passed properties are not found on <see cref="Resident" />
        /// - Status internal server error (500) on error or not created
        /// </returns>
        [HttpPut]
        public override async Task<IActionResult> UpdateAsync([FromBody] Resident item, [FromQuery] string[] properties)
            => await base.UpdateAsync(item, properties);

        #endregion put (update)

        #endregion METHODS
    }
}