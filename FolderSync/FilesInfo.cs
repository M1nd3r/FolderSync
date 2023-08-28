using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSync {
    internal class FilesInfo {
        private readonly string filename;
        private readonly long sizeBytes;
        public FilesInfo(string filename, long sizeBytes) {
            this.filename = filename;
            this.sizeBytes = sizeBytes;
        }
        public string Name { get => filename; }
        public long SizeBytes { get => sizeBytes; }
    }
}
