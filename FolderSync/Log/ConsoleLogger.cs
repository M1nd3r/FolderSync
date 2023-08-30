namespace FolderSync.Log {
    internal class ConsoleLogger : ILogger {
        private static ConsoleLogger? instance;
        private ConsoleLogger() { }
        public void Log(object? sender, LogEventArgs e) {
            if (e.Message.Length < 1)
                Console.WriteLine("{0} {1}", e.Time.ToString(), e.Type.Name);
            else
                Console.WriteLine("{0} {1}: {2}", e.Time.ToString(), e.Type.Name, e.Message);
        }
        public static ConsoleLogger GetInstance() {
            instance ??= new ConsoleLogger();
            return instance;
        }
        public void Dispose() { }
    }
}
