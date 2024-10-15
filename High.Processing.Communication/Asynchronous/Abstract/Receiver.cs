using NetMQ;

namespace High.Processing.Communication.Asynchronous.Abstract;

public abstract class Receiver : IReceiver, IDisposable
{
    private readonly Dictionary<string, Orchestrator> _orchestrators = new();

    public void Dispose()
    {
        foreach (var orchestrator in _orchestrators.Values) orchestrator.Dispose();
    }

    public Task Receive<T>(Func<T, Task> handler, string topic)
    {
        if (!_orchestrators.ContainsKey(topic))
        {
            var orchestrator = new Orchestrator(CreateSocket(topic));
            _orchestrators.Add(topic, orchestrator);
        }

        _orchestrators[topic].Register(handler);

        return Task.CompletedTask;
    }


    protected abstract INetMQSocket CreateSocket(string topic);

    public void Start()
    {
        foreach (var orchestrator in _orchestrators.Values) orchestrator.Start();
    }

    public void Stop()
    {
        foreach (var orchestrator in _orchestrators.Values) orchestrator.Stop();
    }
}