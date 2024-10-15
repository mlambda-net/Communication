using System.Text;
using System.Text.Json;
using NetMQ;

namespace High.Processing.Communication.Asynchronous.Abstract;

public class Orchestrator : IDisposable
{
    private readonly List<Func<string, Task>> _actions = new();
    private readonly CancellationTokenSource _cancellation;
    private readonly Thread _listener;
    private readonly INetMQSocket _socket;

    public Orchestrator(INetMQSocket socket)
    {
        _socket = socket;
        _listener = new Thread(Listen);
        _cancellation = new CancellationTokenSource();
    }

    public void Dispose()
    {
        _socket?.Dispose();
        _cancellation.Dispose();
    }

    public void Register<T>(Func<T, Task> handler)
    {
        var transform = async (string msg) =>
        {
            var message = JsonSerializer.Deserialize<T>(msg);
            if (message != null) await handler(message);
        };
        _actions.Add(transform);
    }

    public void Process(string msgReceived)
    {
        Parallel.ForEach(_actions, action => action(msgReceived));
    }

    private void Listen()
    {
        while (!_cancellation.IsCancellationRequested)
            try
            {
                var topic = _socket.ReceiveFrameString();
                var messageBytes = _socket.ReceiveFrameBytes();
                var messageReceived = Encoding.UTF8.GetString(messageBytes);
                Parallel.ForEach(_actions, action => action(messageReceived));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
    }

    public void Stop()
    {
        _cancellation.Cancel();
    }

    public void Start()
    {
        _listener.Start();
    }
}