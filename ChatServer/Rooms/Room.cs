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
    private Queue<string> messageHistory = new();
    private int maxHistory = 50;
    private object historyLock = new();

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

        lock (historyLock)
        {
            foreach (var msg in messageHistory)
            {
                handler.SendMessage(msg);
            }
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

    /// <summary>
    /// Returns all members in list
    /// </summary>
    /// <returns>List of members</returns>
    public IEnumerable<ClientHandler> GetMembers()
    {
        lock (membersLock)
        {
            return members.ToList();
        }
    }
    
    /// <summary>
    /// Saves written messages to List to save
    /// </summary>
    /// <param name="message"></param>
    public void AddMessageToHistory(string message)
    {
        lock (historyLock)
        {
            messageHistory.Enqueue(message);
            if (messageHistory.Count > maxHistory)
            {
                messageHistory.Dequeue();
            }
        }
    }
}