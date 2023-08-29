using FolderSync.Log;

namespace FolderSync {
    internal abstract class WithLog {
        protected event EventHandler<LogEventArgs>? log;
        public void AddLogListener(EventHandler<LogEventArgs> listener) {
            log += listener;
        }
        protected void Log(LogEventType type, string message) {
            log?.Invoke(this, new LogEventArgs(DateTime.Now, type, message));
        }
        protected void Log(LogEventType type) {
            Log(type, "");
        }
    }
}
