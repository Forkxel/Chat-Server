using ChatServer.Rooms;

namespace ChatServerTest.RoomsTests;

/// <summary>
/// Test class for testing RoomManager
/// </summary>
public class RoomManagerTest
{
    /// <summary>
    /// Testing method for creating room
    /// </summary>
    [Fact]
    public void CreateRoom()
    {
        var manager = new RoomManager();
        var room = manager.GetOrCreateRoom("general");
        
        Assert.Equal("general", room.Name);
    }

    /// <summary>
    /// Testing method for getting room
    /// </summary>
    [Fact]
    public void GetRoom()
    {
        var manager = new RoomManager();
        var room1 = manager.GetOrCreateRoom("general");
        var room2 = manager.GetOrCreateRoom("general");
        
        Assert.Same(room2, room1);
    }
}