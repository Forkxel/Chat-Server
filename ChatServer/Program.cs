// See https://aka.ms/new-console-template for more information

var server = new ChatServer.ChatServer(5000);

server.Start();
Console.WriteLine("Type 'exit' to stop.");
while(Console.ReadLine() != "exit") { }
server.Stop();