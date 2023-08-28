using FolderSync.Log;

namespace FolderSync {
    internal class SyncLoop {
        private bool shouldStop = false;
        private DateTime nextSyncTime = DateTime.MinValue;
        private ISync folderSync;
        private IList<ILogger> loggers;
        private readonly int intervalSeconds;
        public SyncLoop(int intervalSeconds, ISync sync) {
            this.intervalSeconds = intervalSeconds;
            this.folderSync = sync;
            this.loggers = new List<ILogger>();
        }
        public void AddLoggers(IEnumerable<ILogger> loggers) { 
            foreach (ILogger logger in loggers) {
                this.loggers.Add(logger);
                folderSync.AddLogListener(logger.Log);
            }
        }
        public void Start() {
            while (!shouldStop) {
                if (!folderSync.IsBusy && nextSyncTime < DateTime.Now) {
                    nextSyncTime = GetNextSyncTime();
                    folderSync.Start();
                }
                Thread.Sleep(500);
            }
            this.Dispose();
        }
        public void Stop() {
            shouldStop = true;
        }
        public void Dispose() {
            foreach (var logger in loggers)
                logger.Dispose();
        }
        private DateTime GetNextSyncTime() {
            if (intervalSeconds > 0)
                return DateTime.Now.AddSeconds(intervalSeconds);
            return DateTime.MaxValue;
        }
    }
}
