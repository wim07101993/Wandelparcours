using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebService.Controllers.Bases;
using WebService.Helpers.Attributes;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Models.Bases;
using WebService.Services.Authorization;
using WebService.Services.Data;
using WebService.Services.Logging;

namespace WebService.Controllers
{
    [Route("api/v1/[controller]")]
    public class MediaController : ARestControllerBase<MediaData>, IMediaController
    {
        #region FIELDS

        public const string GetOneFileWithExtensionTemplate = "{id}/file.{extension}";
        public const string GetFileTemplate = "{id}/file";

        private readonly IResidentsService _residentsService;

        #endregion FIELDS


        #region COSNTRUCTROS

        public MediaController(IMediaService dataService, IUsersService usersService, ILogger logger,
            ITokenService tokenService, IResidentsService residentsService)
            : base(dataService, logger, usersService)
        {
            _residentsService = residentsService;
        }

        #endregion CONSTRUCTORS


        #region PROPERTIES

        public override IEnumerable<Expression<Func<MediaData, object>>> PropertiesToSendOnGetAll { get; } = null;

        public override IDictionary<string, Expression<Func<MediaData, object>>> PropertySelectors { get; } =
            new Dictionary<string, Expression<Func<MediaData, object>>>
            {
                {nameof(MediaData.Id), x => x.Id},
                {nameof(MediaData.Data), x => x.Data}
            };

        #endregion PROPERTIES


        #region METHODS

        #region auth

        private async Task<ObjectId> CanGetMediaAsync(string mediaId)
        {
            if (!ObjectId.TryParse(mediaId, out var objectId))
                throw new NotFoundException<MediaData>(nameof(IModelWithID.Id), mediaId);
            
            var properties = new Expression<Func<User, object>>[] {x => x.Residents, x => x.UserType, x => x.Group};
            var user = await GetCurrentUser(properties);
            
            var residentId = await ((IMediaService) DataService).GetPropertyAsync(objectId, x => x.OwnerId);
            
            var isResponsible = false;
            switch (user.UserType)
            {
                case EUserType.SysAdmin:
                case EUserType.Module:
                    isResponsible = true;
                    break;
                case EUserType.Nurse:
                    var residentRoom = await _residentsService.GetPropertyAsync(residentId, x => x.Room);
                    isResponsible = new Regex($@"^{residentRoom}[0-9]*$").IsMatch(user.Group);
                    break;
                case EUserType.User:
                    isResponsible = user.Residents.Contains(residentId);
                    break;
                case EUserType.Guest:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return isResponsible
                ? objectId
                    : throw  new NotFoundException<MediaData>(nameof(IModelWithID.Id), mediaId);
        }

        #endregion auth


        #region read

        [Authorize(EUserType.Module, EUserType.SysAdmin, EUserType.User, EUserType.User)]
        [HttpGet(GetOneFileWithExtensionTemplate)]
        public async Task<FileContentResult> GetOneAsync(string id, string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
                throw new NotFoundException<MediaData>(nameof(MediaData.Extension), extension);

            var objectId = await CanGetMediaAsync(id);

            var data = await ((IMediaService) DataService).GetOneAsync(objectId, extension);
            if (data == null)
                throw new NotFoundException<MediaData>(nameof(IModelWithID.Id), id);

            var mediaType = extension.GetEMediaTypeFromExtension();
            return File(data, $"{mediaType.ToString().ToLower()}/{extension}");
        }

        [Authorize(EUserType.Module, EUserType.SysAdmin, EUserType.User, EUserType.User)]
        [HttpGet(GetFileTemplate)]
        public async Task<FileContentResult> GetFileAsync(string id)
        {
            var objectId = await CanGetMediaAsync(id);
            
            var media = await ((IMediaService) DataService).GetOneAsync(objectId,
                new Expression<Func<MediaData, object>>[] {x => x.Data});

            return Equals(media, default(MediaData))
                ? throw new NotFoundException<MediaData>(nameof(IModelWithID.Id), id)
                : File(media.Data, "image/jpg");
        }

        #endregion read

        #endregion METHDOS
    }
}