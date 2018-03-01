using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebService.Services.Logging;
using WebService.Models;
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
    public class ResidentsController : Controller
    {
        #region FIELDS

        /// <summary>
        /// _dataService is used to handle the data traffice to and from the database.
        /// </summary>
        private readonly IDataService _dataService;

        /// <summary>
        /// _logger is used to handle the logging of messages.
        /// </summary>
        private readonly ILogger _logger;

        #endregion FIELDS


        #region CONSTRUCTORS

        /// <summary>
        /// Residentscontroller creates an instance of the <see cref="ResidentsController"/> class. 
        /// </summary>
        /// <param name="dataService">is a service to handle the database connection</param>
        /// <param name="logger">is a service to hanlde the logging of messages</param>
        public ResidentsController(IDataService dataService, ILogger logger)
        {
            // initiate the services
            _dataService = dataService;
            _logger = logger;
        }

        #endregion CONSTRUCTORS


        #region METHODS

        /// <summary>
        /// Get is the method corresponding to the GET method of the /Residents controller of the REST service. (GET 0.0.0.0:3000/api/v1/residents)
        /// <para/>
        /// It returns all the Residents in the database wrapped in an <see cref="IActionResult"/>. To limit the data traffic only a few fields are returned:
        /// <list type="bullet">
        /// <item><description>FirstName</description></item>
        /// <item><description>Lastname</description></item>
        /// <item><description>Room</description></item>
        /// <item><description>BirthDay</description></item>
        /// <item><description>Doctor</description></item>
        /// </list>
        /// </summary>
        /// <returns>
        /// - Status ok with An IEnumerable of all the Residents in the database on success
        /// - Status internal server error when an error occures
        /// </returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                // return the values that come from the data service wrapped in a 200 response 
                return Ok(_dataService.GetResidents(new Expression<Func<Resident, object>>[]
                {
                    // specify the fields that need to be returned
                    x => x.FirstName,
                    x => x.LastName,
                    x => x.Room,
                    x => x.Birthday,
                    x => x.Doctor,
                }));
            }
            catch (Exception e)
            {
                // log the exception
                _logger.Log(this, ELogLevel.Error, e);
                // return a 500 error to the client
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Create is the method corresonding to the POST method of the /Residents controller of the REST service. (POST 0.0.0.0:3000/api/v1/residents)
        /// <para/>
        /// It saves the passed <see cref="Resident"/> to the database.
        /// </summary>
        /// <param name="resident">is the <see cref="Resident"/> to save in the database</param>
        /// <returns>
        /// - Status created (201) if succes
        /// - Status not modified (304) if there was no error but the <see cref="Resident"/> has not been added
        /// - Status internal server error (500) on error
        /// </returns>
        [HttpPost]
        public IActionResult Create([FromBody] Resident resident)
        {
            try
            {
                // use the data service to create a new resident
                return _dataService.CreateResident(resident) != null
                    // if the resident was created return satus created
                    ? StatusCode((int) HttpStatusCode.Created)
                    // if the resident was not created return status not modified
                    : StatusCode((int) HttpStatusCode.NotModified);
            }
            catch (Exception e)
            {
                // log the error
                _logger.Log(this, ELogLevel.Error, e);
                // return a 500 internal server error code
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        #endregion METHDOS
    }
}