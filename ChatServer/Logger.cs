namespace ChatServer;

/// <summary>
/// Simple logger that writes messages with timestamps to a text file.
/// </summary>
public static class Logger
{
    private static string filePath = "chatLog.txt";
    private static object locker = new();

    /// <summary>
    /// Appends a message to the log file with timestamp.
    /// </summary>
    /// <param name="message">Message to log.</param>
    public static void Log(string message)
    {
        var line = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}";

        lock (locker)
        {
            File.AppendAllText(filePath, line + Environment.NewLine);
        }
    }
}