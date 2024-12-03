// Name: Harrison Doppelt and Victor Valdez Landa
// Date: 11/20/2024

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using GUI.Client.Controllers;

namespace WebServer
{
    /// <summary>
    ///     Represents a server task that waits for connections on a given
    ///     port and calls the provided delegate when a connection is made.
    /// </summary>
    public static class Server
    {
        /// <summary>
        ///     Wait on a TcpListener for new connections. Alert the main program
        ///     via a callback (delegate) mechanism.
        /// </summary>
        /// 
        /// <param name="handleConnect">
        ///     Handler for what the user wants to do when a connection is made.
        ///     This should be run asynchronously via a new thread.
        /// </param>
        /// 
        /// <param name="port"> The port (e.g., 11000) to listen on. </param>
        public static void StartServer(Action<NetworkConnection> handleHttpConnection, int port)
        {
            TcpListener listener = new(IPAddress.Any, port);
            listener.Start();

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                new Thread(() => handleHttpConnection(new NetworkConnection(client))).Start();
            }
        }
    }
}
