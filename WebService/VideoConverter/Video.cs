using System;
using System.IO;

namespace VideoConverter
{
    public class Video : IDisposable
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

        public string FilePath { get; }

        public bool DeleteOnDispose { get; set; }

        public void Dispose()
        {
            Stream.Close();
            Stream.Dispose();

            if (DeleteOnDispose && FilePath != null && File.Exists(FilePath))
                File.Delete(FilePath);
        }

        public void WriteToFile(string filePath)
        {
            using (var newFileStream = File.Create(filePath))
            {
                Stream.CopyTo(newFileStream);
            }
        }
    }
}