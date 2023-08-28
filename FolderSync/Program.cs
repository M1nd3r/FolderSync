using FolderSync;
using FolderSync.Log;
using System.Diagnostics;
using System.Runtime.CompilerServices;

internal class Program {
    private static string? fromPath, toPath;
    private static FileStream? logStream = null;
    private static int intervalSeconds = -1;
    private const string MSG_SYNTAX = "Syntax is: from to syncIntervalSeconds logFile\n\nExample: C:\\sourceFolder\\ C:\\targetFolder\\ 300 C:\\logfile.txt\n\n syncIntervalSeconds and logFile are optional arguments.";
    private static void Main(string[] args) {
        args = new string[] { @"C:\Users\Petr\Downloads", @"D:\Windows\plant", "100", @"D:\log.txt" }; //TODO remove
        //args = new string[] { @"C:\", @"D:\Windows\plant", "300" }; //TODO remove

        var shouldExit = !HandleInput(args);
        if (shouldExit)
            return;

        ISync sync = new Sync(fromPath, toPath);

        SyncLoop syncLoop = new SyncLoop(intervalSeconds, sync);
        syncLoop.AddLoggers(GetLoggers());

        var t = StartLoopOnNewThread(syncLoop);

        Console.ReadKey(true);
        syncLoop.Stop();
        Console.WriteLine("Waiting to safely finish.");

        while (t.IsAlive) {
            Thread.Sleep(200);
        }
    }
    private static Thread StartLoopOnNewThread(SyncLoop syncLoop) {
        var t = new Thread(new ThreadStart(syncLoop.Start));
        t.Start();
        return t;
    }
    private static IEnumerable<ILogger> GetLoggers() {
        yield return ConsoleLogger.GetInstance();
        if (logStream == null)
            yield break;
        FileLogger.Initiate(logStream);
        yield return FileLogger.GetInstance();
    }
    private static bool HandleInput(string[] args) {
        if (args is null || args.Length < 2) {
            PrintMissingArguments();
            return false;
        }
        if (args.Length > 4) {
            PrintTooManyArguments();
            return false;
        }

        if (!HandleFromFolder(args[0]))
            return false;
        if (!HandleToFolder(args[1]))
            return false;

        if (args.Length == 4) {
            if (!IsInterval(args[2]))
                return false;
            AssignInterval(args[2]);
            return HandleLogFile(args[3]);
        }

        if (IsInterval(args[2])) {
            AssignInterval(args[2]);
            return true;
        }
        return HandleLogFile(args[2]);
    }
    private static bool HandleFromFolder(string fromFolderPath) {
        fromPath = fromFolderPath;
        if (!Directory.Exists(fromPath)) {
            Console.WriteLine("The source folder does not exist.");
            return false;
        }
        return true;
    }
    private static bool HandleToFolder(string toFolderPath) {
        toPath = toFolderPath;
        if (!Directory.Exists(toPath)) {
            try {
                var x = Directory.CreateDirectory(toFolderPath);
                Console.WriteLine("Replica folder was created now.");
            }
            catch (UnauthorizedAccessException) {
                Console.WriteLine("Replica folder cannot be created - Unauthorized Access.");
                return false;
            }
        }
        return true;
    }
    private static bool HandleLogFile(string logFilePath) {
        if (File.Exists(logFilePath)) {
            var info = new FileInfo(logFilePath);
            if (info.IsReadOnly) {
                Console.WriteLine("Provided log file is readonly.");
                return false;
            }
            logStream = new FileStream(logFilePath, FileMode.Append);
            return true;
        }
        try {
            logStream = File.Create(logFilePath);
            Console.WriteLine("Log file was created now.");
        }
        catch (UnauthorizedAccessException) {
            Console.WriteLine("Log file cannot be created - Unauthorized Access.");
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
