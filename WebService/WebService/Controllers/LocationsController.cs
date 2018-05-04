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


        [Authorize(EUserType.Nurse, EUserType.User)]
        [HttpGet(Routes.Locations.GetAllLastLocations)]
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
    }
}