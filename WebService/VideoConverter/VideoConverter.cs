using System;
using System.Diagnostics;
using System.IO;

namespace VideoConverter
{
    public class VideoConverter : IVideoConverter
    {
        public const string FilesDirectory = "files_to_convert";


        public Video ConvertToWebm(Video input, string outputPath = null)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
                return Convert(input, "webm");

            var newFilePath = outputPath.EndsWith(".webm")
                ? outputPath
                : $"{outputPath}.webm";
            return ConvertToFile(input, newFilePath);
        }

        public Video ConvertToFile(Video input, string outputPath)
        {
            GiveVideoPathIfItHasNone(input);
            Convert(input.FilePath, outputPath);
            input.Delete();

            return new Video(outputPath);
        }

        public Video Convert(Video input, string extension)
        {
            CreateFilesDirectoryIfNotExists();
            var newFilePath = $"{FilesDirectory}/{GenerateFileName()}.{extension}";

            GiveVideoPathIfItHasNone(input);
            Convert(input.FilePath, newFilePath);
            input.Delete();
            
            var data = File.ReadAllBytes(newFilePath);
            var stream = new MemoryStream(data);
            File.Delete(newFilePath);

            return new Video(stream);
        }

        public bool CheckDependencies()
        {
            try
            {
                Convert("", "");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private static void GiveVideoPathIfItHasNone(Video video)
        {
            CreateFilesDirectoryIfNotExists();

            var fileName = GenerateFileName();
            var path = $"{FilesDirectory}/{fileName}";

            video.WriteToFile(path);
        }

        private static void Convert(string source, string destination)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-i \"{source}\" \"{destination}\""
                }
            };
            process.Start();
            process.WaitForExit();
        }

        private static string GenerateFileName()
        {
            var now = DateTime.Now;
            return $"{now.Year}.{now.Month}.{now.Day}.{now.Hour}.{now.Minute}.{now.Second}.{now.Millisecond}";
        }

        private static void CreateFilesDirectoryIfNotExists()
        {
            if (!Directory.Exists(FilesDirectory))
                Directory.CreateDirectory(FilesDirectory);
        }
    }
}