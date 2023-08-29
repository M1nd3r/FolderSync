using System.Security.Cryptography;
using static FolderSync.Log.CommonLogEventTypes;

namespace FolderSync {
    internal class Scanner : WithLog {
        private Folder source, target;
        private IList<IPath>
            addListFiles,
            addListFolders,
            removalListFiles,
            removalListFolders;
        private bool wasScanned = false;
        public Scanner(Folder source, Folder target) {
            this.source = source;
            this.target = target;
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
                var subFolders = TryGetSubfolders(folder.Path);
                foreach (var subFolder in subFolders) {
                    folder.AddFolder(subFolder);
                    foldersToSolve.Enqueue(subFolder);
                }

                var filesPaths = Directory.GetFiles(folder.Path);
                foreach (var filePath in filesPaths) {
                    var info = new FileInfo(filePath);
                    Log(HashingFile, Path.GetFileName(filePath));
                    var hash = GetFileHash(filePath);
                    var myInfo = new FilesInfo(filePath, info.Length, hash);
                    folder.AddFile(myInfo);
                }
            }
            (removalListFiles, removalListFolders) = GetDifferenceLists(source, target);
            (addListFiles, addListFolders) = GetDifferenceLists(target, source);
            wasScanned = true;
        }
        public IList<IPath> GetList(ScanListType type) {
            if (!wasScanned)
                throw new InvalidOperationException("Cannot retrieve result list before Scan is completed.");
            switch (type) {
                case ScanListType.AddFiles:
                    return addListFiles;
                case ScanListType.AddFolders:
                    return addListFolders;
                case ScanListType.RemoveFiles:
                    return removalListFiles;
                case ScanListType.RemoveFolders:
                    return removalListFolders;
                default:
                    throw new InvalidOperationException("The type provided was not recognized.");
            }
        }
        public enum ScanListType {
            AddFiles, AddFolders, RemoveFiles, RemoveFolders
        }

        private (List<IPath>, List<IPath>) GetDifferenceLists(Folder from, Folder to) {
            var listFiles = new List<IPath>();
            var listFolders = new List<IPath>();
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
        private string GetFileHash(string path) {
            MD5 md5 = MD5.Create();
            using (var stream = new FileStream(path, FileMode.Open)) {
                return BitConverter.ToString(md5.ComputeHash(stream));
            }
        }
        private IEnumerable<Folder> TryGetSubfolders(string path) {
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
    }
}
