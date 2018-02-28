using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using WebService.Services.Logging;
using MongoDB.Driver;
using WebService.Models;

namespace WebService.Controllers
{
    [Route("api/v1/[controller]")]
    [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
    public class ResidentsController : Controller
    {
        #region FIELDS

        private readonly ILogger _logger;
        private readonly IMongoCollection<Resident> _collection;

        #endregion FIELDS


        #region CONSTRUCTORS

        public ResidentsController(IConfiguration config, ILogger logger)
        {
            _logger = logger;

            _collection = new MongoClient(config["Database:ConnectionString"])
                .GetDatabase(config["Database:DatabaseName"])
                .GetCollection<Resident>(config["Database:ResidentsCollectionName"]);
        }

        #endregion CONSTRUCTORS


        #region METHODS

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var selector = Builders<Resident>
                    .Projection
                    .Include(x => x.FirstName)
                    .Include(x => x.LastName)
                    .Include(x => x.Room)
                    .Include(x => x.Birthday)
                    .Include(x => x.Doctor);

                return Ok(_collection
                    .Find(FilterDefinition<Resident>.Empty)
                    .Project<Resident>(selector)
                    .ToList());
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
                resident.ID = ObjectId.GenerateNewId();
                _collection.InsertOne(resident);
                return StatusCode((int) HttpStatusCode.Created);
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