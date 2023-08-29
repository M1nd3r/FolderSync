namespace FolderSync {
    public class Folder : IPath {
        private readonly List<Folder> folders;
        private readonly List<FilesInfo> files;
        private readonly string path;
        private readonly string name;
        public Folder(string path) {
            if(path == null) 
                throw new ArgumentNullException(nameof(path));
            folders = new List<Folder>();
            files = new List<FilesInfo>();
            this.path = path;
            this.name = System.IO.Path.GetFileName(path);
        }
        public void AddSubfolder(Folder folder) {
            if (path.Contains(folder.Path))
                throw new ArgumentException(String.Format("Folder {0} is not subfolder of {1} and cannot be added as subfolder",folder.path,this.path));
            folders.Add(folder);
        }
        public void AddFile(FilesInfo file) {
            files.Add(file);
        }
        public void AddFiles(IEnumerable<FilesInfo> files) {
            this.files.AddRange(files);
        }
        public List<Folder> Folders { get => folders; }
        public List<FilesInfo> Files { get => files; }
        public string Path { get => path; }
        public string Name { get => name; }

        public bool ContainsFile(FilesInfo file) {
            foreach (var f in files) {
                if (f.Name == file.Name && f.IsEqual(file))
                    return true;
            }
            return false;
        }
        public bool ContainsFolder(Folder folder) {
            foreach (var f in folders) {
                if (f.Name == folder.Name)
                    return true;
            }
            return false;
        }
        public Folder? GetFolder(string Name) {
            foreach (var f in folders) {
                if (f.Name == Name)
                    return f;
            }
            return null;
        }
        public static IEnumerable<Folder> GetFolders(IEnumerable<string> paths) {
            foreach (string path in paths) {
                yield return new Folder(path);
            }
        }
    }
}
