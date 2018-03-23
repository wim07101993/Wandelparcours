using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Models;

namespace WebService.Controllers.Bases
{
    public interface IMediaController : IRestController<MediaData>
    {
        Task<IActionResult> GetOneAsync(string id);
    }
}