# Snake Game

A networked multiplayer Snake game built with C# and Blazor, featuring a GUI client, AI-controlled snakes, and a server that manages real-time game logic and player state. 
Users can connect to a local server, play against other players, or launch AI snakes for added challenge.

## Features
- Interactive Blazor-based GUI client
- Optional AI-controlled snake players
- Server-based game state management
- Real-time multiplayer over localhost
- Snake stats viewable via a web browser (via WebServer)

## 🛠️ Technologies Used
- C# / .NET
- Blazor
- TCP Sockets
- ASP.NET Core
- Visual Studio 2022

## 🚀 Getting Started
**Prerequisites**
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Visual Studio 2022 or later (with ASP.NET and Blazor workload)
- Windows OS (for running `.exe` files)

**Build & Run**
1. Start the server
   
```bash
Server-AI-Client > Server.exe
```

- This starts the core game logic on the default port (11000).

2. Run the GUI Client

- Open Snake.sln in Visual Studio.
- Set GUI as the startup project.
- Run it (F5 or "Start Debugging").
- On the Welcome screen, navigate to the Snake Game screen.
- Enter your name and connect using port 11000.

3. Add AI Snakes (Optional)

```bash
Server-AI-Client > AIClient.exe
```

- Type 'localhost' for server
- Type # of desired AI Snakes.

## 📄 License
This project is licensed under the MIT License. See the LICENSE file for details.

## 👤 Author
hdoppelt

KaiokenBro
