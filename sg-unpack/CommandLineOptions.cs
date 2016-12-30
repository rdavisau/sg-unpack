using CommandLine;

namespace SgUnpack
{
    public class CommandLineOptions
    {
        [Option('i', "input-path", Required = true,
            HelpText = "Path to input/s. Can be an individual file, or a directory.")]
        public string InputPath { get; set; }

        [Option('m', "input-mask", Required = false, Default = "*.mpk",
            HelpText = "Search mask for --input-path, used if --input-path is a directory.")]
        public string InputMask { get; set; }

        [Option('o', "output-path", Required = false,
            HelpText = "Output path for extracted files. If omitted, contents will be listed but not extracted.")]
        public string OutputPath { get; set; }

        [Option('f', "output-filter", Required = false, HelpText = "Case-insensitive filter for files to extract.")]
        public string Filter { get; set; }

        [Option('q', "quiet", Default = (false), HelpText = "Set true to hide most output.")]
        public bool Quiet { get; set; }
    }
}
