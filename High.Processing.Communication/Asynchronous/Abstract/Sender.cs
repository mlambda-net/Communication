using System.Collections.Concurrent;
using System.Text.Json;
using NetMQ;

namespace High.Processing.Communication.Asynchronous.Abstract;

public record Data(string Topic, string Message);

public abstract class Sender : IDisposable
{
    private readonly ConcurrentQueue<Data> _messages = new();
    private readonly Thread _pushing;

    public Sender()
    {
        _pushing = new Thread(Push);
        _pushing.Start();
    }

    protected abstract INetMQSocket Socket { get; }

    public void Dispose()
    {
        Socket.Dispose();
    }


    public Task Send<T>(T message, string topic)
    {
        return Task.Run(() =>
        {
            try
            {
                var json = JsonSerializer.Serialize(message);
                _messages.Enqueue(new Data(topic, json));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        });
    }

    private void Push()
    {
        while (true)
            if (_messages.TryDequeue(out var data))
            {
                Socket.SendMoreFrame(data.Topic);
                Socket.SendFrame(data.Message);
            }
    }
}