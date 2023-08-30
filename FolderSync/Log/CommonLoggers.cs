namespace FolderSync.Log {
    internal static class CommonLoggers {
        private static bool wasInitiated = false;
        private static IEnumerable<ILogger> loggers = new List<ILogger>();
        private static readonly object lockObj = new();
        internal static void Initiate(FileStream? logStream) {
            lock (lockObj) {
                if (wasInitiated)
                    return;
                loggers = SetLoggerss(logStream).ToList();
                wasInitiated = true;
            }
        }
        internal static IEnumerable<ILogger> GetLoggers() {
            if (!wasInitiated)
                Initiate(null);
            return CommonLoggers.loggers;
        }

        private static ILogger GetConsoleLogger() {
            return ConsoleLogger.GetInstance();
        }
        private static ILogger GetFileLogger(FileStream logStream) {
            if (logStream == null)
                throw new ArgumentNullException(nameof(logStream));
            FileLogger.Initiate(logStream);
            return FileLogger.GetInstance();
        }
        private static IEnumerable<ILogger> SetLoggerss(FileStream? logStream) {
            yield return GetConsoleLogger();
            if (logStream == null)
                yield break;
            yield return GetFileLogger(logStream);
        }
    }
}
