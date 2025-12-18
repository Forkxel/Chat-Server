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
    
    /// <summary>
    /// Method to get names of all the rooms
    /// </summary>
    /// <returns>Names of the rooms</returns>
    public string[] GetRoomNames()
    {
        lock (roomsLock)
        {
            return rooms.Keys.ToArray();
        }
    }

    /// <summary>
    /// Method to delete room
    /// </summary>
    /// <param name="roomName"></param>
    /// <returns>True if the room is deleted</returns>
    public bool RemoveRoom(string roomName)
    {
        lock (roomsLock)
        {
            if (rooms.TryGetValue(roomName, out var room))
            {
                rooms.Remove(roomName);

                var historyFile = $"{roomName}_history.txt";
                if (File.Exists(historyFile))
                {
                    File.Delete(historyFile);
                }
                return true; 
            }
            return false;
        }
    }
}