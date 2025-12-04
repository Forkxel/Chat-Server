# Chat Server

A multithreaded TCP chat server in C# with support for multiple rooms, nicknames, and message logging.

## Features

1. Multiple chat rooms, with automatic room creation.
2. Nickname support for clients.
3. Server logs all messages to a file.
4. Clients can join/leave rooms dynamically.
5. Concurrent message dispatching using a queue to ensure thread safety.

## Getting Started

### Dependencies

<ul>
    <li>.NET 9.0 SDK</li>
    <li>NuGet package - System.Configuration.ConfigurationManager </li>
</ul>

### Installing

1. Open GitHub repository https://github.com/Forkxel/Chat-Server.git.
2. Download zip file
3. Open the directory and you will find both exe files.

### Executing program

#### Running Server

1. Open ChatServer.exe in the directory ChatServer-exe
2. The console will show:

```
Loaded configuration: IP: 0.0.0.0 Port: 5000
Server started
Type 'exit' to stop the server.
```
3. Server runs on localhost (network - 127.0.0.0) and listens on port 5000 by default

##### Running Client

1. Open ChatClient.exe in the directory ChatClient-exe
2. You will be prompted to enter the server IP:

```
Server IP:
```
3. After that the client is asked about the port:
```
Server Port: 
```
4. If the IP and the Port is correct client connects to the server
```
Connection established.
Welcome to Chat Server!
Use -nick <name> to set nickname.
Use -join <name> to switch room.
```
5. You can now start sending messages.

##### Client commands

<ul>
    <li>-nick &lt;name> – Set or change your nickname.</li>
    <li>-join &lt;name> – Join an existing room or create a new room.</li>
</ul>

#### Server logging

All messages are saved in chatLog.txt with timestamps and room names:

```
[2025-11-27 12:34:56] [general] Anonymous: Hello world
```

### Customization

If you want to configure IP addresses or Port of the server clone the repository and Open App.config. <br>
Here you will find:
```
<add key="ServerIP" value="0.0.0.0" />
<add key="ServerPort" value="5000" />
```
<ul>
    <li>ServerIP - IP address of the server</li>
    <li>ServerPort - Port of the server</li>
</ul>

## Help

If you need anything from me about this application contact me at:
* pavel.halik06@gmail.com
