using ChatServer;
using ChatServer.Rooms;

namespace ChatServerTest;

public class RoomTest
{
    [Fact]
    public void AddMember_Test()
    {
        var room = new Room("general");
        var mockClient = new ClientHandlerMock();
        
        room.AddMember(mockClient);
        
        Assert.Contains(mockClient, room.GetMembers());
    }

    [Fact]
    public void RemoveMember_Test()
    {
        var room = new Room("general");
        var mockClient = new ClientHandlerMock();
        
        room.AddMember(mockClient);
        room.RemoveMember(mockClient);
        
        Assert.DoesNotContain(mockClient, room.GetMembers());
    }
}

public class ClientHandlerMock : ClientHandler
{
    public ClientHandlerMock() : base(null, null)
    {
        
    }
}