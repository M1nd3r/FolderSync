using FolderSync;
using FolderSync.Log;

internal class Program {
    private static void Main(string[] args) {
        args = new string[] { @"C:\Users\Petr\Downloads", @"D:\Windows\plant", "100", @"D:\log.txt" }; //TODO remove

        var shouldExit = !InputHandler.HandleInput(args);
        if (shouldExit)
            return;

        ISync sync = new Sync(InputHandler.FromPath, InputHandler.ToPath, verboseScanner:true);
        
        var syncLoop = new SyncLoop(InputHandler.IntervalSeconds, sync);
        syncLoop.AddLoggers(CommonLoggers.GetLoggers());

        var t = StartLoopOnNewThread(syncLoop);

        Console.ReadKey(true);
        syncLoop.Stop();
        Console.WriteLine("Waiting to safely finish.");

        while (t.IsAlive) {
            Thread.Sleep(200);
        }
    }

    private static Thread StartLoopOnNewThread(SyncLoop syncLoop) {
        var t = new Thread(new ThreadStart(syncLoop.Start));
        t.Start();
        return t;
    }
}
