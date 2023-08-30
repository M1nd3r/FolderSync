using FolderSync.Log;

namespace FolderSync {
    public abstract class WithLog {
        private event EventHandler<LogEventArgs>? LogHandler;
        public void AddLogListener(EventHandler<LogEventArgs> listener) {
            LogHandler += listener ?? throw new ArgumentNullException(nameof(listener));
        }
        protected void Log(LogEventType type, string message) {
            LogHandler?.Invoke(this, new LogEventArgs(DateTime.Now, type, message));
        }
        protected void Log(LogEventType type) {
            Log(type, "");
        }
    }
}
