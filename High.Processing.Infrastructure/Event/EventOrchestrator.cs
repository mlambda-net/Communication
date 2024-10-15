using High.Processing.Communication.Asynchronous.PubSub;
using High.Processing.Domain.Services;

namespace High.Processing.Infrastructure.Event;

public class EventOrchestrator : IEventHandler
{
    private readonly Subscriber _subscriber;

    public EventOrchestrator(string host = "localhost", int port = 9090)
    {
        _subscriber = new Subscriber("localhost", 9090);
    }

    public void Start()
    {
        _subscriber.Start();
    }

    public async Task Handler<T>(Func<T, Task> handler)
    {
        await _subscriber.Receive(handler, typeof(T).Name);
    }


    public void Dispose()
    {
        _subscriber.Dispose();
    }

    public void Stop()
    {
        _subscriber.Stop();
    }
}