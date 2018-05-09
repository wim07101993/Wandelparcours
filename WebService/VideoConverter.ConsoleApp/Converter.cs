using System;
using System.IO;

namespace VideoConverter.ConsoleApp
{
    public class Converter
    {
        private readonly VideoConverter _videoConverter = new VideoConverter();
        private string _inputExtension;
        private string _inputPath;
        private string _outputPath;


        public string InputPath
        {
            get
            {
                if (string.IsNullOrEmpty(_inputPath))
                {
                    Console.WriteLine("What is the input file?");
                    _inputPath = Console.ReadLine()?.Trim();
                }

                return _inputPath;
            }
            set => _inputPath = value;
        }

        public string InputExtension
        {
            get
            {
                if (string.IsNullOrEmpty(_inputExtension))
                {
                    Console.WriteLine("What is the file extension? (default: the extension in the filepath)");
                    var extension = Console.ReadLine()?.Trim();
                    _inputExtension = string.IsNullOrWhiteSpace(extension)
                        ? null
                        : extension;
                }

                return _inputExtension ?? InputPath?.Substring(InputPath.LastIndexOf('.'));
            }
            set => _inputExtension = value;
        }

        public string OutputPath
        {
            get
            {
                if (string.IsNullOrEmpty(_outputPath))
                {
                    Console.WriteLine(
                        "Where do we need to place the output? (default: same as old file but new extension)");
                    var outputPath = Console.ReadLine()?.Trim();
                    _outputPath = string.IsNullOrWhiteSpace(outputPath)
                        ? InputPath.Substring(0, InputPath.LastIndexOf('.'))
                        : outputPath;
                }

                return _outputPath;
            }
            set => _outputPath = value;
        }

        private FileStream InputStream
        {
            get
            {
                if (!File.Exists(InputPath))
                    Console.WriteLine("Inputfile not found");
                else
                {
                    try
                    {
                        return File.OpenRead(InputPath);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine("Failed to open input file:");
                        Console.WriteLine(e.Message);
                    }
                }

                Environment.Exit(-1);
                // ReSharper disable once HeuristicUnreachableCode
                return null;
            }
        }

        private FileStream OutputStream
        {
            get
            {
                if (!File.Exists(OutputPath) || AskForFileReplaceMent())
                {
                    try
                    {
                        return File.Create(OutputPath);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine("Failed to create output file:");
                        Console.WriteLine(e.Message);
                    }
                }

                Environment.Exit(-1);
                // ReSharper disable once HeuristicUnreachableCode
                return null;
            }
        }


        public void Convert()
        {
            var input = InputStream;
            var output = OutputStream;
            
            if (string.IsNullOrWhiteSpace(InputExtension))
            {
                Console.WriteLine("No extension found");
                return;
            }

            var convertedVideo = _videoConverter.ConvertToMp4(input, InputExtension);

            using (convertedVideo)
            using (output)
            {
                convertedVideo.Seek(0, SeekOrigin.Begin);
                convertedVideo.CopyTo(output);
            }
        }

        private static bool AskForFileReplaceMent()
        {
            Console.WriteLine("Outputfile already exists, do you want to replace it? (Y/n)");
            while (true)
            {
                var k = Console.ReadKey();
                switch (k.Key)
                {
                    case ConsoleKey.Y:
                    case ConsoleKey.J:
                        return true;
                    case ConsoleKey.N:
                        return false;
                }
            }
        }
    }
}