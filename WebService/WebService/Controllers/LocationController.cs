using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebService.Controllers.Bases;
using WebService.Helpers.Attributes;
using WebService.Helpers.Exceptions;
using WebService.Models;
using WebService.Models.Bases;
using WebService.Services.Data;
using WebService.Services.Logging;

namespace WebService.Controllers
{
    [Route("api/v1/[controller]")]
    [Route("api/v1/locations")]
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


        protected override IEnumerable<Expression<Func<ResidentLocation, object>>> PropertiesToSendOnGetAll { get; } =
            null;

        protected override IDictionary<string, Expression<Func<ResidentLocation, object>>> PropertySelectors { get; }
            = new Dictionary<string, Expression<Func<ResidentLocation, object>>>
            {
                {nameof(ResidentLocation.Id), x => x.Id},
                {nameof(ResidentLocation.ResidentId), x => x.ResidentId},
                {nameof(ResidentLocation.TimeStamp), x => x.TimeStamp},
                {nameof(ResidentLocation.X), x => x.X},
                {nameof(ResidentLocation.Y), x => x.Y},
            };


        [Authorize(EUserType.Module)]
        [HttpPost]
        public override async Task<string> CreateAsync([FromBody] ResidentLocation item)
        {
            item.TimeStamp = DateTime.Now;
            return await base.CreateAsync(item);
        }

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpGet(Routes.Locations.GetLastLocationOneResident)]
        public LocalRedirectResult GetLastLocationOneResident(string id)
        {
            return LocalRedirect($"~/api/v1/residents/{id}/LastRecordedPosition");
//            var selectors = new Expression<Func<Resident, object>>[]
//                {x => x.LastRecordedPosition, x => x.Id, x => x.LastName, x => x.FirstName};
//            if (!ObjectId.TryParse(id, out var objectid))
//                throw new NotFoundException<Resident>(nameof(IModelWithID.Id), id);
//
//            return await _residentService.GetOneAsync(objectid, selectors);
        }

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpGet("residents/{tag}/lastlocation")]
        public LocalRedirectResult GetLastLocationOneResident(int tag)
        {
            return LocalRedirect($"~/api/v1/residents/{tag}/LastRecordedPosition");
//            var selectors = new Expression<Func<Resident, object>>[]
//                {x => x.LastRecordedPosition, x => x.Id, x => x.LastName, x => x.FirstName};
//           
//            return await _residentService.GetOneAsync(tag, selectors);
        }

        [Authorize(EUserType.Nurse, EUserType.User)]
        // TODO change url to api/v1/locations/lastlocations
        [HttpGet("residents/lastlocation")]
        public async Task<IEnumerable<Resident>> GetLastLocation()
        {
            var selectors = new Expression<Func<Resident, object>>[]
                {x => x.LastRecordedPosition, x => x.Id, x => x.LastName, x => x.FirstName};

            var lastposition = await _residentService.GetAsync(selectors);

            return lastposition.Where(resident => resident.LastRecordedPosition != null);
        }

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpGet]
        public async Task<IEnumerable<ResidentLocation>> GetSince([FromQuery] int since)
        {
            return since == 0
                ? await GetAllAsync(null)
                : await _locationService.GetSinceAsync(DateTime.Now - TimeSpan.FromMinutes(since));
        }

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpGet("{id}")]
        public async Task<IEnumerable<ResidentLocation>> GetSince(string id, [FromQuery] int since)
        {
            if (!ObjectId.TryParse(id, out var objectid))
                throw new NotFoundException<Resident>(nameof(IModelWithID.Id), id);

            return since == 0
                ? await _locationService.GetSinceAsync(default(DateTime), objectid)
                : await _locationService.GetSinceAsync(DateTime.Now - TimeSpan.FromMinutes(since), objectid);
        }

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpPost("{id}/lastlocation")]
        public async Task SetLastLocation(string id, [FromBody] Point currentLocation)
        {
            currentLocation.TimeStamp = DateTime.Now;
            if (!ObjectId.TryParse(id, out var objectid))
                throw new NotFoundException<Resident>(nameof(IModelWithID.Id), id);

            await _residentService.UpdatePropertyAsync(objectid, x => x.LastRecordedPosition, currentLocation);
        }

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpPost("{tag}/lastlocation/bytag")]
        public async Task SetLastLocation(int tag, [FromBody] Point currentLocation)
        {
            currentLocation.TimeStamp = DateTime.Now;

            await _residentService.UpdatePropertyAsync(tag, x => x.LastRecordedPosition, currentLocation);
        }
    }
}