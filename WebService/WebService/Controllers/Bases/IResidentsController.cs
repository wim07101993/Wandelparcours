using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Models;

namespace WebService.Controllers.Bases
{
    /// <summary>
    /// An interface that extends from the <see cref="IRestController{T}"/> interface with as generic type parameter
    /// <see cref="Resident"/>.
    /// It is used to do the basic CURD operations for the residents.
    /// </summary>
    public interface IResidentsController : IRestController<Resident>
    {
        #region CREATE

        /// <summary>
        /// Adds a music file to the music collection of the resident in the database.
        /// </summary>
        /// <param name="residentId">id of the resident to add the music to</param>
        /// <param name="musicData">the music file to add to the residents collection</param>
        /// <returns>201 created</returns>
        Task<StatusCodeResult> AddMusicAsync(string residentId, [FromForm] MultiPartFile musicData);

        /// <summary>
        /// Adds a video file to the video collection of the resident in the database.
        /// </summary>
        /// <param name="residentId">id of the resident to add the video to</param>
        /// <param name="videoData">the video file to add to the residents collection</param>
        /// <returns>201 created</returns>
        Task<StatusCodeResult> AddVideoAsync(string residentId, [FromForm] MultiPartFile videoData);

        /// <summary>
        /// Adds an image file to the image collection of the resident in the database.
        /// </summary>
        /// <param name="residentId">id of the resident to add the image to</param>
        /// <param name="imageData">the image file to add to the residents collection</param>
        /// <returns>201 created</returns>
        Task<StatusCodeResult> AddImageAsync(string residentId, [FromForm] MultiPartFile imageData);

        /// <summary>
        /// Adds an url to the music collection of the resident in the database
        /// </summary>
        /// <param name="residentId">id of the resident to add the url to</param>
        /// <param name="url">the url to the track to add to the collection</param>
        /// <returns>201 created</returns>
        Task<StatusCodeResult> AddMusicAsync(string residentId, [FromBody] string url);

        /// <summary>
        /// Adds an url to the video collection of the resident in the database
        /// </summary>
        /// <param name="residentId">id of the resident to add the url to</param>
        /// <param name="url">the url to the video to add to the collection</param>
        /// <returns>201 created</returns>
        Task<StatusCodeResult> AddVideoAsync(string residentId, [FromBody] string url);

        /// <summary>
        /// Adds an url to the image collection of the resident in the database
        /// </summary>
        /// <param name="residentId">id of the resident to add the url to</param>
        /// <param name="url">the url to the image to add to the collection</param>
        /// <returns>201 created</returns>
        Task<StatusCodeResult> AddImageAsync(string residentId, [FromBody] string url);

        /// <summary>
        /// Adds a color to the color collection of the resident in the database
        /// </summary>
        /// <param name="residentId">id of the resident to add the color to</param>
        /// <param name="colorData">the color to add the collection</param>
        /// <returns></returns>
        Task<StatusCodeResult> AddColorAsync(string residentId, [FromBody] Color colorData);

        #endregion CREATE


        #region READ

        /// <summary>
        /// Fetches the <see cref="Resident"/> with the given tag from the database
        /// </summary>
        /// <param name="tag">the tag of the resident to fetch</param>
        /// <param name="propertiesToInclude">
        /// The properties that should be passed with the result.
        /// By default a default set of properties is returned.
        /// </param>
        /// <returns>The <see cref="Resident"/> with the asked tag</returns>
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