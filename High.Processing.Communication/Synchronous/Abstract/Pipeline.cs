using System.Collections.Concurrent;
using System.Text.Json;
using NetMQ;

namespace High.Processing.Communication.Synchronous.Abstract;

public class Pipeline
{
    private readonly Dictionary<string, List<Action<string>>> _actions;
    private readonly CancellationTokenSource _cancellation;

    private readonly Thread _listener;
    private readonly INetMQSocket _socket;
    private readonly ConcurrentQueue<string> _topics;

    public Pipeline(INetMQSocket socket)
    {
        _socket = socket;
        _actions = new Dictionary<string, List<Action<string>>>();
        _topics = new ConcurrentQueue<string>();
        _listener = new Thread(TopicListener);
        _cancellation = new CancellationTokenSource();
    }

    public void Start()
    {
        _listener.Start();
    }

    public void Stop()
    {
        _cancellation.Cancel();
    }


    public Task Send<T>(T message)
    {
        var topic = typeof(T).Name;
        var json = JsonSerializer.Serialize(message);
        _socket.SendMoreFrame(topic).SendFrame(json);
        return Task.CompletedTask;
    }

    public Task Receive<T>(Action<T> callback)
    {
        var topic = typeof(T).Name;
        var transform = (string msg) =>
        {
            var message = JsonSerializer.Deserialize<T>(msg);
            if (message != null) callback(message);
        };

        if (_actions.TryGetValue(topic, out var action)) action.Add(transform);

        _actions.Add(topic, [transform]);
        _topics.Enqueue(topic);
        return Task.CompletedTask;
    }

    private void TopicListener()
    {
        while (!_cancellation.IsCancellationRequested)
        {
            var hasTopic = _topics.TryDequeue(out var topic);
            if (!hasTopic || topic == null) continue;
            var thread = new Thread(() => Listener(topic));
            thread.Start();
        }
    }

    private void Listener(string topic)
    {
        while (!_cancellation.IsCancellationRequested)
        {
            var topicReceived = _socket.ReceiveFrameString();
            if (!_actions.ContainsKey(topic)) continue;
            var msgReceived = _socket.ReceiveFrameString();
            var actions = _actions[topicReceived];
            Parallel.ForEach(actions, action => action(msgReceived));
        }
    }
}