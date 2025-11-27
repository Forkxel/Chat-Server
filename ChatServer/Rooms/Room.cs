using System.Collections.Concurrent;

namespace ChatServer.Rooms;

public class Room
{
    public string Name { get; set; }
    private List<ClientHandler> members = new();
    private object membersLock = new();

    public Room(string name)
    {
        Name = name;
    }

    public void AddMember(ClientHandler handler)
    {
        lock (membersLock)
        {
            members.Add(handler);
        }
    }

    public void RemoveMember(ClientHandler handler)
    {
        lock (membersLock)
        {
            members.Remove(handler);
        }
    }
}