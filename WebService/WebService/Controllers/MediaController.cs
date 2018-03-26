using System;
using System.Collections.Generic;
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
    [Route("api/v1/[controller]")]
    public class MediaController : ARestControllerBase<MediaData>, IMediaController
    {
        #region FIELDS

        public const string GetOneWithExtensionTemplate = "{id}.{extension}";

        private readonly IUsersService _usersService;
        private readonly ITokenService _tokenService;

        #endregion FIELDS


        #region COSNTRUCTROS

        public MediaController(IMediaService dataService, IUsersService usersService, ILogger logger,
            ITokenService tokenService)
            : base(dataService, logger, usersService)
        {
            _usersService = usersService;
            _tokenService = tokenService;
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

        #region read

        [Authorize(EUserType.Module, EUserType.SysAdmin, EUserType.User, EUserType.User)]
        [HttpGet(GetOneWithExtensionTemplate)]
        public async Task<FileContentResult> GetOneAsync(string id, string extension, [FromHeader] string token)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                throw new NotFoundException<MediaData>(nameof(IModelWithID.Id), id);

            if (string.IsNullOrWhiteSpace(extension))
                throw new NotFoundException<MediaData>(nameof(MediaData.Extension), extension);

            var residentId = await ((IMediaService) DataService).GetPropertyAsync(objectId, x => x.OwnerId);
            var userId = await _tokenService.GetIdFromToken(token);

            var userResidents = (IEnumerable<ObjectId>) await _usersService.GetPropertyAsync(userId, x => x.Residents);
            if (userResidents.All(x => x != residentId))
                throw new NotFoundException<MediaData>(nameof(IModelWithID.Id), id);

            var data = await ((IMediaService) DataService).GetOneAsync(objectId, extension);
            if (data == null)
                throw new NotFoundException<MediaData>(nameof(IModelWithID.Id), id);

            var mediaType = extension.GetEMediaTypeFromExtension();
            return File(data, $"{mediaType.ToString().ToLower()}/{extension}");
        }

        [Authorize(EUserType.Module, EUserType.SysAdmin, EUserType.User, EUserType.User)]
        [HttpGet(GetOneTemplate)]
        public async Task<FileContentResult> GetOneAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                throw new NotFoundException<MediaData>(nameof(IModelWithID.Id), id);

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