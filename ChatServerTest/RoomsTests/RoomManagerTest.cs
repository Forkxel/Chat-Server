using ChatServer.Rooms;

namespace ChatServerTest.RoomsTests;

public class RoomManagerTest
{
    [Fact]
    public void CreateRoom()
    {
        var manager = new RoomManager();
        var room = manager.GetOrCreateRoom("general");
        
        Assert.Equal("general", room.Name);
    }

    [Fact]
    public void GetRoom()
    {
        var manager = new RoomManager();
        var room1 = manager.GetOrCreateRoom("general");
        var room2 = manager.GetOrCreateRoom("general");
        
        Assert.Same(room2, room1);
    }
}