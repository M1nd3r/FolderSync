namespace FolderSync.Log {
    public class LogEventType {
        private readonly string name;
        public LogEventType(string typeName) {
            this.name = typeName;
        }
        public string Name { get => name; }
    }
}
