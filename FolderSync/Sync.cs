using FolderSync.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
            folderCopied = new LogEventType("Folder Copied"),
            folderCreated = new LogEventType("Folder Created"),
            folderRemoved = new LogEventType("Folder Removed"),
            hashingFile = new LogEventType("Getting hash of a file"),
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

                var filesPaths = Directory.GetFiles(folder.Path);
                foreach (var filePath in filesPaths) {
                    var info = new FileInfo(filePath);
                    Log(hashingFile, Path.GetFileName(filePath));
                    var hash = GetFileHash(filePath);
                    var myInfo = new FilesInfo(filePath, info.Length, hash);
                    folder.AddFile(myInfo);
                }
            }

            Log(test, "Scanning completed. Start of comparing.");
            var (removalListFiles, removalListFolders) = GetDifferenceLists(fromFolder, toFolder);
            var (addListFiles, addListFolders) = GetDifferenceLists(toFolder, fromFolder);

            RemoveFolders(removalListFolders);
            RemoveFiles(removalListFiles);
            CopyFolders(addListFolders, fromFolder, toFolder);
            CopyFiles(addListFiles, fromFolder, toFolder);

            Log(syncCompleted);
            isBusy = false;
        }
        public void AddLogListener(EventHandler<LogEventArgs> listener) {
            log += listener;
        }
        public bool IsBusy { get => isBusy; }

        private void RemoveFolders(IList<Folder> removeListFolders) {
            foreach (var folder in removeListFolders) {
                new DirectoryInfo(folder.Path).Delete(true);
                Log(folderRemoved, String.Format("Previously replicated folder {0} and all its contents were deleted.", folder.Path));
            }
        }
        private void RemoveFiles(IList<FilesInfo> removeListFiles) {
            foreach (var file in removeListFiles) {
                File.Delete(file.Path);
                Log(fileRemoved, String.Format("Previously replicated file {0} was deleted.", file.Path));
            }
        }
        private void CopyFolders(IList<Folder> addListFolders, Folder fromFolder, Folder toFolder) {
            foreach (var folder in addListFolders) {
                var newPath = GetNewPath(folder.Path, fromFolder, toFolder);
                CopyDirectory(folder.Path, newPath);
                Log(folderCopied, String.Format("Folder {0} is newly replicated to {1} with all its contents.", folder.Path, newPath));
            }
        }
        private void CopyFiles(IList<FilesInfo> addListFiles, Folder fromFolder, Folder toFolder) {
            foreach (var file in addListFiles) {
                var newPath = GetNewPath(file.Path, fromFolder, toFolder);
                File.Copy(file.Path, newPath);
                Log(fileCopied, String.Format("File {0} is newly replicated to {1}.", file.Path, newPath));
            }
        }
        private string GetNewPath(string oldPath, Folder fromFolder, Folder toFolder) {
            return toFolder.Path + (oldPath.Remove(0, fromFolder.Path.Length));
        }
        private (List<FilesInfo>, List<Folder>) GetDifferenceLists(Folder from, Folder to) {
            var listFiles = new List<FilesInfo>();
            var listFolders = new List<Folder>();
            var compareQueue = new Queue<(Folder, Folder)>();
            compareQueue.Enqueue((from, to));
            while (compareQueue.Count > 0) {
                var (f, t) = compareQueue.Dequeue();
                foreach (var file in t.Files) {
                    if (!f.ContainsFile(file))
                        listFiles.Add(file);
                }
                foreach (var folder in t.Folders) {
                    if (!f.ContainsFolder(folder))
                        listFolders.Add(folder);
                }
            }
            return (listFiles, listFolders);
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
        private string GetFileHash(string path) {
            MD5 md5 = MD5.Create();
            using (var stream = new FileStream(path, FileMode.Open)) {
                return BitConverter.ToString(md5.ComputeHash(stream));
            }
        }
        private void CopyDirectory(string source, string target) {

            var dir = new DirectoryInfo(source);
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(target);

            foreach (FileInfo file in dir.GetFiles()) {
                string targetFilePath = Path.Combine(target, file.Name);
                file.CopyTo(targetFilePath);
            }

            foreach (DirectoryInfo subDir in dirs) {
                string newDestinationDir = Path.Combine(target, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir);
            }
        }
    }
}
