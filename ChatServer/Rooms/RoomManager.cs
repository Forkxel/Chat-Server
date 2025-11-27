using System.Collections.Concurrent;

namespace ChatServer.Rooms;

/// <summary>
/// Manages chat rooms, creating them as needed.
/// </summary>
public class RoomManager
{
    private Dictionary<string, Room> rooms = new();
    private object roomsLock = new();

    /// <summary>
    /// Returns an existing room or creates a new one if it doesn't exist.
    /// </summary>
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
}