using System;
using System.IO;

namespace VideoConverter
{
    public class Video
    {
        public Video(Stream input)
        {
            Stream = input;
        }

        public Video(string filePath)
        {
            FilePath = filePath;
            if (File.Exists(FilePath))
                Stream = File.OpenRead(FilePath);
        }


        public Stream Stream { get; }

        public string FilePath { get; private set; }

        public void WriteToFile(string filePath)
        {
            using (var newFileStream = File.Create(filePath))
            {
                Stream.CopyTo(newFileStream);
            }

            FilePath = filePath;
        }

        public void Delete()
        {
            if (!string.IsNullOrWhiteSpace(FilePath) && File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }
}