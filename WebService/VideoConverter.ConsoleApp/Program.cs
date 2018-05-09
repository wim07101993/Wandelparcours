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

            if (_programParameters != null && _programParameters.ContainsKey("help"))
                PrintHelp();

            var converter = _programParameters == null
                ? new Converter()
                : new Converter
                {
                    InputPath = _programParameters["input"],
                    OutputPath = _programParameters["output"],
                    InputExtension = _programParameters.ContainsKey("extension")
                        ? _programParameters["extension"]
                        : null
                };

            converter.Convert();
        }

        private static void ConvertParams(IReadOnlyList<string> param)
        {
            if (param == null || param.Count <= 0)
            {
                _programParameters = null;
                return;
            }

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
                case "-e":
                case "-extension":
                    return "extension";
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
                "-e\t--extension\tThe extension of the input file\n"
                + "-h\t--help\tPrint help\n"
                + "-i\t--input\tThe input file\n"
                + "-o\t--output\tThe output file");
        }
    }
}