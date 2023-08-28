using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSync {
    internal class FilesInfo {
        private readonly string 
            hash,
            name, 
            path;
        private readonly long sizeBytes;
        public FilesInfo(string path, long sizeBytes) {
            this.path = path;
            this.sizeBytes = sizeBytes;
            this.name = System.IO.Path.GetFileName(path);
            this.hash = "emptyHash";
        }
        public FilesInfo(string path, long sizeBytes, string hash) {
            this.path = path;
            this.sizeBytes = sizeBytes;
            this.name = System.IO.Path.GetFileName(path);
            this.hash = hash;
        }
        public string Path { get => path; }
        public string Name { get => name; }
        public long SizeBytes { get => sizeBytes; }
        public bool IsEqual(FilesInfo other) {
            if (other == null) 
                return false;
            if (other.sizeBytes != this.sizeBytes) 
                return false;
            if (other.hash != this.hash)
                return false;
            return true;
        }
    }
}
