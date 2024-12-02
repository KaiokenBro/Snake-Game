using GUI.Client.Controllers;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Linq;

namespace WebServer
{
    internal class WebServer
    {

        // http://localhost:8080/

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
                string response = "<html><h3>Welcome to the Snake Game Database!</h3><a href=\"/games\">View Games</a></html>";

                client.Send(httpOkHeader + response);
            }

            // If All Games
            else if (request.Contains("GET /games"))
            {
                string response = GetAllGames();

                client.Send(httpOkHeader + response);
            }

            // If Specifc Game


            // Server closes socket
            client.Disconnect();
        }

        private static string GetAllGames()
        {
            // List to store games
            var games = new List<(int GameId, string StartTime, string EndTime)>();

            // Create a connection to the database
            using (MySqlConnection databaseConnection = new MySqlConnection(NetworkController.connectionString))
            {
                // Open the connection
                databaseConnection.Open();

                // Create a command
                MySqlCommand command = databaseConnection.CreateCommand();

                // SQL Command
                command.CommandText = "SELECT id, start_time, end_time FROM Games;";

                // Run the command
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    // Get the results out
                    while (reader.Read())
                    {
                        int gameId = reader.GetInt32(0);
                        string startTime = reader.GetDateTime(1).ToString("yyyy-MM-dd HH:mm:ss");
                        string endTime = reader.IsDBNull(2) ? "NULL" : reader.GetDateTime(2).ToString("yyyy-MM-dd HH:mm:ss");

                        games.Add((gameId, startTime, endTime));
                    }
                }
            }

            // holds html
            string html = string.Empty;

            html += "<html><h3>All Games</h3><table border='1'>";
            html += "<thead><tr><td>ID</td><td>Start</td><td>End</td></tr></thead><tbody>";

            foreach (var game in games)
            {
                html += "<tr>";
                html += $"<td><a href='/games?gid={game.GameId}'>{game.GameId}</a></td>";
                html += $"<td>{game.StartTime}</td>";
                html += $"<td>{game.EndTime}</td>";
                html += "</tr>";
            }

            html += "</tbody></table></html>";

            return html;
        }

    }
}