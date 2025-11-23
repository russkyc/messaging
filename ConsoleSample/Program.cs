// See https://aka.ms/new-console-template for more information

using Russkyc.Messaging;
using Russkyc.Messaging.Messages;

var subscriber = new object();

WeakReferenceMessenger.Default.Register<TestMessage>(subscriber, OnTestMessageReceived);

void OnTestMessageReceived(object recipient, TestMessage message)
{
    Console.WriteLine($"Received message: {message.Message}");
    message.Reply($"Hi {message.Message}");
}

var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

_ = Task.Run(async () =>
{
    while (await timer.WaitForNextTickAsync())
    {
        var reply = WeakReferenceMessenger.Default.Send(new TestMessage($"Hello {DateTime.Now:T}"));
        var response = await reply.Response;
        Console.WriteLine($"Reply: {response}");
    }
});

Console.WriteLine("Start Messaging");
Console.ReadLine();

class TestMessage : AsyncRequestMessage<string>
{
    public TestMessage(string message)
    {
        Message = message;
    }

    public string Message { get; set; }
}