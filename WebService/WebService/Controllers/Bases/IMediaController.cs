using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers.Bases
{
    public interface IMediaController
    {
        Task<FileContentResult> GetAsync(string id, string extension);
        Task<FileContentResult> GetAsync(string id);
    }
}
