using System.Collections.Concurrent;

namespace ChatServer.Rooms;

public class Room
{
    public string Name { get; set; }
    private ConcurrentDictionary<ClientHandler, byte> members = new();

    public Room(string name)
    {
        Name = name;
    }

    public void AddMember(ClientHandler handler)
    {
        members[handler] = 0;
    }

    public void RemoveMember(ClientHandler handler)
    {
        members.TryRemove(handler, out _);
    }

    public IEnumerable<ClientHandler> GetMembers()
    {
        return members.Keys;
    }
}