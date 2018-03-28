using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using MongoDB.Bson;
using WebService.Controllers.Bases;
using WebService.Models;
using WebService.Services.Data;
using WebService.Services.Exceptions;
using WebService.Services.Logging;

namespace WebService.Controllers
{
    /// <inheritdoc cref="ARestControllerBase{T}"/>
    /// <summary>
    /// LocationController handles the reading and writing of locations for residents to the database.
    /// </summary>
    [Route("api/v1/[controller]")]
    [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
    public class LocationController : ARestControllerBase<Location>, ILocationController

    {
        private readonly IResidentsService _residentService;

        public LocationController(IThrow iThrow, IDataService<Location> dataService, ILogger logger, IResidentsService residentService) : base(iThrow, dataService, logger)
        {
            _residentService = residentService;
        }
        [HttpPost]
        public override async Task<StatusCodeResult> CreateAsync([FromBody] Location item)
        {
            item.TimeStamp=DateTime.Now;
            return await base.CreateAsync(item);
            
        }

        [HttpGet("{id}/lastlocation")]
        public async Task<Resident> GetLastLocation(string id)
        {
            var selectors = new Expression<Func<Resident, object>>[] { x=> x.LastRecordedPosition, x=> x.Id,x=> x.LastName,x=> x.FirstName};
            if (ObjectId.TryParse(id, out var objectid))
            {
                var lastposition = await this._residentService.GetAsync(objectid, selectors);
                return lastposition;
            }

            
            return null;
        }
        
        
        
        [HttpPost("{id}/lastlocation")]
        public async Task<Resident> SetLastLocation(string id, [FromBody]Point currentLocation)
        {
            currentLocation.TimeStamp=DateTime.Now;
            if (ObjectId.TryParse(id, out var objectid))
            {
                
                await this._residentService.UpdatePropertyAsync(objectid, x => x.LastRecordedPosition, currentLocation);
                
            }

            
            return null;
        }
        
        


        public override IEnumerable<Expression<Func<Location, object>>> PropertiesToSendOnGetAll { get; }
        public override IDictionary<string, Expression<Func<Location, object>>> PropertySelectors { get; }
    }
}