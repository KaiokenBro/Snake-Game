using GUI.Client.Controllers;
using MySqlX.XDevAPI;

namespace WebServer
{
    internal class WebServer
    {

        public const string httpOkHeader =
            "HTTP/1.1 200 OK\r\n" +
            "Connection: close\r\n" +
            "Content-Type: text/html; charset=UTF-8\r\n" +
            "\r\n";

        private const string httpBadHeader =
            "HTTP/1.1 404 Not Found\r\n" +
            "Connection: close\r\n" +
            "Content-Type: text/html; charset=UTF-8\r\n" +
            "\r\n";

        static void Main(string[] args)
        {
            Server.StartServer(HandleHttpConnection, 80);

            // Prevent main from returning
            Console.Read();
        }

        private static void HandleHttpConnection(NetworkConnection client)
        {

            // Browser sends
            string request = client.ReadLine();

            Console.WriteLine(request);

            // If Homepage
            if (request.Contains("GET /"))
            {
                // Opening HTML
                string response = "<html>";

                // Header
                response += "<h3>Welcome to the Snake Game Database!</h3>";

                // Link
                response += "<a href=\"/games\">View Games</a>";

                // Closing HTML
                response += "</html>";
            }

            // If All Games
            //else if (request.Contains("GET /games"))
            //{

            //}

            // If Specific Game
            //else if (request.Contains("GET /games?gid=x"))
            //{

            //}

            // Server closes socket
            client.Disconnect();
        }

    }
}
