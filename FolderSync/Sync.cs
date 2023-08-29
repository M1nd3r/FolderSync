using FolderSync.Log;
using static FolderSync.Log.CommonLogEventTypes;

namespace FolderSync {
    internal class Sync : WithLog, ISync {
        private bool isBusy = false;
        private Folder from, to;
        public Sync(string? from, string? to, bool verboseScanner = false) {
            if (from == null)
                throw new ArgumentNullException(nameof(from));
            if (to == null)
                throw new ArgumentNullException(nameof(to));

            this.from = new Folder(from);
            this.to = new Folder(to);
            this.VerboseScanner = verboseScanner;
        }
        public void Start() {
            isBusy = true;
            Log(SyncStarted);

            Scanner scanner = GetScanner(from, to);

            scanner.Scan();
            Log(Info, "Scanning completed. Start of copy/removal of files and folders.");

            var removeFiles = scanner.GetList(Scanner.ScanListType.RemoveFiles);
            var removeFolders = scanner.GetList(Scanner.ScanListType.RemoveFolders);
            var addFiles = scanner.GetList(Scanner.ScanListType.AddFiles);
            var addFolders = scanner.GetList(Scanner.ScanListType.AddFolders);

            RemoveFolders(removeFolders);
            RemoveFiles(removeFiles);
            CopyFolders(addFolders, from, to);
            CopyFiles(addFiles, from, to);

            isBusy = false;
            Log(SyncCompleted);
        }
        public bool IsBusy { get => isBusy; }

        public bool VerboseScanner { get; set; }
        private Scanner GetScanner(Folder source, Folder target) {
            var scanner = new Scanner(source, target);
            if (VerboseScanner)
                SetScannerVerbose(scanner);
            return scanner;
        }
        private void SetScannerVerbose(Scanner scanner) {
            var loggers = CommonLoggers.GetLoggers();
            foreach (ILogger logger in loggers) {
                scanner.AddLogListener(logger.Log);
            }
        }
        private string GetNewPath(string oldPath, IPath fromFolder, IPath toFolder) {
            return toFolder.Path + (oldPath.Remove(0, fromFolder.Path.Length));
        }
        private void RemoveFolders(IList<IPath> removeListFolders) {
            foreach (var folder in removeListFolders) {
                new DirectoryInfo(folder.Path).Delete(true);
                Log(FolderRemoved, String.Format("Previously replicated folder {0} and all its contents were deleted.", folder.Path));
            }
        }
        private void RemoveFiles(IList<IPath> removeListFiles) {
            foreach (var file in removeListFiles) {
                File.Delete(file.Path);
                Log(FileRemoved, String.Format("Previously replicated file {0} was deleted.", file.Path));
            }
        }
        private void CopyFiles(IList<IPath> addListFiles, IPath fromFolder, IPath toFolder) {
            foreach (var file in addListFiles) {
                var newPath = GetNewPath(file.Path, fromFolder, toFolder);
                File.Copy(file.Path, newPath);
                Log(FileCopied, String.Format("File {0} is replicated to {1}.", file.Path, newPath));
            }
        }
        private void CopyFolders(IList<IPath> addListFolders, IPath fromFolder, IPath toFolder) {
            foreach (var folder in addListFolders) {
                var newPath = GetNewPath(folder.Path, fromFolder, toFolder);
                CopyFolder(folder.Path, newPath);
                Log(FolderCopied, String.Format("Folder {0} is replicated to {1} with all its contents.", folder.Path, newPath));
            }
        }

        private void CopyFolder(string sourcePath, string targetPath) {

            var dir = new DirectoryInfo(sourcePath);
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(targetPath);
            Log(FolderCreated, String.Format("Folder {0} is created.", targetPath));
            Log(FolderCopied, String.Format("Folder {0} is being replicated to {1} with all its contents.", sourcePath, targetPath));
            foreach (FileInfo file in dir.GetFiles()) {
                string targetFilePath = Path.Combine(targetPath, file.Name);
                file.CopyTo(targetFilePath);
                Log(FileCopied, String.Format("File {0} is copied.", targetFilePath));
            }

            foreach (DirectoryInfo subDir in dirs) {
                string newDestinationDir = Path.Combine(targetPath, subDir.Name);
                CopyFolder(subDir.FullName, newDestinationDir);
            }
        }
    }
}
