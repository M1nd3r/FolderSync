using FolderSync.Log;

namespace FolderSync {
    public static class InputHandler {
        private const string MSG_SYNTAX = "Syntax is: \"From\" \"To\" \"SyncIntervalSeconds\" \"logFile\"\n\nExample: C:\\sourceFolder\\ C:\\targetFolder\\ 300 C:\\logfile.txt\n\nSyncIntervalSeconds and logFile are optional arguments. It is possible to add one or both. If both optional arguments are entered, the order must be preserved.";
        private static string? fromPath, toPath;
        private static int intervalSeconds = -1;
        public static int IntervalSeconds { get => intervalSeconds; }
        public static string? FromPath { get => fromPath; }
        public static string? ToPath { get => toPath; }
        public static bool HandleInput(string[] args) {
            if (!VerifyArgumentsCount(args))
                return false;

            if (!HandleMandatoryArguments(args))
                return false;

            if (args.Length == 2)
                return true;

            if (args.Length == 3)
                return HandleThreeArguments(args);

            return HandleFourArguments(args);
        }
        private static bool VerifyArgumentsCount(string[] args) {
            if (args is null || args.Length < 2) {
                PrintMissingArguments();
                return false;
            }
            if (args.Length > 4) {
                PrintTooManyArguments();
                return false;
            }
            return true;
        }
        private static bool HandleMandatoryArguments(string[] args) {
            if (!HandleFromFolder(args[0]))
                return false;
            return HandleToFolder(args[1]);
        }
        private static bool HandleFourArguments(string[] args) {
            if (!IsInterval(args[2]))
                return false;
            AssignInterval(args[2]);
            return HandleLogFile(args[3]);
        }
        private static bool HandleThreeArguments(string[] args) {
            if (IsInterval(args[2])) {
                AssignInterval(args[2]);
                return true;
            }
            return HandleLogFile(args[2]);
        }
        private static bool HandleFromFolder(string sourceFolderPath) {
            fromPath = sourceFolderPath;
            if (!Directory.Exists(fromPath)) {
                Console.WriteLine("The source folder does not exist.");
                return false;
            }
            return true;
        }
        private static bool HandleToFolder(string targetFolderPath) {
            toPath = targetFolderPath;
            if (!Directory.Exists(toPath))
                return TryCreateFolder(toPath);
            return true;
        }
        private static bool HandleLogFile(string logFilePath) {
            if (File.Exists(logFilePath))
                return HandleExistingLogFile(logFilePath);
            return HandleNewLogFile(logFilePath);
        }
        private static bool HandleExistingLogFile(string logFilePath) {
            var info = new FileInfo(logFilePath);
            if (info.IsReadOnly) {
                Console.WriteLine("Provided log file is readonly.");
                return false;
            }
            CommonLoggers.Initiate(new FileStream(logFilePath, FileMode.Append));
            return true;
        }
        private static bool HandleNewLogFile(string logFilePath) {
            try {
                CommonLoggers.Initiate(File.Create(logFilePath));
                Console.WriteLine("Log file was created now.");
            }
            catch (UnauthorizedAccessException) {
                Console.WriteLine("Log file cannot be created - Unauthorized Access.");
                return false;
            }
            catch (ArgumentException e) {
                Console.WriteLine("Log file path is invalid: {0}", e.Message);
                return false;
            }
            return true;
        }
        private static bool TryCreateFolder(string path) {
            try {
                var x = Directory.CreateDirectory(path);
                Console.WriteLine("Replica folder was created now.");
            }
            catch (UnauthorizedAccessException) {
                Console.WriteLine("Replica folder cannot be created - Unauthorized Access.");
                return false;
            }
            return true;
        }
        private static void AssignInterval(string interval)
            => intervalSeconds = int.Parse(interval);
        private static bool IsInterval(string interval)
            => int.TryParse(interval, out _);
        private static void PrintMissingArguments()
            => Console.WriteLine("Missing mandatory argument(s)\n" + MSG_SYNTAX);
        private static void PrintTooManyArguments()
            => Console.WriteLine("Too many arguments\n" + MSG_SYNTAX);
    }
}
