using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Models;

namespace WebService.Controllers.Bases
{
    public interface IResidentsController : IRestController<Resident>
    {
        #region CREATE

        Task<StatusCodeResult> AddMusicAsync(string residentId, [FromForm] MultiPartFile musicData);

        Task<StatusCodeResult> AddVideoAsync(string residentId, [FromForm] MultiPartFile videoData);

        Task<StatusCodeResult> AddImageAsync(string residentId, [FromForm] MultiPartFile imageData);


        Task<StatusCodeResult> AddMusicAsync(string residentId, [FromBody] string url);

        Task<StatusCodeResult> AddVideoAsync(string residentId, [FromBody] string url);

        Task<StatusCodeResult> AddImageAsync(string residentId, [FromBody] string url);


        Task<StatusCodeResult> AddColorAsync(string residentId, [FromBody] Color colorData);

        #endregion CREATE


        #region READ

        Task<Resident> GetByTagAsync(int tag, string[] propertiesToInclude);

        Task<object> GetRandomElementFromPropertyAsync(int tag, string mediaType);

        Task<object> GetPropertyAsync(int tag, string propertyName);

        #endregion READ


        #region UPDATE

        Task UpdatePictureAsync(string id, MultiPartFile picture);

        Task UpdateLastRecordedLocation(int tag, ResidentLocation location);
        
        #endregion UPDATE


        #region DELETE

        Task RemoveMusicAsync(string residentId, string musicId);

        Task RemoveVideoAsync(string residentId, string videoId);

        Task RemoveImageAsync(string residentId, string imageId);

        Task RemoveColorAsync(string residentId, Color color);

        #endregion DELETE
    }
}