using FolderSync.Log;

namespace FolderSync {
    internal abstract class WithLog {
        private event EventHandler<LogEventArgs>? LogHandler;
        public void AddLogListener(EventHandler<LogEventArgs> listener) {
            LogHandler += listener;
        }
        protected void Log(LogEventType type, string message) {
            LogHandler?.Invoke(this, new LogEventArgs(DateTime.Now, type, message));
        }
        protected void Log(LogEventType type) {
            Log(type, "");
        }
    }
}
