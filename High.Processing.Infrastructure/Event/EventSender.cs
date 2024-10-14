
using System.Collections.Concurrent;
using High.Processing.Communication.Asynchronous.PubSub;
using High.Processing.Domain.Services;

namespace High.Processing.Infrastructure.Event;

public class EventSender(int port = 9090) : IEventSender
{
    
    
    
    
    private readonly Publisher _sender = new(port);

    public async Task Send<T>(T message)
    {
        await _sender.Send(message, typeof(T).Name);
    }

    public void Dispose()
    {
        _sender.Dispose();
    }
}