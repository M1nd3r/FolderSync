namespace FolderSync.Log {
    public class LogEventArgs : EventArgs {
        public readonly string Message;
        public readonly LogEventType Type;
        public readonly DateTime Time;
        public LogEventArgs(DateTime time, LogEventType type, string message) {
            this.Time = time;
            this.Type = type;
            this.Message = message;
        }
    }
}
