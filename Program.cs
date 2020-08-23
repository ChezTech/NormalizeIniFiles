using System;

namespace NormalizeIniFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            var clisArgs = new CliArgs(args);
            if (!clisArgs.IsValid)
                Console.Error.WriteLine($"Invalid arguments: {clisArgs.ErrorReason}");
        }
    }
}
