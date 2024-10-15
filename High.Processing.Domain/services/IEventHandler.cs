namespace High.Processing.Domain.Services;

public interface IEventHandler : IDisposable
{
    public void Start();

    public void Stop();
    public Task Handler<T>(Func<T, Task> handler);
}