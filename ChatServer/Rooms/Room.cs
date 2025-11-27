using System.Collections.Concurrent;

namespace ChatServer.Rooms;

/// <summary>
/// Represents a chat room containing multiple clients.
/// </summary>
public class Room
{
    public string Name { get; set; }
    private List<ClientHandler> members = new();
    private object membersLock = new();

    public Room(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Adds a client to the room.
    /// </summary>
    public void AddMember(ClientHandler handler)
    {
        lock (membersLock)
        {
            members.Add(handler);
        }
    }

    /// <summary>
    /// Removes a client from the room.
    /// </summary>
    public void RemoveMember(ClientHandler handler)
    {
        lock (membersLock)
        {
            members.Remove(handler);
        }
    }
}