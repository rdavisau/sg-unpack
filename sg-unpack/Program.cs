using CommandLine;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Humanizer;
using static Extensions;
using static BadLoggerDontJudgeMe;

namespace SgUnpack
{
    public class Program
    {
        private const string HumanizeFormat = "#.##";

        static void Main(string[] args)
        {
            // parse arguments and validate
            var parseResult = Parser.Default.ParseArguments<CommandLineOptions>(args);
            if (parseResult.Tag == ParserResultType.NotParsed)
                return;

            var options = parseResult.MapResult(o => o, _ => new CommandLineOptions());

            var inputFiles = GetInputs(options.InputPath, options.InputMask);
            if (!inputFiles?.Any() ?? true)
            {
                Console.Error.WriteLine($"Could not locate input file/s for '{options.InputPath}', aborting.");
                return;
            }
            else
            {
                var dir = Path.GetDirectoryName(options.InputPath);
                var fileNames = inputFiles.Select(Path.GetFileName);

                LogLine($"Using inputs [ {String.Join(", ", fileNames)} ] in {dir}.");
            }
            var listOnly = String.IsNullOrWhiteSpace(options.OutputPath);
            if (listOnly)
                LogLine("--output-path not specified. Listing contents only.");
            else
            {
                var outPath = options.OutputPath;
                CreateDirectoryIfNotExists(outPath);
            }

            if (options.Filter != null)
                options.Filter = options.Filter.ToLower();

            Verbose = !options.Quiet;

            // read inputs 
            LogLine();
            LogLine($"Scanning {inputFiles.Length:N0} inputs...");
            var sw = Stopwatch.StartNew();
            var reader = new MPakReader();
            var mpaks =
                inputFiles
                    .AsParallel()
                    .AsOrdered()
                    .Select(reader.Process)
                    .ToList();

            // list and extract
            var hasFilter = !String.IsNullOrWhiteSpace(options.Filter);
            var matchChar = " *";
            var maxPackNameLen = mpaks.Max(p => p.Name.Length);
            var maxEntryNameLen = mpaks.SelectMany(p => p.Entries).Max(e => e.FileName.Length) + matchChar.Length;

            var entriesCount = mpaks.SelectMany(p => p.Entries).Count();
            var matchCount = 0;
            var extractCount = 0;
            long extractSize = 0;

            var filterText = hasFilter ? $" matching on filter '{options.Filter}'" : "";
            LogLine($"Processing {entriesCount:N0} entries{filterText}...");
            foreach (var pack in mpaks)
            {
                using (var fs = File.OpenRead(pack.FileName))
                {
                    foreach (var entry in pack.Entries)
                    {
                        var matches = hasFilter && entry.FileName.ToLower().Contains(options.Filter);
                        var matchIndicator = matches && listOnly ? matchChar : "";
                        var fn = $"{entry.FileName}{matchIndicator}";

                        if (matches)
                            matchCount += 1;

                        if (listOnly)
                        {
                            LogVerboseLine(
                                $"{pack.Name.PadRight(maxPackNameLen)}{fn.PadRight(maxEntryNameLen)}{entry.Length.Bytes().Humanize(HumanizeFormat)}");
                            continue;
                        }

                        if (hasFilter && !matches)
                            continue;

                        var outputDir = Path.Combine(options.OutputPath, Path.GetFileNameWithoutExtension(pack.FileName));
                        CreateDirectoryIfNotExists(outputDir);

                        var outputPath = Path.Combine(outputDir, entry.FileName);
                        reader.Extract(entry, outputPath, fs);
                        LogVerboseLine($"{pack.Name.PadRight(maxPackNameLen)}{fn.PadRight(maxEntryNameLen)}{entry.Length.Bytes().Humanize(HumanizeFormat)}");

                        extractCount++;
                        extractSize += entry.Length;
                    }
                }
            }

            // summarise
            LogLine();
            if (listOnly)
            {
                var matchingFilterString =
                    hasFilter
                        ? $", {matchCount:N0} matching filter. (* indicates matches)"
                        : "";

                LogLine($"{entriesCount:N0} entries found in {mpaks.Count:N0} packs{matchingFilterString}");
                LogLine("No files extracted. Use --output-path to specify an output directory.");
            }
            else
            {
                LogLine(
                    $"{extractCount:N0} files ({extractSize.Bytes().Humanize(HumanizeFormat)}) extracted to '{options.OutputPath}' in {sw.Elapsed}.");
            }
        }
    }
}