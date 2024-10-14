namespace High.Processing.Communication.Asynchronous;

public interface ISender
{
    Task Send<T>(T message, string topic);
}