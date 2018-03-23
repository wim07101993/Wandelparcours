using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebService.Controllers.Bases;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Data;
using WebService.Services.Exceptions;
using WebService.Services.Logging;

namespace WebService.Controllers
{
    [Route("api/v1/[controller]")]
    public class MediaController : ARestControllerBase<MediaData>, IMediaController
    {
        public MediaController(IThrow iThrow, IMediaService dataService, ILogger logger) : base(iThrow,
            dataService, logger)
        {
        }

        public override IEnumerable<Expression<Func<MediaData, object>>> PropertiesToSendOnGetAll { get; } = null;

        public override IDictionary<string, Expression<Func<MediaData, object>>> PropertySelectors { get; } =
            new Dictionary<string, Expression<Func<MediaData, object>>>
            {
                {nameof(MediaData.Id), x => x.Id},
                {nameof(MediaData.Data), x => x.Data}
            };

        [HttpGet(@"{id}.{extension}")]
        public async Task<FileContentResult> GetOneAsync(string id, string extension)
        {
            // parse the id
            if (!ObjectId.TryParse(id, out var objectId))
                // if it fails, throw not found exception
                throw new NotFoundException($"The {typeof(MediaData).Name} with id {id} could not be found");

            if (string.IsNullOrWhiteSpace(extension))
                throw new NotFoundException($"there is no data with no extension");

            // get the jsonValue from the data service
            var data = await ((IMediaService) DataService).GetOneAsync(objectId, extension);

            var mediaType = extension.GetEMediaTypeFromExtension();

            return data == null
                // if the item is null, throw a not found exception
                ? throw new NotFoundException($"The {typeof(MediaData).Name} with id {id} could not be found")
                // else return the values
                : File(data, $"{mediaType.ToString().ToLower()}/{extension}");
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
                new Expression<Func<MediaData, object>>[] {x => x.Data});

            return Equals(media, default(MediaData))
                // if the item is null, throw a not found exception
                ? throw new NotFoundException($"The {typeof(MediaData).Name} with id {id} could not be found")
                // else return the values
                : File(media.Data, "image/jpg");
        }
    }
}