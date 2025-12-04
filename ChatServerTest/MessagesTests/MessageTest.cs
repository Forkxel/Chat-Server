using ChatServer.Messages;

namespace ChatServerTest.MessagesTests;

/// <summary>
/// Test class testing Message
/// </summary>
public class MessageTest
{
    /// <summary>
    /// Method for testing setting properties of Message
    /// </summary>
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