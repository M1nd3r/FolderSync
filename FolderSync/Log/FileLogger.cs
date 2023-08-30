namespace FolderSync.Log {
    internal class FileLogger : ILogger {
        private static FileLogger? instance;
        private readonly StreamWriter sw;
        private FileLogger(FileStream fs) {
            sw = new StreamWriter(fs);
        }
        public void Log(object? sender, LogEventArgs e) {
            if (e.Message.Length < 1)
                sw.WriteLine("{0} {1}", e.Time.ToString(), e.Type.Name);
            else
                sw.WriteLine("{0} {1}: {2}", e.Time.ToString(), e.Type.Name, e.Message);
        }
        public static void Initiate(FileStream fileStream) {
            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));
            if (instance != null)
                throw new InvalidOperationException("FileLogger has been already initialized.");
            instance = new FileLogger(fileStream);
        }
        public static FileLogger GetInstance() {
            if (instance == null)
                throw new InvalidOperationException("FileLogger has not been initialized, yet.");
            return instance;
        }
        public void Dispose() {
            sw.Flush();
            sw.Dispose();
        }
    }
}
