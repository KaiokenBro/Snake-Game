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
    ///     A list of active client connections. Each <see cref="NetworkConnection"/>
    ///     in this list represents a client currently connected to the server.
    /// </summary>
    private static List<NetworkConnection> clients = new();

    /// <summary>
    ///     A dictionary mapping each client connection to its associated name.
    ///     The <see cref="NetworkConnection"/> key represents a client, and the 
    ///     <see cref="string"/> value represents the client's name.
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
        try
        {
            // Welcome clinet, ask for name
            connection.Send("Welcome! Please enter your name.");

            // Name of client
            string clientName = connection.ReadLine();

            // Locked to prevent multiple List mutations at the same time
            lock (clients)
            {
                // Add client to List
                clients.Add(connection);

                // Assign client name to its respected client.
                names[connection] = clientName;
            }

            // Notify client they have joined the chat
            connection.Send("You are now connected to the chat.");

            // Confirm client's name has joined the chat
            Console.WriteLine($"{clientName} has joined the chat.");

            // Infinite loop handling all messages until client disconnects
            while (true)
            {
                try
                {
                    // Message from client
                    string message = connection.ReadLine();

                    // Locked so only one client can Broadcast at a time.
                    lock (clients)
                    {
                        foreach (var client in clients)
                        {
                            client.Send($"{clientName}: {message}");
                        }
                    }

                    // Confirm message recieved from client
                    Console.WriteLine($"Recieved message from {clientName}: {message}");
                }
                // Handles client disconnecting while in List
                catch (Exception)
                {
                    // Confirm which client disconnected
                    Console.WriteLine($"{names[connection]} has disconnected.");

                    // Locked to prevent multiple List mutations at the same time
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
        // Handles client disconnecting before adding to List
        catch (Exception)
        {
            // Confirm the client disconnected
            Console.WriteLine("Client has disconnected.");

            // Disconnect the client connection
            connection.Disconnect();

            return;
        }
    }
}