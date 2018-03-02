using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebService.Models;

namespace WebService.Controllers
{
    [Route("api/v1/[controller]")]
    public class ReceiverModuleController :Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                // return the values that come from the data service wrapped in a 200 response
                var stations=new List<ReceiverModule>();
                var station0=  new ReceiverModule{ Mac = "test0", Position = new Point{X=0,Y=0}};
                var station1=  new ReceiverModule{ Mac = "test1", Position = new Point{X=0.5,Y=0.5}};
                var station2=  new ReceiverModule{ Mac = "test2", Position = new Point{X=1,Y=1}};
                stations.Add(station0);
                stations.Add(station1);
                stations.Add(station2);
                
                return Ok(stations);
            }
            catch (Exception e)
            {
                // return a 500 error to the client
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }
        [HttpPost]
        public IActionResult Create([FromBody] ReceiverModule station)
        {
            try
            {
                return StatusCode((int) HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                // return a 500 error to the client
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }
    }
}