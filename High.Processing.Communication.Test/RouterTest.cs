using High.Processing.Communication.Router;

namespace High.Processing.Communication.Test;

public class Tests
{
    private Client _client;
    private TaskCompletionSource<bool> _completion;
    private Server _server;


    [SetUp]
    public async Task Setup()
    {
        _completion = new TaskCompletionSource<bool>();
        _server = new Server(8080);
        _server.Start();
        _client = new Client("localhost", 8080);


        await _client.Subscribe("/message/hello", (string msg) =>
        {
            Console.WriteLine(msg);
            if (msg == "done") _completion.SetResult(true);
        });

        _client.Connect();
    }

    [Test]
    public async Task Test1()
    {
        await _client.Send("/message/hello", "Hi Hello I am sending messages to server");
        await _client.Send("/message/hello", "this is another message");
        await _client.Send("/message/hello", "done");
        Assert.IsTrue(_completion.Task.Result);
    }

    [TearDown]
    public void TearDown()
    {
        _client.Disconnect();
        _server.Stop();
        _client.Dispose();
        _server.Dispose();
    }
}