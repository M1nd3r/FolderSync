using System.IO;
using System.Security;
using System.Security.Cryptography;
using static FolderSync.Log.CommonLogEventTypes;

namespace FolderSync {
    internal class Scanner : WithLog {
        private readonly Folder source, target;
        private IList<IPath>
            addListFiles,
            addListFolders,
            removalListFiles,
            removalListFolders;
        private bool wasScanned = false;
        public Scanner(Folder source, Folder target) {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.target = target ?? throw new ArgumentNullException(nameof(target));
            addListFiles = new List<IPath>();
            addListFolders = new List<IPath>();
            removalListFiles = new List<IPath>();
            removalListFolders = new List<IPath>();
        }
        public void Scan() {
            var foldersToSolve = new Queue<Folder>();
            foldersToSolve.Enqueue(source);
            foldersToSolve.Enqueue(target);

            while (foldersToSolve.Count > 0) {
                var folder = foldersToSolve.Dequeue();
                ProcessSubfolders(folder, foldersToSolve);
                ProcessFilesInFolder(folder);
            }
            (removalListFiles, removalListFolders) = GetDifferenceLists(source, target);
            (addListFiles, addListFolders) = GetDifferenceLists(target, source);
            wasScanned = true;
        }
        private void ProcessSubfolders(Folder folder, Queue<Folder> foldersToSolve) {
            var subFolders = GetSubfoldersSafely(folder.Path);
            foreach (var subFolder in subFolders) {
                folder.AddFolder(subFolder);
                foldersToSolve.Enqueue(subFolder);
            }
        }
        private void ProcessFilesInFolder(Folder folder) {
            if (!TryGetFilesPaths(folder.Path, out var filePaths))
                return;
            var filesPaths = Directory.GetFiles(folder.Path);
            foreach (var filePath in filesPaths)
                ScanAndAddFileToFolder(filePath, folder);
        }
        private void ScanAndAddFileToFolder(string filePath, Folder folder) {
            if (!TryGetFileInfo(filePath, out var info))
                return;
            Log(HashingFile, Path.GetFileName(filePath));
            var hash = GetFileHash(filePath);
            var myInfo = new FilesInfo(filePath, info!.Length, hash);
            folder.AddFile(myInfo);
        }

        public IList<IPath> GetList(ScanListType type) {
            if (!wasScanned)
                throw new InvalidOperationException("Cannot retrieve result list before Scan is completed.");
            return type switch {
                ScanListType.AddFiles => addListFiles,
                ScanListType.AddFolders => addListFolders,
                ScanListType.RemoveFiles => removalListFiles,
                ScanListType.RemoveFolders => removalListFolders,
                _ => throw new InvalidOperationException("The type provided was not recognized."),
            };
        }
        public enum ScanListType {
            AddFiles, AddFolders, RemoveFiles, RemoveFolders
        }

        private static (List<IPath> listFiles, List<IPath> listFolders) GetDifferenceLists(Folder from, Folder to) {
            var listFiles = new List<IPath>();
            var listFolders = new List<IPath>();
            var compareQueue = new Queue<(Folder, Folder)>();
            compareQueue.Enqueue((from, to));
            CompareFoldersAndFiles(ref compareQueue, ref listFiles, ref listFolders);
            return (listFiles, listFolders);
        }
        private static void CompareFoldersAndFiles(ref Queue<(Folder, Folder)> compareQueue, ref List<IPath> listFiles, ref List<IPath> listFolders) {
            while (compareQueue.Count > 0) {
                var (source, target) = compareQueue.Dequeue();
                CompareFiles(source, target, ref listFiles);
                CompareFolders(source, target, ref listFolders, ref compareQueue);
            }
        }
        private static void CompareFiles(Folder source, Folder target, ref List<IPath> listFiles) {
            foreach (var file in target.Files) {
                if (!source.ContainsFile(file))
                    listFiles.Add(file);
            }
        }
        private static void CompareFolders(Folder source, Folder target, ref List<IPath> listFolders, ref Queue<(Folder, Folder)> compareQueue) {
            foreach (var folder in target.Folders) {
                if (!source.ContainsFolder(folder))
                    listFolders.Add(folder);
                else {
                    var sourcePairFolder = source.GetFolder(folder.Name);
                    if (sourcePairFolder != null)
                        compareQueue.Enqueue((sourcePairFolder, folder));
                }
            }
        }
        private static string GetFileHash(string path) {
            MD5 md5 = MD5.Create();
            using var stream = new FileStream(path, FileMode.Open);
            return BitConverter.ToString(md5.ComputeHash(stream));
        }
        private IEnumerable<Folder> GetSubfoldersSafely(string path) {
            try {
                return Folder.GetFolders(Directory.GetDirectories(path));
            }
            catch (UnauthorizedAccessException) {
                Log(UnauthorizedAccess, String.Format("The folder {0} cannot be scanned or copied!", path));
            }
            catch (IOException e) {
                Log(IoException, String.Format("The folder {0} cannot be scanned or copied! The following error occured: {1}", path, e.Message));
            }
            return new List<Folder>();
        }
        private bool TryGetFileInfo(string filePath, out FileInfo? info) {
            info = null;
            try {
                info = new FileInfo(filePath);
                return true;
            }
            catch (Exception e)
                when (
                e is UnauthorizedAccessException ||
                e is SecurityException ||
                e is NotSupportedException) {
                Log(UnauthorizedAccess, String.Format("The folder {0} cannot be scanned or copied!", filePath));
                return false;
            }
            catch (ArgumentNullException) {
                Log(Info, "A folder path cannot be retrieved.");
                return false;
            }
            catch (PathTooLongException) {
                Log(PathTooLong, String.Format("The folder {0} cannot be scanned or copied, because the path is too long.", filePath));
                return false;
            }
        }
        private bool TryGetFilesPaths(string folderPath, out string[]? filesPaths) {
            filesPaths = null;
            try {
                filesPaths = Directory.GetFiles(folderPath);
                return true;
            }
            catch (Exception e) when (
                e is UnauthorizedAccessException ||
                e is IOException ||
                e is NotSupportedException) {
                Log(UnauthorizedAccess, String.Format("The files in folder {0} cannot be scanned!", folderPath));
                return false;
            }
            catch (Exception e) when (
                e is DirectoryNotFoundException ||
                e is ArgumentNullException) {
                Log(Info, "A folder path cannot be retrieved.");
                return false;
            }
            catch (PathTooLongException) {
                Log(PathTooLong, String.Format("The files in {0} cannot be scanned, because there is a too long path.", folderPath));
                return false;
            }
        }
    }
}
