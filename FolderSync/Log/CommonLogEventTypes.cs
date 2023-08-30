namespace FolderSync.Log {
    internal static class CommonLogEventTypes {
        internal static readonly LogEventType
            Info = new("Info"),
            FileCopied = new("File Copied"),
            FileCreated = new("File Created"),
            FileRemoved = new("File Removed"),
            FolderCopied = new("Folder Copied"),
            FolderCreated = new("Folder Created"),
            FolderRemoved = new("Folder Removed"),
            HashingFile = new("Getting hash of a file"),
            PathTooLong = new("Path too long exception"),
            SyncCompleted = new("Sync Completed"),
            SyncStarted = new("Sync Started"),
            Test = new("Test"),
            UnauthorizedAccess = new("Unauthorized Access"),
            IoException = new("IO Exception");
    }
}
