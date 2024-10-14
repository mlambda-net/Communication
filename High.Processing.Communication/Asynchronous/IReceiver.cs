namespace High.Processing.Communication.Asynchronous;

public interface IReceiver
{
    Task Receive<T>(Func<T, Task> handler, string topic);
}