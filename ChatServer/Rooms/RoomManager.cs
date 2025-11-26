using System.Collections.Concurrent;

namespace ChatServer.Rooms;

public class RoomManager
{
    private Dictionary<string, Room> rooms = new();
    private object roomsLock = new();

    public Room GetOrCreateRoom(string roomName)
    {
        lock (roomsLock)
        {
            if (!rooms.TryGetValue(roomName, out Room room))
            {
                rooms[roomName] = room = new Room(roomName);
            }
            return room;
        }
    }
    /*
    public Room GetRoom(string roomName)
    {
        lock (roomsLock)
        {
            return rooms[roomName];
        }
    }
    */
}