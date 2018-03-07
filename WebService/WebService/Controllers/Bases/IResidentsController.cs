using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Models;

namespace WebService.Controllers.Bases
{
    public interface IResidentsController : IRestController<Resident>
    {
        Task<IActionResult> GetAsync(int tag, string[] properties);

        Task<IActionResult> AddMusicAsync(string residentId, [FromBody] byte[] data);
        Task<IActionResult> AddMusicAsync(string residentId, [FromBody] string url);

        Task<IActionResult> RemoveMusicAsync(string residentId, string mediaId);

        Task<IActionResult> AddVideoAsync(string residentId, [FromBody] byte[] data);
        Task<IActionResult> AddVideoAsync(string residentId, [FromBody] string url);

        Task<IActionResult> RemoveVideoAsync(string residentId, string mediaId);

        Task<IActionResult> AddImageAsync(string residentId, [FromBody] byte[] data);
        Task<IActionResult> AddImageAsync(string residentId, [FromBody] string url);

        Task<IActionResult> RemoveImageAsync(string residentId, string mediaId);

        Task<IActionResult> AddColorAsync(string residentId, [FromBody] byte[] data);
        Task<IActionResult> AddColorAsync(string residentId, [FromBody] string url);

        Task<IActionResult> RemoveColorAsync(string residentId, string mediaId);
    }
}