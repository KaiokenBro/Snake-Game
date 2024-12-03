using GUI.Client.Controllers;
using GUI.Client.Models;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Linq;
using System.Text;

namespace WebServer
{
    internal class WebServer
    {

        // http://localhost:8080/

        public const string httpOkHeader =
            "HTTP/1.1 200 OK\r\n" +
            "Connection: close\r\n" +
            "Content-Type: text/html; charset=UTF-8\r\n";

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

            string response = string.Empty;

            // If Specifc Game
            if (request.Contains("GET /games?gid="))
            {
                // Parse Game ID from the request URL
                int gameId = ParseGameId(request);

                response = GetSpecificGamePage(gameId);
            }

            // If Homepage
            else if (request.Contains("GET / "))
            {
                response = "<html><h3>Welcome to the Snake Game Database!</h3><a href=\"/games\">View Games</a></html>";
            }

            // If All Games
            else if (request.Contains("GET /games"))
            {
                response = GetGamePage();
            }

            // Calculate Content-Length dynamically
            int contentLength = Encoding.UTF8.GetByteCount(response);
            string header = httpOkHeader + $"Content-Length: {contentLength}\r\n\r\n";

            // Send response
            client.Send(header + response);

            // Server closes socket
            client.Disconnect();
        }

        private static string GetGamePage()
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

        private static int ParseGameId(string request)
        {
            // Extract the part after "gid="
            int startIndex = request.IndexOf("?gid=") + 5;
            string gameIdString = request.Substring(startIndex).Split(' ')[0];

            // Convert to integer
            if (int.TryParse(gameIdString, out int gameId))
            {
                return gameId;
            }

            return -1;
        }

        private static string GetSpecificGamePage(int gameId)
        {
            string gameInfoHtml = "";
            string playersHtml = "";

            // Create a connection to the database
            using (MySqlConnection databaseConnection = new MySqlConnection(NetworkController.connectionString))
            {
                // Open the connection
                databaseConnection.Open();

                // Create a command
                MySqlCommand command = databaseConnection.CreateCommand();

                // SQL Command
                command.CommandText = "SELECT id, name, max_score, enter_time, leave_time FROM Players WHERE game_id = @gameId;";

                // Add the parameters to the SQL query
                command.Parameters.AddWithValue("@gameId", gameId);

                // Run the command
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    playersHtml = "<table border='1'><thead><tr>" +
                                  "<td>Player ID</td><td>Player Name</td><td>Max Score</td><td>Enter Time</td><td>Leave Time</td>" +
                                  "</tr></thead><tbody>";

                    // Get the results out
                    while (reader.Read())
                    {
                        int playerId = reader.GetInt32(0);
                        string playerName = reader.GetString(1);
                        int maxScore = reader.GetInt32(2);
                        string enterTime = reader.GetDateTime(3).ToString("yyyy-MM-dd HH:mm:ss");
                        string leaveTime = reader.IsDBNull(4) ? "NULL" : reader.GetDateTime(4).ToString("yyyy-MM-dd HH:mm:ss");

                        playersHtml += $"<tr><td>{playerId}</td><td>{playerName}</td><td>{maxScore}</td>" + $"<td>{enterTime}</td><td>{leaveTime}</td></tr>";
                    }
                }

                // Combine game info and player stats
                return $"<html><h3>Stats for Game {gameId}</h3>{gameInfoHtml}{playersHtml}</html>";
            }
        }
    }
}