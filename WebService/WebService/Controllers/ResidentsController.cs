﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebService.Controllers.Bases;
using WebService.Helpers;
using WebService.Services.Logging;
using WebService.Models;
using WebService.Models.Bases;
using WebService.Services.Data;

namespace WebService.Controllers
{
    /// <inheritdoc cref="Controller"/>
    /// <summary>
    /// ResidentsController is a controller for the REST service.
    /// <para />
    /// It handles the reading and writing of residents data to the database.
    /// </summary>
    [Route("api/v1/[controller]")]
    [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
    public class ResidentsController : ARestControllerBase<Resident>
    {
        #region CONSTRUCTOR

        /// <inheritdoc cref="ARestControllerBase{T}" />
        /// <summary>
        /// Residentscontroller creates an instance of the <see cref="T:WebService.Controllers.ResidentsController" /> class. 
        /// </summary>
        /// <param name="dataService">is a service to handle the database connection</param>
        /// <param name="logger">is a service to hanlde the logging of messages</param>
        public ResidentsController(IDataService<Resident> dataService, ILogger logger)
            : base(dataService, logger)
        {
        }

        #endregion CONSTRUCTOR


        #region METHODS

        /// <summary>
        /// SmallDataProperties is a collection of expressions to select the properties that
        /// consume the least space (FirstName, LastName, Room Birthday and Doctor).
        /// </summary>
        public override Expression<Func<Resident, object>>[] PropertiesToSendOnGet { get; } =
        {
            // specify the fields that need to be returned
            x => x.FirstName,
            x => x.LastName,
            x => x.Room,
            x => x.Birthday,
            x => x.Doctor,
        };


        public override IEnumerable<Expression<Func<Resident, object>>> ConvertStringsToSelectors(
            IEnumerable<string> strings)
        {
            return strings.ConvertToResidentPropertySelectors();
        }

        [HttpGet]
        public override async Task<IActionResult> GetAsync()
            => await base.GetAsync();

        [HttpPost]
        public override async Task<IActionResult> CreateAsync([FromBody] Resident item)
            => await base.CreateAsync(item);

        [HttpDelete("{id}")]
        public override async Task<IActionResult> DeleteAsync(string id)
            => await base.DeleteAsync(id);

        [HttpPut]
        public override async Task<IActionResult> UpdateAsync([FromBody] AUpdater<Resident> updater)
            => await base.UpdateAsync(updater);

        #endregion METHODS
    }
}