namespace FolderSync.Log {
    internal interface ILogger : IDisposable {
        public void Log(object? sender, LogEventArgs e);
    }
}
