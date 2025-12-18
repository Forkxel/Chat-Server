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
    private string historyFile;

    public Room(string name)
    {
        Name = name;
        historyFile = $"{name}_history.txt";
        
        if (File.Exists(historyFile))
        {
            var lines = File.ReadAllLines(historyFile);
            foreach (var line in lines.TakeLast(maxHistory))
            {
                messageHistory.Enqueue(line);
            }
        }
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

            try
            {
                File.AppendAllText(historyFile, message + Environment.NewLine);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to write history for room {Name}: {e.Message}");
                
                throw;
            }
        }
    }
    
    /// <summary>
    /// Clears the history of the room
    /// </summary>
    public void ClearHistory()
    {
        lock (historyLock)
        {
            messageHistory.Clear();
            try
            {
                if (File.Exists(historyFile))
                {
                    File.Delete(historyFile);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to clear history for room {Name}");
                Logger.Log($"Failed to clear history for room {Name}");
            }
            
            lock (membersLock)
            {
                foreach (var member in members)
                {
                    member.SendMessage($"[System] History for room {Name} has been cleared.");
                }
            }
        }
    }
}