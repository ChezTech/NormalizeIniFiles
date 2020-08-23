using System;

namespace NormalizeIniFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            var cliArgs = new CliArgs(args);
            if (!cliArgs.IsValid)
                Console.Error.WriteLine($"Invalid arguments: {cliArgs.ErrorReason}");

            var normalizer = new Normalizer();
            normalizer.NormalizeFile(cliArgs.FilePath);
            Console.WriteLine($"File has been normalized: ${cliArgs.FilePath}");
        }
    }
}
