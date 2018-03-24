using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Models;

namespace WebService.Controllers.Bases
{
    public interface IMediaController : IRestController<MediaData>
    {
        Task<FileContentResult> GetOneAsync(string id, string extension, string token);
        Task<FileContentResult> GetOneAsync(string id);
    }
}