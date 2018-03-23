using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Controllers.Bases;
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

        [HttpGet(GetOneTemplate)]
        public async Task<IActionResult> GetOneAsync(string id) 
            => File((await base.GetOneAsync(id, null)).Data, "image/jpg");
    }
}