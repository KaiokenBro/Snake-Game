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
            Server.Connect(HandleHttpConnection, 80);

            // Prevent main from returning
            Console.Read();
        }

        private static void HandleHttpConnection(NetworkConnection connection)
        {

            // Browser sends
            string request = client.ReadLine();
            Console.WriteLine(request);

            // If Homepage
            if (request.Contains("GET /"))
            {

            }

            // If Game
            else if (request.Contains("GET /games"))
            {

            }

            // If Specific Game
            else if (request.Contains("GET /games?gid=x"))
            {

            }

            // If bad request
            else
            {

            }

        }

    }
}
