namespace High.Processing.Communication.Synchronous;

public interface IServer : ISocket
{
    void Start();

    void Stop();
}