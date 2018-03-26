using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace WebService.Controllers.Bases
{
    public abstract class AControllerBase : Controller, IController
    {
        public ObjectId UserId { get; set; }
    }
}