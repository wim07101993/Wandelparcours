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
        #region CONSTRUCTOR

        public ResidentsController(IThrow iThrow, IDataService<Resident> dataService, ILogger logger)
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
                // {nameof(Resident.Colors), x => x.Colors},
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

        [HttpPost]
        public override Task CreateAsync([FromBody] Resident item)
            => base.CreateAsync(item);

        [HttpPost("{residentId}/Music/data")]
        public Task AddMusicAsync(string residentId, [FromForm] MultiPartFile musicData)
            => AddMediaAsync(residentId, musicData, EMediaType.Audio, (int) 20e6);

        [HttpPost("{residentId}/Music/url")]
        public Task AddMusicAsync(string residentId, [FromBody] string url)
            => AddMediaAsync(residentId, url, EMediaType.Audio);

        [HttpPost("{residentId}/Videos/data")]
        public Task AddVideoAsync(string residentId, [FromForm] MultiPartFile videoData)
            => AddMediaAsync(residentId, videoData, EMediaType.Video, (int) 1e9);

        [HttpPost("{residentId}/Videos/url")]
        public Task AddVideoAsync(string residentId, [FromBody] string url)
            => AddMediaAsync(residentId, url, EMediaType.Video);

        [HttpPost("{residentId}/Images/data")]
        public Task AddImageAsync(string residentId, [FromForm] MultiPartFile imageData)
            => AddMediaAsync(residentId, imageData, EMediaType.Image, (int) 20e6);

        [HttpPost("{residentId}/Images/url")]
        public Task AddImageAsync(string residentId, [FromBody] string url)
            => AddMediaAsync(residentId, url, EMediaType.Image);

        [HttpPost("{residentId}/Colors/data")]
        public async Task AddColorAsync(string residentId, [FromBody] byte[] colorData)
        {
            // parse the id
            if (!ObjectId.TryParse(residentId, out var residentObjectId))

            {
                // if it fails, throw not found exception
                Throw.NotFound<Resident>(residentId);
            }

            // TODO
        }

        [HttpPost("{residentId}/Colors/url")]
        public async Task AddColorAsync(string residentId, [FromBody] string url)
        {
            // TODO
        }
        public async Task AddMediaAsync(string residentId, MultiPartFile data, EMediaType mediaType,

            int maxFileSize = int.MaxValue)
        {
            if (data?.File == null)
            {
                Throw.NullArgument(nameof(data));
                return;
            }

            // parse the id
            if (!ObjectId.TryParse(residentId, out var residentObjectId))
            {
                // if it fails, throw not found exception
                Throw.NotFound<Resident>(residentId);
                return;
            }

            try
            {
                var bytes = data.ConvertToBytes(maxFileSize);

                await ((IResidentsService) DataService).AddMediaAsync(residentObjectId, bytes, mediaType);
            }
            catch (FileToLargeException)
            {
                Throw.FileToLarge(maxFileSize);
            }
        }

        public async Task AddMediaAsync(string residentId, string url, EMediaType mediaType)
        {
            // parse the id
            if (!ObjectId.TryParse(residentId, out var residentObjectId))
            {
                // if it fails, throw not found exception
                Throw.NotFound<Resident>(residentId);
                return;
            }

            // use the data service to create a new updater
            await ((IResidentsService) DataService).AddMediaAsync(residentObjectId, url, mediaType);
        }

        #endregion post (create)

        #region get (read)

        [HttpGet]
        public override Task<IEnumerable<Resident>> GetAsync([FromQuery] string[] propertiesToInclude)
            => base.GetAsync(propertiesToInclude);

        [HttpGet("{id}")]
        public override Task<Resident> GetAsync(string id, [FromQuery] string[] propertiesToInclude)
            => base.GetAsync(id, propertiesToInclude);

        [HttpGet("byTag/{tag}")]
        public async Task<Resident> GetAsync(int tag, [FromQuery] string[] propertiesToInclude)
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

        [HttpGet("{tag}/{mediaType}/random")]
        public async Task<string> GetRandomMedia(int tag, string mediaType)
        {
            if (!Enum.TryParse<EMediaType>(mediaType, out var eMediaType))
            {
                Throw.MediaTypeNotFound<EMediaType>(mediaType);
                return null;
            }

            IList<MediaUrl> media;

            switch (eMediaType)
            {
                case EMediaType.Audio:
                    media = (await ((IResidentsService) DataService).GetAsync(tag,
                        new Expression<Func<Resident, object>>[] {x => x.Music})).Music;
                    break;
                case EMediaType.Video:
                    media = (await ((IResidentsService) DataService).GetAsync(tag,
                        new Expression<Func<Resident, object>>[] {x => x.Videos})).Videos;
                    break;
                case EMediaType.Image:
                    media = (await ((IResidentsService) DataService).GetAsync(tag,
                        new Expression<Func<Resident, object>>[] {x => x.Images})).Images;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return media.RandomItem().Url;
        }

        [HttpGet("{id}/{propertyName}")]
        public override Task<object> GetPropertyAsync(string id, string propertyName)
            => base.GetPropertyAsync(id, propertyName);

        #endregion get (read)

        #region put (update)

        [HttpPut]
        public override Task UpdateAsync([FromBody] Resident item, [FromQuery] string[] properties)
            => base.UpdateAsync(item, properties);

        [HttpPut("{id}/{propertyName}")]
        public override Task UpdatePropertyAsync(string id, string propertyName, [FromBody] string jsonValue)
            => base.UpdatePropertyAsync(id, propertyName, jsonValue);

        #endregion put (update)

        #region delete

        [HttpDelete("{id}")]
        public override Task DeleteAsync(string id)
            => base.DeleteAsync(id);

        [HttpDelete("{residentId}/Videos")]
        public  Task RemoveVideoAsync(string residentId, string videoId)
            =>  RemoveMediaAsync(residentId, videoId, EMediaType.Video);

        [HttpDelete("{residentId}/Music")]
        public Task RemoveMusicAsync(string residentId, string musicId)
            => RemoveMediaAsync(residentId, musicId, EMediaType.Audio);

        [HttpDelete("{residentId}/Images")]
        public Task RemoveImageAsync(string residentId, string imageId)
            => RemoveMediaAsync(residentId, imageId, EMediaType.Image);

        [HttpDelete("{residentId}/Colors")]
        public async Task RemoveColorAsync(string residentId, string colorId)
        {
            // TODO
        }

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

        #endregion delete

        #endregion METHODS
    }
}