using FolderSync.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSync {
    internal class Sync : ISync {
        private bool isBusy = false;
        private event EventHandler<LogEventArgs>? log;
        private string from, to;
        private readonly LogEventType
            test = new LogEventType("Test"),
            fileCopied = new LogEventType("File Copied"),
            fileCreated = new LogEventType("File Created"),
            fileRemoved = new LogEventType("File Removed"),
            folderCreated = new LogEventType("Folder Created"),
            folderRemoved = new LogEventType("Folder Removed"),
            syncCompleted = new LogEventType("Sync Completed"),
            syncStarted = new LogEventType("Sync Started"),
            unauthorizedAccess = new LogEventType("Unauthorized Access"),
            ioException = new LogEventType("IO Exception");


        private Queue<Folder> foldersToSolve;

        public Sync(string? from, string? to) {
            if (from == null)
                throw new ArgumentNullException(nameof(from));
            if (to == null)
                throw new ArgumentNullException(nameof(to));

            this.from = from;
            this.to = to;
            this.foldersToSolve = new Queue<Folder>();
        }
        public void Start() {
            isBusy = true;
            Log(syncStarted);
            Folder
                toFolder = new Folder(to),
                fromFolder = new Folder(from);

            foldersToSolve.Enqueue(toFolder);
            foldersToSolve.Enqueue(fromFolder);

            while (foldersToSolve.Count > 0) {
                var folder = foldersToSolve.Dequeue();
                var subFolders = TryGetSubfolders(folder.Path);
                foreach (var subFolder in subFolders) {
                    folder.AddFolder(subFolder);
                    foldersToSolve.Enqueue(subFolder);
                }

                var fileNames = Directory.GetFiles(folder.Path);
                foreach (var fileName in fileNames) {
                    var info = new FileInfo(fileName);
                    var myInfo = new FilesInfo(fileName, info.Length);
                    folder.AddFile(myInfo);
                }
            }
            var x = test;

            Log(test, "This is a test message.");
            Log(syncCompleted);
            isBusy = false;
        }
        private IEnumerable<Folder> TryGetSubfolders(string path) {
            try {
                return Folder.GetFolders(Directory.GetDirectories(path));
            }
            catch (UnauthorizedAccessException) {
                Log(unauthorizedAccess, String.Format("The folder {0} cannot be scanned or copied!", path));
            }
            catch (IOException e) {
                Log(ioException, String.Format("The folder {0} cannot be scanned or copied! The following error occured: {1}", path, e.Message));
            }
            return new List<Folder>();

        }
        private void Log(LogEventType type, string message) {
            log?.Invoke(this, new LogEventArgs(DateTime.Now, type, message));
        }
        private void Log(LogEventType type) {
            Log(type, "");
        }
        public void AddLogListener(EventHandler<LogEventArgs> listener) {
            log += listener;
        }
        public bool IsBusy { get => isBusy; }
    }
}
