using FolderSync.Log;

namespace FolderSync {
    internal interface ISync {
        public void Start() { }
        public void AddLogListener(EventHandler<LogEventArgs> listener) { }
        public bool IsBusy { get; }
    }
}
