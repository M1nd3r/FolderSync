using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSync {
    internal class Folder {
        private List<Folder> folders;
        private List<FilesInfo> files;
        private readonly string path;
        private readonly string name;
        public Folder(string path) {
            folders = new List<Folder>();
            files = new List<FilesInfo>();
            this.path = path;
            this.name = System.IO.Path.GetFileName(path);
        }
        public void AddFolder(Folder folder) {
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
        public string Name { get => path; }

        public bool ContainsFile(FilesInfo file) {
            foreach (var f in files) {
                if (f.Name == file.Name && f.IsEqual(file))
                    return true;
            }
            return false;
        }
        public bool ContainsFolder(Folder folder) {
            foreach (var f in folders) { 
                if(f.Name==folder.Name) 
                    return true;
            }
            return false;
        }
        public static IEnumerable<Folder> GetFolders(IEnumerable<string> paths) {
            foreach (string path in paths) {
                yield return new Folder(path);
            }
        }
    }
}
