using FolderSync;
using FolderSync.Log;

internal class Program {
    private static void Main(string[] args) {
        var shouldExit = !InputHandler.HandleInput(args);
        if (shouldExit)
            return;
        StartSyncLoop(verboseScanner: true);
    }
    private static void StartSyncLoop(bool verboseScanner = true) {
        var syncLoop = GetSyncLoop(verboseScanner);
        var thread = StartLoopOnNewThread(syncLoop);

        Console.ReadKey(true);
        StopSyncLoopOnThread(syncLoop, thread);
    }
    private static SyncLoop GetSyncLoop(bool verboseScanner) {
        ISync sync = new Sync(InputHandler.FromPath, InputHandler.ToPath, verboseScanner);
        var syncLoop = new SyncLoop(InputHandler.IntervalSeconds, sync);
        syncLoop.AddLoggers(CommonLoggers.GetLoggers());
        return syncLoop;
    }
    private static Thread StartLoopOnNewThread(SyncLoop syncLoop) {
        var t = new Thread(new ThreadStart(syncLoop.Start));
        t.Start();
        return t;
    }
    private static void StopSyncLoopOnThread(SyncLoop syncLoop, Thread thread) {
        syncLoop.Stop();
        Console.WriteLine("Waiting to safely finish.");
        while (thread.IsAlive)
            Thread.Sleep(200);
    }

}
