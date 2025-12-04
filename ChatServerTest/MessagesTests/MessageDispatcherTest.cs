using ChatServer.Messages;

namespace ChatServerTest.MessagesTests;

public class MessageDispatcherTest
{
    [Fact]
    public void SendMessageInRoom()
    {
        var room = "general";

        var client1 = new MessageClientHandler();
        var client2 = new MessageClientHandler();
        
        var dispatcher = new MessageDispatcher(() => new[] { client1, client2 });
        
        dispatcher.Enqueue(new Message("Hello World", room, client1));
        Thread.Sleep(200);

        Assert.Contains("Hello world", client1.Received);
        Assert.Contains("Hello world", client2.Received);
    }
}