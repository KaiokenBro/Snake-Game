// <copyright file="ChatServer.cs" company="UofU-CS3500">
//     Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// 
// Name: Harrison Doppelt and Victor Valdez Landa
// Date: 11/03/2024

using CS3500.Networking;
using System;
using System.Text;

namespace CS3500.Chatting;

/// <summary>
///     A simple ChatServer that handles clients separately and replies with a static message.
/// </summary>
public partial class ChatServer
{
    /// <summary>
    ///     TODO:
    /// </summary>
    private static List<NetworkConnection> clients = new();

    /// <summary>
    ///     TODO:
    /// </summary>
    private static Dictionary<NetworkConnection, string> names = new();

    /// <summary>
    ///     The main program.
    /// </summary>
    /// 
    /// <param name="args"> ignored. </param>
    /// 
    /// <returns> A Task. Not really used. </returns>
    private static void Main(string[] args)
    {
        Server.StartServer(HandleConnect, 11_000);
        Console.Read();
    }

    /// <summary>
    ///     <pre>
    ///         When a new connection is established, enter a loop that receives from and
    ///         replies to a client.
    ///     </pre>
    /// </summary>
    private static void HandleConnect(NetworkConnection connection)
    {
        // Welcome clinet, ask for name
        connection.Send("Welcome! Please enter your name.");

        // Name of client
        string clientName = connection.ReadLine();

        // Locked
        lock (clients)
        {
            // Add client to List
            clients.Add(connection);

            // Assign client name to its respected client.
            names[connection] = clientName;
        }

        // Notify client they have joined the chat
        connection.Send($"{clientName} has joined the chat.");

        // Confirm client's name has joined the chat
        Console.WriteLine($"{clientName} has joined the chat.");

        // Infinite loop handling all messages until client disconnects
        while (true)
        {
            try
            {
                // Message from client
                var message = connection.ReadLine();

                // Broadcast client message to all clients
                lock (clients)
                {
                    foreach (var client in clients)
                    {
                        client.Send($"{clientName}: {message}");
                    }
                }

                // Confirm message recieved from client
                Console.WriteLine($"Recieved from {clientName}: {message}");
            }
            catch (Exception)
            {
                // Confirm which client disconnected
                Console.WriteLine($"{names[connection]} has disconnected.");

                // Clean up the client that disconnected
                lock (clients)
                {
                    clients.Remove(connection);
                    names.Remove(connection);
                }

                // Disconnect the client connection
                connection.Disconnect();

                return;
            }
        }
    }
}