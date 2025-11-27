namespace ChatServer;

/// <summary>
/// Class 
/// </summary>
public class Logger
{
    private string filePath = "chatLog.txt";
    private object locker = new();

    public void Log(string message)
    {
        var line = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}";

        lock (locker)
        {
            File.AppendAllText(filePath, line + Environment.NewLine);
        }
    }
}