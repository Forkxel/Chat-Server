using ChatServer.Messages;

namespace ChatServerTest.MessagesTests;

public class MessageTest
{
    [Fact]
    public void MessagePropertiesTest()
    {
        var sender = new ClientHandlerMock();
        var msg = new Message("ahoj", "general", sender);
        
        Assert.Equal("ahoj", msg.Text);
        Assert.Same(sender, sender);
        Assert.Equal("general", msg.Room);
    }
}