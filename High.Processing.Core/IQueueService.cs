namespace High.Processing.Core;

public interface IQueueService
{
    void Start();
    void Stop();
    Task Send<T>(T message, string topic);
    Task Receive<T>(Action<T> handler, string topic);
}