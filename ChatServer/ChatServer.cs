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
        // Handle all messages until disconnect.
        while (true)
        {
            try
            {
                var message = connection.ReadLine();

                // Confirm message recieved from client
                Console.WriteLine("Recieved from client: " + message);

                // Respond to client
                connection.Send("Thanks!");
            }
            catch (Exception)
            {
                // Confirm client disconnected
                Console.WriteLine("Client connection closed.");

                // Disconnect client
                connection.Disconnect();

                return;
            }
        }
    }
}