using ChatServer.Rooms;

namespace ChatServerTest.RoomsTests;

/// <summary>
/// Test class for testing Room
/// </summary>
public class RoomTest
{
    /// <summary>
    /// Method testing adding user to the room
    /// </summary>
    [Fact]
    public void AddMember_Test()
    {
        var room = new Room("general");
        var mockClient = new ClientHandlerMock();
        
        room.AddMember(mockClient);
        
        Assert.Contains(mockClient, room.GetMembers());
    }

    /// <summary>
    /// Method testing removing of user from room
    /// </summary>
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