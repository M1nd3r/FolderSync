namespace FolderSync {
    public class FilesInfo : IPath {
        private readonly string
            hash,
            name,
            path;
        private readonly long sizeBytes;
        public FilesInfo(string path, long sizeBytes, string hash) {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.sizeBytes = (sizeBytes >= 0) ? sizeBytes : throw new ArgumentOutOfRangeException(nameof(sizeBytes));
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
