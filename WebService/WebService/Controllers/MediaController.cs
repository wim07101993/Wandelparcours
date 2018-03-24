using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebService.Controllers.Bases;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Authorization;
using WebService.Services.Data;
using WebService.Services.Exceptions;
using WebService.Services.Logging;

namespace WebService.Controllers
{
    [Route("api/v1/[controller]")]
    public class MediaController : ARestControllerBase<MediaData>, IMediaController
    {
        private readonly IUsersService _usersService;
        private readonly ITokenService _tokenService;
        private readonly IResidentsService _residentsService;

        public MediaController(IThrow iThrow, IMediaService dataService, IUsersService usersService, ILogger logger, ITokenService tokenService,
            IResidentsService residentsService)
            : base(iThrow, dataService, logger)
        {
            _usersService = usersService;
            _tokenService = tokenService;
            _residentsService = residentsService;
        }

        public override IEnumerable<Expression<Func<MediaData, object>>> PropertiesToSendOnGetAll { get; } = null;

        public override IDictionary<string, Expression<Func<MediaData, object>>> PropertySelectors { get; } =
            new Dictionary<string, Expression<Func<MediaData, object>>>
            {
                {nameof(MediaData.Id), x => x.Id},
                {nameof(MediaData.Data), x => x.Data}
            };


        [HttpGet(@"{id}.{extension}")]
        public async Task<FileContentResult> GetOneAsync(string id, string extension, [FromHeader] string token)
        {
            // parse the id
            if (!ObjectId.TryParse(id, out var objectId))
                // if it fails, throw not found exception
                throw new NotFoundException($"The {typeof(MediaData).Name} with id {id} could not be found");
            
            if (string.IsNullOrWhiteSpace(extension))
                throw new NotFoundException("there is no data with no extension");

            var residentId = (ObjectId) await ((IMediaService) DataService).GetPropertyAsync(objectId, x => x.OwnerId);
            var userId = await _tokenService.GetIdFromToken(token);

            var userResidents = (IEnumerable<ObjectId>) await _usersService.GetPropertyAsync(userId, x => x.Residents);
            if (userResidents.All(x => x != residentId))
                throw new NotFoundException($"The {typeof(MediaData).Name} with id {id} could not be found");
            
            var data = await ((IMediaService) DataService).GetOneAsync(objectId, extension);
            if (data == null)
                throw new NotFoundException($"The {typeof(MediaData).Name} with id {id} could not be found");
            
            var mediaType = extension.GetEMediaTypeFromExtension();
            return File(data, $"{mediaType.ToString().ToLower()}/{extension}");
        }

        [HttpGet(@"{id}")]
        public async Task<FileContentResult> GetOneAsync(string id)
        {
            // parse the id
            if (!ObjectId.TryParse(id, out var objectId))
                // if it fails, throw not found exception
                throw new NotFoundException($"The {typeof(MediaData).Name} with id {id} could not be found");

            // get the jsonValue from the data service
            var media = await ((IMediaService) DataService).GetOneAsync(objectId,
                new Expression<Func<MediaData, object>>[]
                {
                    x => x.Data
                });
            return Equals(media, default(MediaData))
                // if the item is null, throw a not found exception
                ? throw new NotFoundException($"The {typeof(MediaData).Name} with id {id} could not be found")
                // else return the values
                : File(media.Data, "image/jpg");
        }
    }
}