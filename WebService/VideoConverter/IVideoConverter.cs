namespace VideoConverter
{
    public interface IVideoConverter
    {
        /// <summary>
        /// Converts the input to webm. If the outputpath is null, the video is not writen to storage.
        /// </summary>
        /// <param name="input">the video to convert</param>
        /// <param name="outputPath">the path where the output for the conversion should be stored</param>
        /// <returns>the converted video</returns>
        Video ConvertToWebm(Video input, string outputPath = null);
        
        /// <summary>
        /// Converts the input to the video extension given in <see cref="outputPath"/>
        /// </summary>
        /// <param name="input">the video to convert</param>
        /// <param name="outputPath">the path where the output for the conversion should be stored</param>
        /// <returns>the converted video</returns>
        Video ConvertToFile(Video input, string outputPath);
        
        /// <summary>
        /// Converts the input to the given extension
        /// </summary>
        /// <param name="input">the video to convert</param>
        /// <param name="extension">the new extension for the video</param>
        /// <returns>the converted video</returns>
        Video Convert(Video input, string extension);
    }
}