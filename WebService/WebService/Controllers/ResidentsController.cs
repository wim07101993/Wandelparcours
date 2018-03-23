using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
using WebService.Services.Data;
using WebService.Services.Exceptions;

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
        #region FIELDS

        public const string AddMusicDataTemplate = "{residentId}/Music/data";
        public const string AddVideoDataTemplate = "{residentId}/Videos/data";
        public const string AddImageDataTemplate = "{residentId}/Images/data";
        public const string AddColorTemplate = "{residentId}/Colors/data";

        public const string AddMusicUrlTemplate = "{residentId}/Music/url";
        public const string AddVideoUrlTemplate = "{residentId}/Videos/url";
        public const string AddMImageUrlTemplate = "{residentId}/Images/url";

        public const string GetByTagTemplate = "byTag/{tag}";
        public const string GetRandomElementFromPropertyTemplate = "byTag/{tag}/{propertyName}/random";
        public const string GetPropertyByTagTemplate = "byTag/{tag}/{propertyName}";

        public const string RemoveMusicTemplate = "{residentId}/Music/{musicId}";
        public const string RemoveVideoTemplate = "{residentId}/Videos/{videoId}";
        public const string RemoveImageTemplate = "{residentId}/Images/{imageId}";
        public const string RemoveColorTemplate = "{residentId}/Colors";

        #endregion FIELDS


        #region CONSTRUCTOR

        public ResidentsController(IThrow iThrow, IResidentsService dataService, ILogger logger)
            : base(iThrow, dataService, logger)
        {
        }

        #endregion CONSTRUCTOR


        #region PROPERTIES

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

        public override IDictionary<string, Expression<Func<Resident, object>>> PropertySelectors { get; } =
            new Dictionary<string, Expression<Func<Resident, object>>>
            {
                {nameof(Resident.Birthday), x => x.Birthday},
                {nameof(Resident.Colors), x => x.Colors},
                {nameof(Resident.Doctor), x => x.Doctor},
                {nameof(Resident.FirstName), x => x.FirstName},
                {nameof(Resident.Images), x => x.Images},
                {nameof(Resident.LastName), x => x.LastName},
                {nameof(Resident.LastRecordedPosition), x => x.LastRecordedPosition},
                {nameof(Resident.Locations), x => x.Locations},
                {nameof(Resident.Music), x => x.Music},
                {nameof(Resident.ImagePicture), x => x.ImagePicture},
                {nameof(Resident.Room), x => x.Room},
                {nameof(Resident.Tags), x => x.Tags},
                {nameof(Resident.Videos), x => x.Videos},
                {nameof(Resident.Id), x => x.Id}
            };

        #endregion PROPERTIES


        #region METHODS

        #region post (create)

        [HttpPost(AddMusicDataTemplate)]
        public Task<StatusCodeResult> AddMusicAsync(string residentId, [FromForm] MultiPartFile musicData)
            => AddMediaAsync(residentId, musicData, EMediaType.Audio, (int) 20e6);

        [HttpPost(AddVideoDataTemplate)]
        public Task<StatusCodeResult> AddVideoAsync(string residentId, [FromForm] MultiPartFile videoData)
            => AddMediaAsync(residentId, videoData, EMediaType.Video, (int) 1e9);

        [HttpPost(AddImageDataTemplate)]
        public Task<StatusCodeResult> AddImageAsync(string residentId, [FromForm] MultiPartFile imageData)
            => AddMediaAsync(residentId, imageData, EMediaType.Image, (int) 20e6);

        public async Task<StatusCodeResult> AddMediaAsync(string residentId, MultiPartFile data, EMediaType mediaType,
            int maxFileSize = int.MaxValue)
        {
            if (data?.File == null)
            {
                Throw.NullArgument(nameof(data));
                return null;
            }

            // parse the id
            if (!ObjectId.TryParse(residentId, out var residentObjectId))
            {
                // if it fails, throw not found exception
                Throw.NotFound<Resident>(residentId);
                return null;
            }

            try
            {
                var bytes = data.ConvertToBytes(maxFileSize);
                var title = data.File.FileName;
                await ((IResidentsService) DataService).AddMediaAsync(residentObjectId, title, bytes, mediaType,
                    data.File.ContentType.Split('/')[1]);
                return StatusCode((int) HttpStatusCode.Created);
            }
            catch (FileToLargeException)
            {
                Throw.FileToLarge(maxFileSize);
            }

            return null;
        }

        [HttpPost(AddMusicUrlTemplate)]
        public Task<StatusCodeResult> AddMusicAsync(string residentId, [FromBody] string url)
            => AddMediaAsync(residentId, url, EMediaType.Audio);

        [HttpPost(AddVideoUrlTemplate)]
        public Task<StatusCodeResult> AddVideoAsync(string residentId, [FromBody] string url)
            => AddMediaAsync(residentId, url, EMediaType.Video);

        [HttpPost("{residentId}/Images/url")]
        public Task<StatusCodeResult> AddImageAsync(string residentId, [FromBody] string url)
            => AddMediaAsync(residentId, url, EMediaType.Image);

        public async Task<StatusCodeResult> AddMediaAsync(string residentId, string url, EMediaType mediaType)
        {
            // parse the id
            if (!ObjectId.TryParse(residentId, out var residentObjectId))
            {
                // if it fails, throw not found exception
                Throw.NotFound<Resident>(residentId);
                return null;
            }

            // use the data service to create a new updater
            await ((IResidentsService) DataService).AddMediaAsync(residentObjectId, url, mediaType);
            return StatusCode((int) HttpStatusCode.Created);
        }


        [HttpPost("{residentId}/Colors/data")]
        public async Task<StatusCodeResult> AddColorAsync(string residentId, [FromBody] Color colorData)
        {
            // parse the id
            if (!ObjectId.TryParse(residentId, out var residentObjectId))
            {
                // if it fails, throw not found exception
                Throw.NotFound<Resident>(residentId);
                return null;
            }

            await DataService.AddItemToListProperty(residentObjectId, x => x.Colors, colorData);
            return StatusCode((int) HttpStatusCode.Created);
        }

        #endregion post (create)


        #region get (read)

        [HttpGet(GetByTagTemplate)]
        public async Task<Resident> GetByTagAsync(int tag, [FromQuery] string[] propertiesToInclude)
        {
            // convert the property names to selectors, if there are any
            var selectors = !EnumerableExtensions.IsNullOrEmpty(propertiesToInclude)
                ? ConvertStringsToSelectors(propertiesToInclude)
                : null;

            var resident = await ((IResidentsService) DataService).GetAsync(tag, selectors);

            // return the fetched resident, if it is null, throw a not found exception
            if (resident == null)
            {
                Throw.NotFound<Resident>(tag);
                return null;
            }

            return resident;
        }

        [HttpGet(GetRandomElementFromPropertyTemplate)]
        public async Task<object> GetRandomElementFromPropertyAsync(int tag, string propertyName)
        {
            IList data;

            switch (propertyName.ToUpperCamelCase())
            {
                case nameof(Resident.Music):
                    data = (await ((IResidentsService) DataService).GetAsync(tag,
                        new Expression<Func<Resident, object>>[] {x => x.Music})).Music;
                    break;
                case nameof(Resident.Videos):
                    data = (await ((IResidentsService) DataService).GetAsync(tag,
                        new Expression<Func<Resident, object>>[] {x => x.Videos})).Videos;
                    break;
                case nameof(Resident.Images):
                    data = (await ((IResidentsService) DataService).GetAsync(tag,
                        new Expression<Func<Resident, object>>[] {x => x.Images})).Images;
                    break;
                case nameof(Resident.Colors):
                    data = (await ((IResidentsService) DataService).GetAsync(tag,
                        new Expression<Func<Resident, object>>[] {x => x.Colors})).Colors;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return data.RandomItem();
        }

        [HttpGet(GetPropertyTemplate)]
        public async Task<object> GetPropertyAsync(int tag, string propertyName)
        {
            // check if the property exists on the item
            if (!typeof(Resident).GetProperties().Any(x => x.Name.EqualsWithCamelCasing(propertyName)))
                throw new WebArgumentException(
                    $"Property {propertyName} cannot be found on {typeof(Resident).Name}", nameof(propertyName));

            // get the property from the database
            return await ((IResidentsService) DataService).GetPropertyAsync(tag,
                PropertySelectors[propertyName.ToUpperCamelCase()]);
        }

        #endregion get (read)


        #region put (update)

        [HttpPut]
        public override Task UpdateAsync([FromBody] Resident item, [FromQuery] string[] properties)
            => base.UpdateAsync(item, properties);

        [HttpPut("{id}/{propertyName}")]
        public override Task UpdatePropertyAsync(string id, string propertyName, [FromBody] string jsonValue)
            => base.UpdatePropertyAsync(id, propertyName, jsonValue);

        [HttpPut("{id}/picture")]
        public async Task UpdatePictureAsync(string id, [FromForm] MultiPartFile picture)
        {
            const int maxFileSize = (int) 10e6;
            if (picture?.File == null)
            {
                Throw.NullArgument(nameof(picture));
                return;
            }

            if (!ObjectId.TryParse(id, out var objectId))
            {
                Throw.NotFound<Resident>(id);
                return;
            }

            try
            {
                var bytes = picture.ConvertToBytes(maxFileSize);

                await DataService.UpdatePropertyAsync(objectId, x => x.ImagePicture, bytes);
            }
            catch (FileToLargeException)
            {
                Throw.FileToLarge(maxFileSize);
            }
        }

        #endregion put (update)

        #region delete

        [HttpDelete(RemoveMusicTemplate)]
        public Task RemoveMusicAsync(string residentId, string musicId)
            => RemoveMediaAsync(residentId, musicId, EMediaType.Audio);

        [HttpDelete(RemoveVideoTemplate)]
        public Task RemoveVideoAsync(string residentId, string videoId)
            => RemoveMediaAsync(residentId, videoId, EMediaType.Video);

        [HttpDelete(RemoveImageTemplate)]
        public Task RemoveImageAsync(string residentId, string imageId)
            => RemoveMediaAsync(residentId, imageId, EMediaType.Image);

        public async Task RemoveMediaAsync(string residentId, string mediaId, EMediaType mediaType)
        {
            // parse the resident id
            if (!ObjectId.TryParse(residentId, out var residentObjectId))
            {
                // if it fails, throw not found exception
                Throw.NotFound<Resident>(residentId);
                return;
            }

            // parse the media id
            if (!ObjectId.TryParse(mediaId, out var mediaObjectId))
            {
                // if it fails, throw not found exception
                Throw.NotFound<MediaData>(mediaId);
                return;
            }

            // remove the media from the database
            await ((IResidentsService) DataService).RemoveMediaAsync(residentObjectId, mediaObjectId, mediaType);
        }

        [HttpDelete(RemoveColorTemplate)]
        public async Task RemoveColorAsync(string residentId, [FromBody] Color color)
        {
            // parse the resident id
            if (!ObjectId.TryParse(residentId, out var residentObjectId))
            {
                // if it fails, throw not found exception
                Throw.NotFound<Resident>(residentId);
                return;
            }

            // remove the media from the database
            await ((IResidentsService) DataService)
                .RemoveSubItemAsync(residentObjectId, x => x.Colors, color);
        }

        #endregion delete

        #endregion METHODS
    }
}