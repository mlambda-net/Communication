using High.Processing.App;

namespace High.Processing.Server;

class Program
{
    static async Task Main(string[] args)
    {
        
        ThreadPool.SetMinThreads(20, 20);
        ThreadPool.SetMaxThreads(100, 100);
        
        var worker = new Worker();
        await worker.Configure();
        worker.Start();
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
        Console.WriteLine("Exiting...");
        worker.Stop();
    }
}