using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
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
        public LocationController(IThrow iThrow, IDataService<Location> dataService, ILogger logger) : base(iThrow, dataService, logger)
        {
        }
        [HttpPost]
        public override async Task<StatusCodeResult> CreateAsync([FromBody] Location item)
        {
            item.TimeStamp=DateTime.Now;
            return await base.CreateAsync(item);
            
        }

        public override IEnumerable<Expression<Func<Location, object>>> PropertiesToSendOnGetAll { get; }
        public override IDictionary<string, Expression<Func<Location, object>>> PropertySelectors { get; }
    }
}