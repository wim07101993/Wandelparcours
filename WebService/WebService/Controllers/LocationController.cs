using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebService.Controllers.Bases;
using WebService.Helpers.Exceptions;
using WebService.Models;
using WebService.Models.Bases;
using WebService.Services.Data;
using WebService.Services.Logging;

namespace WebService.Controllers
{
    [Route("api/v1/[controller]")]
    [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
    public class LocationController : ARestControllerBase<ResidentLocation>, ILocationController
    {
        private readonly IResidentsService _residentService;
        private readonly ILocationService _locationService;


        public LocationController(ILocationService dataService, ILogger logger, IResidentsService residentService,
            IUsersService usersService, ILocationService locationService)
            : base(dataService, logger, usersService)
        {
            _residentService = residentService;
            _locationService = locationService;
        }


        public override IEnumerable<Expression<Func<ResidentLocation, object>>> PropertiesToSendOnGetAll { get; } =
            null;

        public override IDictionary<string, Expression<Func<ResidentLocation, object>>> PropertySelectors { get; }
            = new Dictionary<string, Expression<Func<ResidentLocation, object>>>
            {
                {nameof(ResidentLocation.Id), x => x.Id},
                {nameof(ResidentLocation.Id), x => x.ResidentId},
                {nameof(ResidentLocation.Id), x => x.TimeStamp},
                {nameof(ResidentLocation.Id), x => x.X},
                {nameof(ResidentLocation.Id), x => x.Y},
            };


        [HttpPost]
        public override async Task<string> CreateAsync([FromBody] ResidentLocation item)
        {
            item.TimeStamp = DateTime.Now;
            return await base.CreateAsync(item);
        }

        [HttpGet("residents/{id}/lastlocation")]
        public async Task<Resident> GetLastLocationOneResident(string id)
        {
            var selectors = new Expression<Func<Resident, object>>[]
                {x => x.LastRecordedPosition, x => x.Id, x => x.LastName, x => x.FirstName};
            if (!ObjectId.TryParse(id, out var objectid))
                throw new NotFoundException<Resident>(nameof(IModelWithID.Id), id);

            return await _residentService.GetOneAsync(objectid, selectors);
        }

        [HttpGet("residents/lastlocation")]
        public async Task<IEnumerable<Resident>> GetLastLocation()
        {
            var selectors = new Expression<Func<Resident, object>>[]
                {x => x.LastRecordedPosition, x => x.Id, x => x.LastName, x => x.FirstName};

            var lastposition = await _residentService.GetAsync(selectors);

            return lastposition.Where(resident => resident.LastRecordedPosition != null);
        }

        [HttpGet]
        public async Task<IEnumerable<ResidentLocation>> GetSince([FromQuery] int since)
        {
            return since == 0
                ? await GetAllAsync(null)
                : await _locationService.GetSinceAsync(DateTime.Now - TimeSpan.FromMinutes(since));
        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<ResidentLocation>> GetSince(string id, [FromQuery] int since)
        {
            if (!ObjectId.TryParse(id, out var objectid))
                throw new NotFoundException<Resident>(nameof(IModelWithID.Id), id);

            return since == 0
                ? await _locationService.GetSinceAsync(default(DateTime), objectid)
                : await _locationService.GetSinceAsync(DateTime.Now - TimeSpan.FromMinutes(since), objectid);
        }

        [HttpPost("{id}/lastlocation")]
        public async Task SetLastLocation(string id, [FromBody] Point currentLocation)
        {
            currentLocation.TimeStamp = DateTime.Now;
            if (!ObjectId.TryParse(id, out var objectid))
                throw new NotFoundException<Resident>(nameof(IModelWithID.Id), id);

            await _residentService.UpdatePropertyAsync(objectid, x => x.LastRecordedPosition, currentLocation);
        }
    }
}