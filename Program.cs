using System;

namespace NormalizeIniFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(args.Length);
            Console.WriteLine(args[0]);
            Console.WriteLine(args[1]);
        }

        private bool ValidateArgs(string[] args)
        {
            return false;
        }
    }
}
