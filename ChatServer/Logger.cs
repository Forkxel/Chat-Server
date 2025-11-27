namespace ChatServer;

/// <summary>
/// Simple logger that writes messages with timestamps to a text file.
/// </summary>
public class Logger
{
    private string filePath = "chatLog.txt";
    private object locker = new();

    /// <summary>
    /// Appends a message to the log file with timestamp.
    /// </summary>
    /// <param name="message">Message to log.</param>
    public void Log(string message)
    {
        var line = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}";

        lock (locker)
        {
            File.AppendAllText(filePath, line + Environment.NewLine);
        }
    }
}