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
using WebService.Controllers.Bases;
using WebService.Helpers.Attributes;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Services.Logging;
using WebService.Models;
using WebService.Models.Bases;
using WebService.Services.Data;
using ArgumentException = WebService.Helpers.Exceptions.ArgumentException;
using ArgumentNullException = WebService.Helpers.Exceptions.ArgumentNullException;

namespace WebService.Controllers
{
    [Route(Routes.ResidentsModulesController)]
    [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
    public class ResidentsController : ARestControllerBase<Resident>, IResidentsController
    {
        #region CONSTRUCTOR

        public ResidentsController(IResidentsService dataService, ILogger logger, IUsersService usersService)
            : base(dataService, logger, usersService)
        {
        }

        #endregion CONSTRUCTOR


        #region PROPERTIES

        protected override IEnumerable<Expression<Func<Resident, object>>> PropertiesToSendOnGetAll { get; }
            = new Expression<Func<Resident, object>>[]
            {
                x => x.FirstName,
                x => x.LastName,
                x => x.Room,
                x => x.Birthday,
                x => x.Doctor,
            };

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

            var properties = new Expression<Func<User, object>>[] {x => x.Residents, x => x.UserType, x => x.Group};
            var user = await GetCurrentUser(properties);
            var isResponsible = false;
            switch (user.UserType)
            {
                case EUserType.SysAdmin:
                    isResponsible = true;
                    break;
                case EUserType.Nurse:
                    var residentRoom = await DataService.GetPropertyAsync(residentObjectId, x => x.Room);
                    isResponsible = new Regex($@"^{residentRoom}[0-9]*$").IsMatch(user.Group);
                    break;
                case EUserType.User:
                    isResponsible = user.Residents.Contains(residentObjectId);
                    break;
                case EUserType.Guest:
                case EUserType.Module:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return isResponsible
                ? residentObjectId
                : throw new NotFoundException<Resident>(nameof(IModelWithID.Id), id);
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
                    var residentRoom = await DataService.GetPropertyAsync(residentObjectId, x => x.Room);
                    isResponsible = new Regex($@"^{residentRoom}[0-9]*$").IsMatch(user.Group);
                    break;
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


        private async Task<bool> CanWriteDataToResidentAsync(int tag)
        {
            var properties = new Expression<Func<User, object>>[] {x => x.Residents, x => x.UserType, x => x.Group};
            var user = await GetCurrentUser(properties);
            switch (user.UserType)
            {
                case EUserType.SysAdmin:
                    return true;
                case EUserType.Nurse:
                    var residentRoom = await ((IResidentsService) DataService).GetPropertyAsync(tag, x => x.Room);
                    return new Regex($@"^{residentRoom}[0-9]*$").IsMatch(user.Group);
                case EUserType.User:
                case EUserType.Module:
                case EUserType.Guest:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
                    var residentRoom = await ((IResidentsService) DataService).GetPropertyAsync(tag, x => x.Room);
                    return new Regex($@"^{residentRoom}[0-9]*$").IsMatch(user.Group);
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

        [Authorize(EUserType.SysAdmin, EUserType.Nurse)]
        [HttpPost(Routes.Create)]
        public override Task<string> CreateAsync([FromBody] Resident item)
            => base.CreateAsync(item);

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User)]
        [HttpPost(Routes.AddMusicData)]
        public Task<StatusCodeResult> AddMusicAsync(string id, [FromForm] MultiPartFile musicData)
            => AddMediaAsync(id, musicData, EMediaType.Audio, (int) 20e6);

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User)]
        [HttpPost(Routes.AddVideoData)]
        public Task<StatusCodeResult> AddVideoAsync(string id, [FromForm] MultiPartFile videoData)
            => AddMediaAsync(id, videoData, EMediaType.Video, (int) 1e9);

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User)]
        [HttpPost(Routes.AddImageData)]
        public Task<StatusCodeResult> AddImageAsync(string id, [FromForm] MultiPartFile imageData)
            => AddMediaAsync(id, imageData, EMediaType.Image, (int) 20e6);

        public async Task<StatusCodeResult> AddMediaAsync(string id, MultiPartFile data, EMediaType mediaType,
            int maxFileSize = int.MaxValue)
        {
            if (data?.File == null)
                throw new ArgumentNullException(nameof(data));

            var residentObjectId = await CanWriteDataToResidentAsync(id);

            try
            {
                var bytes = data.ConvertToBytes(maxFileSize);
                var title = data.File.FileName;
                await ((IResidentsService) DataService)
                    .AddMediaAsync(residentObjectId, title, bytes, mediaType, data.File.ContentType.Split('/')[1]);
                return StatusCode((int) HttpStatusCode.Created);
            }
            catch (FileToLargeException)
            {
                throw new FileToLargeException(maxFileSize);
            }
        }

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User)]
        [HttpPost(Routes.AddMusicUrl)]
        public Task<StatusCodeResult> AddMusicAsync(string id, [FromBody] string url)
            => AddMediaAsync(id, url, EMediaType.Audio);

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User)]
        [HttpPost(Routes.AddVideoUrl)]
        public Task<StatusCodeResult> AddVideoAsync(string id, [FromBody] string url)
            => AddMediaAsync(id, url, EMediaType.Video);

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User)]
        [HttpPost(Routes.AddImageUrl)]
        public Task<StatusCodeResult> AddImageAsync(string id, [FromBody] string url)
            => AddMediaAsync(id, url, EMediaType.Image);

        public async Task<StatusCodeResult> AddMediaAsync(string id, string url, EMediaType mediaType)
        {
            var residentObjectId = await CanWriteDataToResidentAsync(id);

            await ((IResidentsService) DataService).AddMediaAsync(residentObjectId, url, mediaType);
            return StatusCode((int) HttpStatusCode.Created);
        }


        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User)]
        [HttpPost(Routes.AddColor)]
        public async Task<StatusCodeResult> AddColorAsync(string id, [FromBody] Color colorData)
        {
            var residentObjectId = await CanWriteDataToResidentAsync(id);

            await DataService.AddItemToListProperty(residentObjectId, x => x.Colors, colorData);
            return StatusCode((int) HttpStatusCode.Created);
        }

        [Authorize(EUserType.SysAdmin)]
        [HttpPost(Routes.AddTag)]
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

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.Module, EUserType.User)]
        [HttpGet(Routes.GetAll)]
        public override async Task<IEnumerable<Resident>> GetAllAsync(string[] propertiesToInclude)
        {
            var properties = new Expression<Func<User, object>>[] {x => x.Residents, x => x.UserType};
            var user = await GetCurrentUser(properties);

            switch (user.UserType)
            {
                case EUserType.SysAdmin:
                case EUserType.Nurse:
                    return await base.GetAllAsync(propertiesToInclude);
                case EUserType.User:
                    return await base.GetAllAsync(propertiesToInclude);
                default:
                    throw new UnauthorizedException(EUserType.SysAdmin, EUserType.Nurse, EUserType.User);
            }
        }

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.Module, EUserType.User)]
        [HttpGet(Routes.GetOne)]
        public override Task<Resident> GetOneAsync(string id, string[] propertiesToInclude)
            => base.GetOneAsync(id, propertiesToInclude);

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.Module, EUserType.User)]
        [HttpGet(Routes.GetProperty)]
        public override Task<object> GetPropertyAsync(string id, string propertyName)
            => base.GetPropertyAsync(id, propertyName);

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.Module, EUserType.User)]
        [HttpGet(Routes.GetPicture)]
        public async Task<FileResult> GetPictureAsync(string id)
        {
            var objectId = await CanGetDataFromResidentAsync(id);
            var picture = await DataService.GetPropertyAsync(objectId, x => x.Picture);

            return picture == null
                ? throw new NotFoundException<Resident>($"Resident with id {id} has no picture")
                : File(picture, "image/jpg");
        }


        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.Module)]
        [HttpGet(Routes.GetByTag)]
        [HttpGet(Routes.GetByTagOld)]
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

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.Module)]
        [HttpGet(Routes.GetRandomElementFromProperty)]
        [HttpGet(Routes.GetRandomElementFromPropertyOld)]
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

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.Module)]
        [HttpGet(Routes.GetPropertyByTag)]
        [HttpGet(Routes.GetPropertyByTagOld)]
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

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User)]
        [HttpPut(Routes.Update)]
        public override Task UpdateAsync([FromBody] Resident item, [FromQuery] string[] properties)
            => base.UpdateAsync(item, properties);

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User)]
        [HttpPut(Routes.UpdateProperty)]
        public override Task UpdatePropertyAsync(string id, string propertyName, [FromBody] string jsonValue)
            => base.UpdatePropertyAsync(id, propertyName, jsonValue);

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.User)]
        [HttpPut(Routes.UpdatePicture)]
        public async Task UpdatePictureAsync(string id, [FromForm] MultiPartFile picture)
        {
            const int maxFileSize = (int) 10e6;
            if (picture?.File == null)
                throw new ArgumentNullException(nameof(picture));

            var objectId = await CanGetDataFromResidentAsync(id);

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

        #endregion put (update)


        #region delete

        [Authorize(EUserType.SysAdmin, EUserType.Nurse)]
        [HttpDelete(Routes.Delete)]
        public override Task DeleteAsync(string id)
            => base.DeleteAsync(id);

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.Module, EUserType.User)]
        [HttpDelete(Routes.RemoveMusic)]
        public Task RemoveMusicAsync(string id, string musicId)
            => RemoveMediaAsync(id, musicId, EMediaType.Audio);

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.Module, EUserType.User)]
        [HttpDelete(Routes.RemoveVideo)]
        public Task RemoveVideoAsync(string id, string videoId)
            => RemoveMediaAsync(id, videoId, EMediaType.Video);

        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.Module, EUserType.User)]
        [HttpDelete(Routes.RemoveImage)]
        public Task RemoveImageAsync(string id, string imageId)
            => RemoveMediaAsync(id, imageId, EMediaType.Image);

        public async Task RemoveMediaAsync(string id, string mediaId, EMediaType mediaType)
        {
            var mediaObjectId = mediaId.ToObjectId();
            var residentObjectId = await CanWriteDataToResidentAsync(id);

            await ((IResidentsService) DataService).RemoveMediaAsync(residentObjectId, mediaObjectId, mediaType);
        }


        [Authorize(EUserType.SysAdmin, EUserType.Nurse, EUserType.Module, EUserType.User)]
        [HttpDelete(Routes.RemoveColor)]
        public async Task RemoveColorAsync(string id, [FromBody] Color color)
        {
            var residentObjectId = await CanWriteDataToResidentAsync(id);

            await ((IResidentsService) DataService)
                .RemoveSubItemAsync(residentObjectId, x => x.Colors, color);
        }

        [Authorize(EUserType.SysAdmin, EUserType.Nurse)]
        [HttpDelete(Routes.RemoveTag)]
        public async Task RemoveTag(string id, int tag)
        {
            var objectId = await CanWriteDataToResidentAsync(id);

            await DataService.RemoveItemFromList(objectId, x => x.Tags, tag);
        }

        #endregion delete

        #endregion METHODS
    }
}