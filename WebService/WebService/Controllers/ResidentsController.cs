using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebService.Services.Logging;
using WebService.Models;
using WebService.Services.Data;

namespace WebService.Controllers
{
    [Route("api/v1/[controller]")]
    [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
    public class ResidentsController : Controller
    {
        #region FIELDS

        private readonly IDataService _dataService;
        private readonly ILogger _logger;

        #endregion FIELDS


        #region CONSTRUCTORS

        public ResidentsController(IConfiguration config, IDataService dataService, ILogger logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        #endregion CONSTRUCTORS


        #region METHODS

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_dataService.GetResidents(new Expression<Func<Resident, object>>[]
                {
                    x => x.FirstName,
                    x => x.LastName,
                    x => x.Room,
                    x => x.Birthday,
                    x => x.Doctor,
                }));
            }
            catch (Exception e)
            {
                _logger.Log(this, ELogLevel.Error, e);
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Resident resident)
        {
            try
            {
                return _dataService.CreateResident(resident)
                    ? StatusCode((int) HttpStatusCode.Created)
                    : StatusCode((int) HttpStatusCode.NotModified);
            }
            catch (Exception e)
            {
                _logger.Log(this, ELogLevel.Error, e);
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        #endregion METHDOS
    }
}