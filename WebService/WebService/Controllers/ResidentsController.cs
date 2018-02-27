using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    public class ResidentsController : Controller
    {
        public IActionResult Index()
        {
            return Ok();
        }
    }
}