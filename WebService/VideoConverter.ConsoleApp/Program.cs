using System;
using System.Collections.Generic;

namespace VideoConverter.ConsoleApp
{
    internal static class Program
    {
        private static IDictionary<string, string> _programParameters;

        private static void Main(string[] param)
        {
            ConvertParams(param);

//            using (var video = new VideoConverter()
//                .ConvertToWebm(new Video("/home/wim/Downloads/mp4/small.mp4")))
//            {
//                video.WriteToFile("/home/wim/Downloads/mp4/small.webm");
//            }
//
//            Environment.Exit(0);

            ConvertVideo();
        }

        private static void ConvertVideo()
        {
            if (_programParameters == null)
            {
                Console.WriteLine("Please enter an input and output");
                return;
            }

            if (_programParameters.ContainsKey("help"))
                PrintHelp();

            if (!_programParameters.ContainsKey("input"))
                Console.WriteLine("Please specify an input");
            else if (!_programParameters.ContainsKey("output"))
                Console.WriteLine("Please specify an output. For help type -h or --help");
            else
                new VideoConverter()
                    .ConvertToWebm(new Video(_programParameters["input"]), _programParameters["output"])
                    .Dispose();
        }

        private static void ConvertParams(IReadOnlyList<string> param)
        {
            if (param == null || param.Count <= 0)
            {
                _programParameters = null;
                return;
            }
            
            _programParameters = new Dictionary<string, string>();
            
            for (var i = 0; i < param.Count; i++)
            {
                if (param[i][0] != '-' || param[i].Length < 2)
                    continue;

                var parameter = ConvertParameter(param[i]);
                var value = param.Count > i && param[i + 1][0] != '-'
                    ? param[i + 1]
                    : null;

                _programParameters.Add(parameter, value);
            }
        }

        private static string ConvertParameter(string param)
        {
            switch (param)
            {
                case "-h":
                case "-help":
                    return "help";
                case "-i":
                case "--input":
                    return "input";
                case "-o":
                case "--output":
                    return "output";
                default:
                    Console.WriteLine($"Unknown parameter: {param}.");
                    Console.WriteLine($"Use -h or --help for help");
                    Environment.Exit(0);
                    // ReSharper disable once HeuristicUnreachableCode
                    return null;
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine(
                "-h\t--help\tPrint help\n"
                + "-i\t--input\tThe input file\n"
                + "-o\t--output\tThe output file");
        }
    }
}