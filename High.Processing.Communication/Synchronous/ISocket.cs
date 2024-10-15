namespace High.Processing.Communication.Synchronous;

public interface ISocket
{
    public Task Send<T>(T message);

    public Task Receive<T>(Action<T> callback);
}