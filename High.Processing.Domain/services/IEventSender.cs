namespace High.Processing.Domain.Services;

public interface IEventSender : IDisposable
{
    Task Send<T>(T message);
}