using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using VideoConverter;
using WebService.Controllers.Bases;
using WebService.Helpers.Attributes;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Services.Logging;
using WebService.Models;
using WebService.Models.Bases;
using WebService.Services.Data;
using WebService.Services.Data.Mongo;
using ArgumentException = WebService.Helpers.Exceptions.ArgumentException;
using ArgumentNullException = WebService.Helpers.Exceptions.ArgumentNullException;

namespace WebService.Controllers
{
    [Route(Routes.RestBase.ControllerRoute)]
    [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
    public class ResidentsController : ARestControllerBase<Resident>, IResidentsController
    {
        private readonly ILocationsService _locationsService;
        private readonly IVideoConverter _videoConverter;
        private static readonly bool CanConvert;

        #region CONSTRUCTOR

        public ResidentsController(ILocationsService locationsService, IResidentsService dataService, ILogger logger,
            IUsersService usersService, IVideoConverter videoConverter)
            : base(dataService, logger, usersService)
        {
            _locationsService = locationsService;
            _videoConverter = videoConverter;
        }

        static ResidentsController()
        {
            CanConvert = new VideoConverter.VideoConverter().CheckDependencies();
        }

        #endregion CONSTRUCTOR


        #region PROPERTIES

        protected override IDictionary<string, Expression<Func<Resident, object>>> PropertySelectors { get; } =
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
                {nameof(Resident.Picture), x => x.Picture},
                {nameof(Resident.Room), x => x.Room},
                {nameof(Resident.Tags), x => x.Tags},
                {nameof(Resident.Videos), x => x.Videos},
                {nameof(Resident.Id), x => x.Id}
            };

        #endregion PROPERTIES


        #region METHODS

        #region auth

        private async Task<ObjectId> CanWriteDataToResidentAsync(string id)
        {
            var residentObjectId = id.ToObjectId();
            if (!await CanWriteDataToResidentAsync(residentObjectId))
                throw new NotFoundException<Resident>(nameof(IModelWithID.Id), id);
            return residentObjectId;
        }

        private async Task<bool> CanWriteDataToResidentAsync(ObjectId id)
        {
            var properties = new Expression<Func<User, object>>[] {x => x.Residents, x => x.UserType, x => x.Group};
            var user = await GetCurrentUser(properties);
            var isResponsible = false;
            switch (user.UserType)
            {
                case EUserType.SysAdmin:
                    isResponsible = true;
                    break;
                case EUserType.Nurse:
                case EUserType.User:
                    isResponsible = user.Residents.Contains(id);
                    break;
                case EUserType.Guest:
                case EUserType.Module:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return isResponsible;
        }

        private async Task<ObjectId> CanGetDataFromResidentAsync(string id)
        {
            var residentObjectId = id.ToObjectId();

            var properties = new Expression<Func<User, object>>[] {x => x.Residents, x => x.UserType, x => x.Group};
            var user = await GetCurrentUser(properties);
            var isResponsible = false;
            switch (user.UserType)
            {
                case EUserType.SysAdmin:
                case EUserType.Module:
                    isResponsible = true;
                    break;
                case EUserType.Nurse:
                case EUserType.User:
                    isResponsible = user.Residents.Contains(residentObjectId);
                    break;
                case EUserType.Guest:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return isResponsible
                ? residentObjectId
                : throw new NotFoundException<Resident>(nameof(IModelWithID.Id), id);
        }

        private async Task<bool> CanGetDataFromResidentAsync(int tag)
        {
            var properties = new Expression<Func<User, object>>[] {x => x.Residents, x => x.UserType, x => x.Group};
            var user = await GetCurrentUser(properties);
            switch (user.UserType)
            {
                case EUserType.SysAdmin:
                case EUserType.Module:
                    return true;
                case EUserType.Nurse:
                case EUserType.User:
                    var id = await ((IResidentsService) DataService).GetPropertyAsync(tag, x => x.Id);
                    return user.Residents.Contains(id);
                case EUserType.Guest:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion auth


        #region post (create)

        [Authorize(EUserType.SysAdmin)]
        [HttpPost(Routes.RestBase.Create)]
        public override Task<string> CreateAsync([FromBody] Resident item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            item.Room = item.Room?.ToUpper();
            return base.CreateAsync(item);
        }

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpPost(Routes.Residents.AddMusicData)]
        [RequestSizeLimit(20_000_000)]
        public Task<StatusCodeResult> AddMusicAsync(string id, [FromForm] MultiPartFile musicData)
            => AddMediaAsync(id, musicData, EMediaType.Audio, (int) 20e6);

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpPost(Routes.Residents.AddVideoData)]
        [RequestSizeLimit(1_000_000_000)]
        public Task<StatusCodeResult> AddVideoAsync(string id, [FromForm] MultiPartFile videoData)
            => AddMediaAsync(id, videoData, EMediaType.Video, (int) 1e9);

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpPost(Routes.Residents.AddImageData)]
        [RequestSizeLimit(20_000_000)]
        public Task<StatusCodeResult> AddImageAsync(string id, [FromForm] MultiPartFile imageData)
            => AddMediaAsync(id, imageData, EMediaType.Image, (int) 20e6);

        public async Task<StatusCodeResult> AddMediaAsync(string id, MultiPartFile data, EMediaType mediaType,
            int maxFileSize = int.MaxValue)
        {
            if (data?.File == null)
                throw new ArgumentNullException(nameof(data));

            var residentObjectId = await CanWriteDataToResidentAsync(id);
            var convertedMedia = GetConvertedMedia(data, mediaType, maxFileSize);
            var title = data.File.FileName;
            await ((IResidentsService) DataService)
                .AddMediaAsync(residentObjectId, title, convertedMedia.Item1, mediaType, convertedMedia.Item2);
            return StatusCode((int) HttpStatusCode.Created);
        }

        private Tuple<byte[], string> GetConvertedMedia(MultiPartFile data, EMediaType mediaType, int maxFileSize)
        {
            byte[] bytes;
            string extension;

            switch (mediaType)
            {
                case EMediaType.Video:
                    if (!CanConvert)
                    {
                        var contentType = data.File.ContentType;
                        switch (contentType)
                        {
                            case "video/ogg":
                            case "video/mp4":
                            case "video/webm":
                                bytes = data.File.OpenReadStream().ToBytes(maxFileSize);
                                extension = GetExtensionFromContentType(contentType);
                                break;
                            default:
                                throw new BadMediaException($"Videos with the type {contentType} are not allowed");
                        }
                    }
                    else
                    {
                        var video = new Video(data.File.OpenReadStream());
                        bytes = _videoConverter.ConvertToWebm(video).Stream.ToBytes(maxFileSize);
                        extension = "webm";
                    }

                    break;
                default:
                    bytes = data.File.OpenReadStream().ToBytes(maxFileSize);
                    extension = GetExtensionFromContentType(data.File.ContentType);
                    break;
            }

            return new Tuple<byte[], string>(bytes, extension);
        }

        private string GetExtensionFromContentType(string contentType)
            => contentType.Split('/')[1];

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpPost(Routes.Residents.AddMusicUrl)]
        public Task<StatusCodeResult> AddMusicAsync(string id, [FromBody] string url)
            => AddMediaAsync(id, url, EMediaType.Audio);

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpPost(Routes.Residents.AddVideoUrl)]
        public Task<StatusCodeResult> AddVideoAsync(string id, [FromBody] string url)
            => AddMediaAsync(id, url, EMediaType.Video);

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpPost(Routes.Residents.AddImageUrl)]
        public Task<StatusCodeResult> AddImageAsync(string id, [FromBody] string url)
            => AddMediaAsync(id, url, EMediaType.Image);

        public async Task<StatusCodeResult> AddMediaAsync(string id, string url, EMediaType mediaType)
        {
            var residentObjectId = await CanWriteDataToResidentAsync(id);

            await ((IResidentsService) DataService).AddMediaAsync(residentObjectId, url, mediaType);
            return StatusCode((int) HttpStatusCode.Created);
        }


        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpPost(Routes.Residents.AddColor)]
        public async Task<StatusCodeResult> AddColorAsync(string id, [FromBody] Color colorData)
        {
            var residentObjectId = await CanWriteDataToResidentAsync(id);

            await DataService.AddItemToListProperty(residentObjectId, x => x.Colors, colorData);
            return StatusCode((int) HttpStatusCode.Created);
        }

        [Authorize(EUserType.SysAdmin)]
        [HttpPost(Routes.Residents.AddTag)]
        public async Task<IEnumerable<int>> AddTag(string id, [FromBody] int tag)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                throw new NotFoundException<Resident>(nameof(AModelWithID.Id), id);

            var res = await DataService
                .GetAsync(new Expression<Func<Resident, object>>[] {x => x.Tags});

            var tagExists = res
                .Any(x => x.Tags?.Any(y => y == tag) == true);

            if (tagExists)
                throw new ArgumentException("cannot have duplicate tags", nameof(tag));

            await DataService.AddItemToListProperty(objectId, x => x.Tags, tag);
            return await DataService.GetPropertyAsync(objectId, x => x.Tags);
        }

        #endregion post (create)


        #region get (read)

        [Authorize(EUserType.Nurse, EUserType.Module, EUserType.User)]
        [HttpGet(Routes.RestBase.GetAll)]
        public override async Task<IEnumerable<Resident>> GetAllAsync(string[] propertiesToInclude)
        {
            var properties = new Expression<Func<User, object>>[]
            {
                x => x.Residents,
                x => x.UserType,
                x => x.Group,
                x => x.Residents
            };
            var user = await GetCurrentUser(properties);

            var selectors = !EnumerableExtensions.IsNullOrEmpty(propertiesToInclude)
                ? ConvertStringsToSelectors(propertiesToInclude)
                : null;

            switch (user.UserType)
            {
                case EUserType.SysAdmin:
                    return await DataService.GetAsync(selectors);
                case EUserType.Nurse:
                case EUserType.User:
                    return await ((IResidentsService) DataService).GetMany(user.Residents, selectors);
                default:
                    throw new UnauthorizedException(EUserType.SysAdmin, EUserType.Nurse, EUserType.User);
            }
        }

        [Authorize(EUserType.Nurse, EUserType.Module, EUserType.User)]
        [HttpGet(Routes.RestBase.GetOne)]
        public override async Task<Resident> GetOneAsync(string id, string[] propertiesToInclude)
        {
            await CanGetDataFromResidentAsync(id);
            return await base.GetOneAsync(id, propertiesToInclude);
        }

        [Authorize(EUserType.Nurse, EUserType.Module, EUserType.User)]
        [HttpGet(Routes.RestBase.GetProperty)]
        public override async Task<object> GetPropertyAsync(string id, string propertyName)
        {
            await CanGetDataFromResidentAsync(id);
            return await base.GetPropertyAsync(id, propertyName);
        }

        [Authorize(EUserType.Nurse, EUserType.Module, EUserType.User)]
        [HttpGet(Routes.Residents.GetPicture)]
        public async Task<FileResult> GetPictureAsync(string id)
        {
            var objectId = await CanGetDataFromResidentAsync(id);
            var picture = await DataService.GetPropertyAsync(objectId, x => x.Picture);

            return picture == null
                ? throw new NotFoundException<Resident>($"Resident with id {id} has no picture")
                : File(picture, "image/jpg");
        }


        [Authorize(EUserType.Nurse, EUserType.Module)]
        [HttpGet(Routes.Residents.GetByTag)]
        public async Task<Resident> GetByTagAsync(int tag, [FromQuery] string[] propertiesToInclude)
        {
            var selectors = !EnumerableExtensions.IsNullOrEmpty(propertiesToInclude)
                ? ConvertStringsToSelectors(propertiesToInclude)
                : null;

            if (!await CanGetDataFromResidentAsync(tag))
                throw new ElementNotFoundException<Resident>(nameof(Resident.Tags), "tag");

            var resident = await ((IResidentsService) DataService).GetOneAsync(tag, selectors);

            if (resident == null)
                throw new ElementNotFoundException<Resident>(nameof(Resident.Tags), "tag");

            return resident;
        }

        [Authorize(EUserType.Nurse, EUserType.Module)]
        [HttpGet(Routes.Residents.GetRandomElementFromProperty)]
        public async Task<object> GetRandomElementFromPropertyAsync(int tag, string propertyName)
        {
            if (!await CanGetDataFromResidentAsync(tag))
                throw new ElementNotFoundException<Resident>(nameof(Resident.Tags), "tag");

            IList data;

            switch (propertyName.ToUpperCamelCase())
            {
                case nameof(Resident.Music):
                    data = (await ((IResidentsService) DataService)
                            .GetOneAsync(tag, new Expression<Func<Resident, object>>[] {x => x.Music}))
                        .Music;
                    break;
                case nameof(Resident.Videos):
                    data = (await ((IResidentsService) DataService)
                            .GetOneAsync(tag, new Expression<Func<Resident, object>>[] {x => x.Videos}))
                        .Videos;
                    break;
                case nameof(Resident.Images):
                    data = (await ((IResidentsService) DataService)
                            .GetOneAsync(tag, new Expression<Func<Resident, object>>[] {x => x.Images}))
                        .Images;
                    break;
                case nameof(Resident.Colors):
                    data = (await ((IResidentsService) DataService)
                            .GetOneAsync(tag, new Expression<Func<Resident, object>>[] {x => x.Colors}))
                        .Colors;
                    break;
                default:
                    throw new PropertyNotFoundException<Resident>(propertyName);
            }

            return data.RandomItem();
        }

        [Authorize(EUserType.Nurse, EUserType.Module)]
        [HttpGet(Routes.Residents.GetPropertyByTag)]
        public async Task<object> GetPropertyAsync(int tag, string propertyName)
        {
            if (!typeof(Resident).GetProperties().Any(x => x.Name.EqualsWithCamelCasing(propertyName)))
                throw new PropertyNotFoundException<Resident>(nameof(propertyName));

            if (!await CanGetDataFromResidentAsync(tag))
                throw new ElementNotFoundException<Resident>(nameof(Resident.Tags), "tag");

            return await ((IResidentsService) DataService)
                .GetPropertyAsync(tag, PropertySelectors[propertyName.ToUpperCamelCase()]);
        }

        #endregion get (read)


        #region put (update)

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpPut(Routes.RestBase.Update)]
        public override async Task UpdateAsync([FromBody] Resident item, [FromQuery] string[] properties)
        {
            await CanWriteDataToResidentAsync(item.Id);

            if (properties.Any(x => x.EqualsWithCamelCasing(nameof(Resident.Room))))
                item.Room = item.Room?.ToUpper();

            await base.UpdateAsync(item, properties);
        }

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpPut(Routes.RestBase.UpdateProperty)]
        public override async Task UpdatePropertyAsync(string id, string propertyName, [FromBody] string jsonValue)
        {
            if (propertyName == nameof(Resident.LastRecordedPosition))
                throw new NotFoundException();

            await CanWriteDataToResidentAsync(id);
            await base.UpdatePropertyAsync(id, propertyName, jsonValue);
        }

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpPut(Routes.Residents.UpdatePicture)]
        public async Task UpdatePictureAsync(string id, [FromForm] MultiPartFile picture)
        {
            const int maxFileSize = (int) 10e6;
            if (picture?.File == null)
                throw new ArgumentNullException(nameof(picture));

            var objectId = await CanWriteDataToResidentAsync(id);

            try
            {
                var bytes = picture.ConvertToBytes(maxFileSize);

                await DataService.UpdatePropertyAsync(objectId, x => x.Picture, bytes);
            }
            catch (FileToLargeException)
            {
                throw new FileToLargeException(maxFileSize);
            }
        }

        [Authorize(EUserType.Module)]
        [HttpPut(Routes.Residents.UpdateLastRecordedPosition)]
        public async Task UpdateLastRecordedLocation(int tag, [FromBody] ResidentLocation location)
        {
            location.Id = ObjectId.GenerateNewId();
            var id = await ((IResidentsService) DataService).GetPropertyAsync(tag, x => x.Id);
            location.ResidentId = id;
            location.TimeStamp = DateTime.Now;
            await _locationsService.CreateAsync(location);
            await ((IResidentsService) DataService)
                .UpdatePropertyAsync(tag, x => x.LastRecordedPosition, location);
            await DataService.AddItemToListProperty(id, x => x.Locations, location.Id);
        }

        #endregion put (update)


        #region delete

        [Authorize(EUserType.Nurse)]
        [HttpDelete(Routes.RestBase.Delete)]
        public override Task DeleteAsync(string id)
            => base.DeleteAsync(id);

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpDelete(Routes.Residents.RemoveMusic)]
        public Task RemoveMusicAsync(string id, string musicId)
            => RemoveMediaAsync(id, musicId, EMediaType.Audio);

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpDelete(Routes.Residents.RemoveVideo)]
        public Task RemoveVideoAsync(string id, string videoId)
            => RemoveMediaAsync(id, videoId, EMediaType.Video);

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpDelete(Routes.Residents.RemoveImage)]
        public Task RemoveImageAsync(string id, string imageId)
            => RemoveMediaAsync(id, imageId, EMediaType.Image);

        public async Task RemoveMediaAsync(string id, string mediaId, EMediaType mediaType)
        {
            var mediaObjectId = mediaId.ToObjectId();
            var residentObjectId = await CanWriteDataToResidentAsync(id);

            await ((IResidentsService) DataService).RemoveMediaAsync(residentObjectId, mediaObjectId, mediaType);
        }


        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpDelete(Routes.Residents.RemoveColor)]
        public async Task RemoveColorAsync(string id, [FromBody] Color color)
        {
            var residentObjectId = await CanWriteDataToResidentAsync(id);

            await ((IResidentsService) DataService).RemoveColor(residentObjectId, color);
        }

        [Authorize(EUserType.Nurse)]
        [HttpDelete(Routes.Residents.RemoveTag)]
        public async Task RemoveTag(string id, int tag)
        {
            var objectId = await CanWriteDataToResidentAsync(id);

            await DataService.RemoveItemFromList(objectId, x => x.Tags, tag);
        }

        #endregion delete

        #endregion METHODS
    }
}