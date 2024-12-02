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
            Server.StartServer(HandleHttpConnection, 8080);

            // Prevent main from returning
            Console.Read();
        }

        private static void HandleHttpConnection(NetworkConnection client)
        {

            // Browser sends
            string request = client.ReadLine();

            // If Homepage
            if (request.Contains("GET / "))
            {
                // Opening HTML
                string response = "<html>";

                // Header
                response += "<h3>Welcome to the Snake Game Database!</h3>";

                // Link
                response += "<a href=\"/games\">View Games</a>";

                // Closing HTML
                response += "</html>";

                client.Send(httpOkHeader + response);
            }

            // If All Games
            else if (request.Contains("GET /games"))
            {
                // Opening HTML
                string response = "<html>";

                // Opening Table
                response += "<table border=\"1\">";

                // Opening Table Head
                response += "<thead>";

                // Opening Table Row
                response += "<tr>";

                // Table Data
                response += "<td>ID</td><td>Start</td><td>End</td>";

                // Closing Table Row
                response += "</tr>";

                // Closing Table Head
                response += "</thead>";

                // Opening Table Body
                response += "<tbody>";

                // Opening Table Row
                response += "<tr>";

                // Table Data
                // TODO

                // Closing Table Row
                response += "</tr>";

                // Closing Table Body
                response += "</tbody>";

                // Closing Table
                response += "</table>";

                // Closing HTML
                response += "</html>";

                client.Send(httpOkHeader + response);
            }

            // Server closes socket
            client.Disconnect();
        }

    }
}