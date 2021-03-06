﻿using Microsoft.Extensions.FileSystemGlobbing;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NormalizeIniFiles
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // Note: We're not using DragonFruit (where we could specify the named args in the Main() parameter list above)
            // because we want a bit more control over the Options/Args than DragonFruit provides

            // References
            // https://www.nuget.org/packages/System.CommandLine
            // https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/march/net-parse-the-command-line-with-system-commandline
            // https://blogs.msmvps.com/bsonnino/2020/04/12/parsing-the-command-line-for-your-application-with-system-commandline/
            // https://dotnetdevaddict.co.za/2020/09/25/getting-started-with-system-commandline/
            // https://github.com/dotnet/command-line-api
            // Tab completion: https://github.com/dotnet/command-line-api/blob/main/docs/dotnet-suggest.md

            // Example args:
            // --file "C:\Users\Public\Daybreak Game Company\Installed Games\EverQuest\Aiyya_test.ini" --file "C:\Users\Public\Daybreak Game Company\Installed Games\EverQuest\UI_Aiyya_test.ini" --dryRun
            // --baseDir "C:\Users\Public\Daybreak Game Company\Installed Games\EverQuest" --pattern "*_eqclient.ini" "*_test.ini" "*_erollisi.ini"
            // --file "C:\Users\Public\Daybreak Game Company\Installed Games\EverQuest\Aiyya_test.ini" --file "C:\Users\Public\Daybreak Game Company\Installed Games\EverQuest\UI_Aiyya_test.ini" --baseDir "C:\Users\Public\Daybreak Game Company\Installed Games\EverQuest" --pattern "UI_*_test.ini" --pattern "*_eqclient.ini" "*_test.ini" "*_erollisi.ini" --dryRun

            var rootCommand = new RootCommand
            {
                new Option<IEnumerable<FileInfo>>(new[] { "--file", "-f" })
                {
                    Description = "The name of the .ini file(s) to normalize. May specify more than one.",
                    Argument = new Argument<IEnumerable<FileInfo>>().ExistingOnly(),
                },
                new Option<DirectoryInfo>( new[] {"--baseDir", "-d"})
                {
                    Description = "Base directory from which to do the glob pattern match.",
                    Argument = new Argument<DirectoryInfo>().ExistingOnly(),
                },
                new Option<IEnumerable<string>>( new[] {"--pattern", "-p"})
                {
                    Description = "The glob pattern to use in the base directory for the pattern match",
                },
                new Option<bool>( new[] {"--dryRun"})
                {
                    Description = "DryRun will just list the files to be normalized without actually doing it.",
                },
            };

            rootCommand.Description = @"Sort the keys and options (normalize) for a .ini file(s)
This is useful for consistent comparision when checking into source control.";

            rootCommand.Handler = CommandHandler.Create<IEnumerable<FileInfo>, DirectoryInfo, IEnumerable<string>, bool>((file, baseDir, pattern, dryRun) =>
            {
                return new Program().MainHandler(file, baseDir, pattern, dryRun);
            });

            // Parse the incoming args and invoke the handler
            args = FixDefaultOptions(args);
            return await rootCommand.InvokeAsync(args);
        }

        /// <summary>
        /// Add default option if arguments don't specify one.
        /// </summary>
        /// <remarks>
        /// System.CommandLine doesn't handle positional arguments (ie unnamed args)
        /// Intercept and add the default "--file" option to the front
        /// Since we're not using Commands, only Options, we can assume that all our Options begin with '-' character
        /// </remarks>
        /// <param name="args"></param>
        /// <returns></returns>
        static string[] FixDefaultOptions(string[] args)
        {
            // If we have no args, don't do anything here
            if (args.Length == 0)
                return args;

            // If the first arg starts with a hyphen, then the user is making use of named Options, so don't do anything
            if (args[0][0] == '-')
                return args;

            // Looks like the user is calling us without using a named Option, add in our default Option
            args = args.Prepend("--file").ToArray();

            return args;
        }

        private int MainHandler(IEnumerable<FileInfo> files, DirectoryInfo baseDir, IEnumerable<string> patterns, bool dryRun)
        {
            var filePaths = GetFileList(files, baseDir, patterns);

            Parallel.ForEach(filePaths, file =>
            {
                if (dryRun)
                {
                    Console.WriteLine($"File would be normalized: {file}");
                }
                else
                {
                    var normalizer = new Normalizer();
                    normalizer.NormalizeFile(file);
                    Console.WriteLine($"File has been normalized: {file}");
                }
            });

            if (filePaths.Count() == 0)
                Console.WriteLine("No files found to normalize. Use the '--help' option to see usage.");
            else
                Console.WriteLine($"Total of {filePaths.Count()} files");

            return 0;
        }

        /// <summary>
        /// Based on the CLI args, find the matching files. Return just a distinct set in case there are duplicates matches
        /// </summary>
        private IEnumerable<string> GetFileList(IEnumerable<FileInfo> files, DirectoryInfo baseDir, IEnumerable<string> patterns)
        {
            var filePaths = files?.Select(x => x.FullName) ?? Array.Empty<string>();

            if (baseDir != null && patterns != null)
            {

                var matcher = new Matcher();
                matcher.AddIncludePatterns(patterns);
                var matchedFiles = matcher.GetResultsInFullPath(baseDir.FullName);

                // Union() removes duplicates from the return set, so we don't need to 'Distinct()' the collection ourselves.
                filePaths = filePaths.Union(matchedFiles);
            }

            // We still need to sort it though
            // (still need to Distinct, since user can pass in duplicate files via the '--file' arg)
            return filePaths.Distinct().OrderBy(x => x);
        }
    }
}
