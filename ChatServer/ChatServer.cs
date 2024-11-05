// <copyright file="ChatServer.cs" company="UofU-CS3500">
//     Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// 
// Name: Harrison Doppelt and Victor Valdez Landa
// Date: 11/03/2024

using CS3500.Networking;
using System.Text;

namespace CS3500.Chatting;



/// <summary>
///     A simple ChatServer that handles clients separately and replies with a static message.
/// </summary>
public partial class ChatServer
{
    private static List<NetworkConnection> clients = new();
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
        Console.Read(); // don't stop the program.
    }

    /// <summary>
    ///     <pre>
    ///         When a new connection is established, enter a loop that receives from and
    ///         replies to a client.
    ///     </pre>
    /// </summary>
    ///
    private static void HandleConnect(NetworkConnection connection)
    {
        string clientName = connection.ReadLine();
        lock (clients)
        {
            clients.Add(connection);
            //Put client name into its respected client.
            names[connection] = clientName;
        }

        //Display who has joined
        connection.Send($"{clientName} has connected");
        Console.WriteLine($"{clientName} has connected");

        // Handle all messages until disconnect.
        while (true)
        {
            try
            {

                var message = connection.ReadLine();


                //Send messages to all clients and state who sent what
                lock (clients)
                {
                    foreach (var client in clients)
                    {
                        client.Send($"{clientName}: {message}");
                        
                    }
                }

                // Confirm message recieved from client
                Console.WriteLine($"Recieved from client: {clientName}");
            }
            catch (Exception)
            {
                // Confirm which client disconnected
                Console.WriteLine($"{names[connection]} has disconnected.");

                //Clean up the client that left/disconnected
                lock (clients)
                {
                    clients.Remove(connection);
                    names.Remove(connection);
                }

                // Disconnect the client client
                connection.Disconnect();

                return;
            }
        }
    }
}