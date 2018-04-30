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
    [Route(Routes.RestBase.ControllerRoute)]
    [Route(Routes.Locations.RouteOld)] //TODO
    [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
    public class LocationsController : ARestControllerBase<ResidentLocation>, ILocationController
    {
        private readonly IResidentsService _residentService;
        private readonly ILocationsService _locationsService;


        public LocationsController(ILocationsService dataService, ILogger logger, IResidentsService residentService,
            IUsersService usersService, ILocationsService locationsService)
            : base(dataService, logger, usersService)
        {
            _residentService = residentService;
            _locationsService = locationsService;
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
        [Obsolete] //TODO
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
        [HttpGet(Routes.Locations.GetlastLocationOneResidentByTag)]
        [Obsolete] //TODO
        public LocalRedirectResult GetLastLocationOneResidentByTag(int tag)
        {
            return LocalRedirect($"~/api/v1/residents/{tag}/LastRecordedPosition");
//            var selectors = new Expression<Func<Resident, object>>[]
//                {x => x.LastRecordedPosition, x => x.Id, x => x.LastName, x => x.FirstName};
//           
//            return await _residentService.GetOneAsync(tag, selectors);
        }

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpGet(Routes.Locations.GetAllLastLocations)]
        [HttpGet(Routes.Locations.GetAllLastLocationsOld)] //TODO
        public async Task<IEnumerable<Resident>> GetLastLocation()
        {
            var selectors = new Expression<Func<Resident, object>>[]
                {x => x.LastRecordedPosition, x => x.Id, x => x.LastName, x => x.FirstName};

            var lastposition = await _residentService.GetAsync(selectors);

            return lastposition.Where(resident => resident.LastRecordedPosition != null);
        }

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpGet(Routes.RestBase.GetAll)]
        public async Task<IEnumerable<ResidentLocation>> GetSince([FromQuery] int since)
        {
            return since == 0
                ? await GetAllAsync(null)
                : await _locationsService.GetSinceAsync(DateTime.Now - TimeSpan.FromMinutes(since));
        }

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpGet(Routes.RestBase.GetOne)]
        public async Task<IEnumerable<ResidentLocation>> GetSince(string id, [FromQuery] int since)
        {
            if (!ObjectId.TryParse(id, out var objectid))
                throw new NotFoundException<Resident>(nameof(IModelWithID.Id), id);

            return since == 0
                ? await _locationsService.GetSinceAsync(default(DateTime), objectid)
                : await _locationsService.GetSinceAsync(DateTime.Now - TimeSpan.FromMinutes(since), objectid);
        }

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpPost(Routes.Locations.SetLastLocation)]
        [Obsolete] //TODO
        public async Task SetLastLocation(string id, [FromBody] Point currentLocation)
        {
            currentLocation.TimeStamp = DateTime.Now;
            if (!ObjectId.TryParse(id, out var objectid))
                throw new NotFoundException<Resident>(nameof(IModelWithID.Id), id);

            await _residentService.UpdatePropertyAsync(objectid, x => x.LastRecordedPosition, currentLocation);
        }

        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpPost(Routes.Locations.SetlastLocationByTag)]
        [Obsolete] //TODO
        public async Task SetLastLocation(int tag, [FromBody] Point currentLocation)
        {
            currentLocation.TimeStamp = DateTime.Now;

            await _residentService.UpdatePropertyAsync(tag, x => x.LastRecordedPosition, currentLocation);
        }
    }
}