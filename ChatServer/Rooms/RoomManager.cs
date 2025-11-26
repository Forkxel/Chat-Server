using System.Collections.Concurrent;

namespace ChatServer.Rooms;

public class RoomManager
{
    private ConcurrentDictionary<string, Room> rooms = new();

    public Room GetOrCreateRoom(string roomName)
    {
        return rooms.GetOrAdd(roomName, n => new Room(roomName));
    }

    public IEnumerable<string> GetAllRooms()
    {
        return rooms.Keys;
    }
}