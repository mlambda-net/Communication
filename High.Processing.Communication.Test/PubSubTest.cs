using System.Reflection.Metadata;
using High.Processing.Communication.Asynchronous;
using High.Processing.Communication.Asynchronous.Abstract;
using High.Processing.Communication.Asynchronous.PubSub;

namespace High.Processing.Communication.Test;

public class PubSubTest
{
    private Publisher sender;
    private Receiver receiver;
    private TaskCompletionSource<bool> _completion;

    [SetUp]
    public async Task Setup()
    {
        _completion = new TaskCompletionSource<bool>();
        sender = new Publisher(7000);
        receiver = new Subscriber("localhost", 7000);
        await receiver.Receive<string>(GetMessages, "");
        receiver.Start();
    }

    private Task GetMessages(string msg)
    {
        Console.WriteLine(msg);
        _completion.SetResult(true);
        return Task.CompletedTask;
    }


    [Test]
    public async Task Messaging()
    {
        await sender.Send<string>("hello", "");
        await sender.Send<string>("hello", "");
        await sender.Send<string>("hello", "");
        await sender.Send<string>("hello", "");
        await sender.Send<string>("hello", "");
        await sender.Send<string>("hello", "");
        
        Assert.IsTrue(_completion.Task.Result);

    }

    [TearDown]
    public void TearDown()
    {

        sender.Dispose();
        receiver.Stop();
        receiver.Dispose();
    }

}