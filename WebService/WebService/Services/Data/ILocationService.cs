using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Models;

namespace WebService.Services.Data
{
    public interface ILocationService : IDataService<Location>
    {
        //Task<IEnumerable<Location>> GetLocationsAfterDate(DateTime dt, ObjectId residentId);
    }
}