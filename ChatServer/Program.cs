// See https://aka.ms/new-console-template for more information

using System.Configuration;

var portStr = ConfigurationManager.AppSettings["ServerPort"];

int port = 5000;
if (!string.IsNullOrEmpty(portStr))
{
    int.TryParse(portStr, out port);
}

var server = new ChatServer.ChatServer(port);

try
{
    server.Start();
    Console.WriteLine("Type 'exit' to stop the server.");
    while ((Console.ReadLine() ?? "") != "exit") { }
}
catch (Exception e)
{
    Console.WriteLine("Server error: " + e.Message);
}
finally
{
    server.Stop();
}