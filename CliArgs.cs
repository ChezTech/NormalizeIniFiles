using System.IO;

namespace NormalizeIniFiles
{
    class CliArgs
    {
        public string FilePath { get; private set; }
        public bool IsValid { get; private set; }
        public string ErrorReason { get; private set; }

        public CliArgs(string[] args)
        {
            IsValid = ValidateArgs(args);
        }

        private bool ValidateArgs(string[] args)
        {
            if (args.Length < 1)
            {
                ErrorReason = "Please specify an .ini file to normalize (sort).";
                return false;
            }

            var filePath = args[0];
            if (!File.Exists(filePath))
            {
                ErrorReason = $"Invalid .ini file locaton: {filePath}. Please specify a valid file path.";
                return false;
            }

            FilePath = new FileInfo(filePath).FullName;

            return true;
        }
    }
}
