using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
    [Route(Routes.RestBase.ControllerRoute)]
    public class MediaController : AControllerBase, IMediaController
    {
        #region FIELDS

        private readonly IMediaService _mediaService;
        private readonly IResidentsService _residentsService;

        #endregion FIELDS


        #region COSNTRUCTROS

        public MediaController(IMediaService mediaService, IUsersService usersService, ILogger logger,
            ITokensService tokensService, IResidentsService residentsService)
            : base(usersService)
        {
            _mediaService = mediaService;
            _residentsService = residentsService;
        }

        #endregion CONSTRUCTORS


        #region METHODS

        #region auth

        private async Task<ObjectId> CanGetMediaAsync(string mediaId)
        {
            var objectId = mediaId.ToObjectId();

            var properties = new Expression<Func<User, object>>[] {x => x.Residents, x => x.UserType, x => x.Group};
            var user = await GetCurrentUser(properties);

            var residentId = await _mediaService.GetOwner(objectId);

            var isResponsible = false;
            switch (user.UserType)
            {
                case EUserType.SysAdmin:
                case EUserType.Module:
                    isResponsible = true;
                    break;
                case EUserType.Nurse:
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
                : throw new NotFoundException<MediaData>(nameof(IModelWithID.Id), mediaId);
        }

        #endregion auth


        #region read

        [Authorize(EUserType.Module, EUserType.Nurse, EUserType.User)]
        [HttpGet(Routes.Media.GetOneFileWithExtension)]
        public async Task<FileStreamResult> GetOneAsync(string id, string extension, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(extension))
                throw new NotFoundException<MediaData>(nameof(MediaData.Extension), extension);

            var objectId = await CanGetMediaAsync(id);

            var memoryStream = new MemoryStream();
            try
            {
                await _mediaService.GetOneAsync(objectId, memoryStream);
            }
            catch (Exception e)
            {
                throw;
            }
            
            memoryStream.Seek(0, 0);
            var mediaType = extension.GetEMediaTypeFromExtension();
            return File(memoryStream, $"{mediaType.ToString().ToLower()}/{extension}");
        }

        [Authorize(EUserType.Module, EUserType.Nurse, EUserType.User)]
        [HttpGet(Routes.Media.GetFile)]
        public async Task<FileStreamResult> GetFileAsync(string id, [FromQuery] string token)
        {
            var objectId = await CanGetMediaAsync(id);

            var memoryStream = new MemoryStream();
            try
            {
                await _mediaService.GetOneAsync(objectId, memoryStream);
            }
            catch (Exception e)
            {
                throw;
            }

            memoryStream.Seek(0, 0);
            return File(memoryStream, "image/jpg");
        }

        #endregion read

        #endregion METHDOS
    }
}