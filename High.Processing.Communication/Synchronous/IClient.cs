namespace High.Processing.Communication.Synchronous;

public interface IClient : ISocket
{
    void Connect();

    void Disconnect();
}