using System;
using System.Diagnostics;
using System.IO;

namespace VideoConverter
{
    public class VideoConverter : IVideoConverter
    {
        private const string FilesDirectory = "files_to_convert";


        public Stream ConvertToWebm(Stream input, string extension)
        {
            if (extension == "webm")
                return input;

            if (!Directory.Exists(FilesDirectory))
                Directory.CreateDirectory(FilesDirectory);

            var now = DateTime.Now;
            var fileName = $"{now.Year}.{now.Month}.{now.Day}.{now.Hour}.{now.Minute}.{now.Second}.{now.Millisecond}";
            var oldFilePath = $"{FilesDirectory}/{fileName}.{extension}";
            var newFilePath = $"{FilesDirectory}/{fileName}.webm";

            var oldFileStream = File.Create(oldFilePath);
            using (input)
            using (oldFileStream)
            {
                input.CopyTo(oldFileStream);
            }

            var vlcProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-i \"{oldFilePath}\" \"{newFilePath}\""
                }
            };

            vlcProcess.Start();
            vlcProcess.WaitForExit();

            File.Delete(oldFilePath);
            
            return File.OpenRead(newFilePath);
        }
    }
}