using GUI.Client.Models;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Text.Json;

namespace GUI.Client.Controllers
{
    /// <summary>
    ///     Manages communication between the client and the server, maintaining the game state and processing server data.
    /// </summary>
    /// <param name="connection">The network connection used to communicate with the server.</param>
    /// <param name="playerName">The name of the player to associate with this controller.</param>
    public class NetworkController(NetworkConnection connection, string playerName)
    {
        /// <summary>
        ///     Represents the network connection used for communication with the server.
        /// </summary>
        private readonly NetworkConnection network = connection;

        /// <summary>
        ///     Indicates whether the player ID has been received from the server.
        /// </summary>
        private bool receivedID = false;

        /// <summary>
        ///     Indicates whether the world size has been received from the server.
        /// </summary>
        private bool receivedSize = false;

        /// <summary>
        ///     Stores the size of the game world, as specified by the server.
        /// </summary>
        private int worldSize;

        /// <summary>
        ///     Stores the unique ID assigned to the player by the server.
        /// </summary>
        public int PlayerID { get; private set; }

        /// <summary>
        ///     Stores the player's name, as specified during initialization.
        /// </summary>
        private readonly string playerName = playerName;

        /// <summary>
        ///     Indicates whether a command has been sent to the server during the current frame.
        /// </summary>
        private bool commandSentThisFrame = false;

        /// <summary>
        ///     Represents the current state of the game world.
        ///     This is populated with data received from the server.
        /// </summary>
        public World? TheWorld { get; private set; }

        /// <summary>
        ///     Connection string used to establish a connection to the MySQL database.
        /// </summary>
        public const string connectionString = "server=localhost;" + "database=snake_game;" + "user=root;" + "password=admin";

        /// <summary>
        ///     Holds the ID of the current game session.
        ///     This value is set after a new game is added to the database.
        /// </summary>
        private int currentGameId;

        /// <summary>
        ///     Inserts a new row into the "Games" table in the database, recording the start time of the game.
        ///     Also retrieves the ID of the newly created game for future use.
        ///     
        ///     This method is called in the ConnectToServerAsync method in the SnakeGUI.razor class.
        /// </summary>
        public void AddNewGameToDatabase()
        {
            try
            {
                string formattedStartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                using (MySqlConnection databaseConnection = new MySqlConnection(connectionString))
                {
                    databaseConnection.Open();
                    MySqlCommand command = databaseConnection.CreateCommand();
                    command.CommandText = $"INSERT INTO Games (start_time, end_time) VALUES ('{formattedStartTime}', NULL);";
                    command.ExecuteNonQuery();

                    string selectQuery = "SELECT LAST_INSERT_ID();";

                    using (MySqlCommand selectCommand = new MySqlCommand(selectQuery, databaseConnection))
                    {
                        var result = selectCommand.ExecuteScalar();

                        currentGameId = (int)Convert.ToInt64(result);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error starting new game: {e.Message}");
            }
        }

        /// <summary>
        ///     Adds a new snake entry to the "Players" table in the database.
        ///     It records the snake's details including ID, name, max score, enter time, and game ID.
        ///     
        ///     This method is initially called in the HandleServerData method of the NetworkController class (For the current player).
        ///     This method is then called in the ParseJsonData method of the NetworkController class (For other players).
        /// </summary>
        /// <param name="snake">
        ///     An instance of the Snake class containing details of the snake to be added to the database.
        /// </param>
        private void AddNewSnakeToDatabase(Snake snake)
        {
            try
            {
                string formattedEnterTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                using (MySqlConnection databaseConnection = new MySqlConnection(connectionString))
                {
                    databaseConnection.Open();
                    MySqlCommand command = databaseConnection.CreateCommand();
                    command.CommandText = $"INSERT INTO Players (id, name, max_score, enter_time, leave_time, game_id) VALUES ({snake.SnakeID}, '{snake.PlayerName}', {snake.PlayerMaxScore}, '{formattedEnterTime}', NULL, {currentGameId});";
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding snake {snake.SnakeID} to the database: {ex.Message}");
            }
        }

        /// <summary>
        ///     Updates the max score for a player in the database if the new score is higher.
        ///     
        ///     This method is called in the ParseJsonData method.
        /// </summary>
        /// <param name="snakeId">The unique ID of the snake (player) whose max score is being updated.</param>
        /// <param name="newScore">The new max score to update in the database.</param>
        private void UpdatePlayerMaxScoreInDatabase(int snakeId, int newScore)
        {
            // Create and start the thread inside this method
            Thread databaseThread = new Thread(() =>
            {
                try
                {
                    using (MySqlConnection databaseConnection = new MySqlConnection(connectionString))
                    {
                        databaseConnection.Open();
                        MySqlCommand command = databaseConnection.CreateCommand();
                        command.CommandText = $"UPDATE Players SET max_score = {newScore} WHERE id = {snakeId} AND game_id = {currentGameId};";
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating max score for player {snakeId}: {ex.Message}");
                }
            });

            databaseThread.IsBackground = true; // Make it a background thread
            databaseThread.Start();
        }

        /// <summary>
        ///     Updates the leave time for a player in the "Players" table in the database.
        ///     
        ///     This method is called in the ParseJsonData method to record when a player leaves the game (Other players).
        ///     This method is also called in the DisconnectFromServer method in the SnakeGUI.razor class when a player leaves the game (Current Player).
        /// </summary>
        /// <param name="snakeId">
        ///     The ID of the snake (player) whose leave time is being updated.
        /// </param>
        public void UpdatePlayerLeaveTimeInDatabase(int snakeId)
        {
            try
            {
                string formattedLeaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                using (MySqlConnection databaseConnection = new MySqlConnection(connectionString))
                {
                    databaseConnection.Open();
                    MySqlCommand command = databaseConnection.CreateCommand();
                    command.CommandText = $"UPDATE Players SET leave_time = '{formattedLeaveTime}' WHERE id = {snakeId};";
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating leave time for player: {ex.Message}");
            }
        }

        /// <summary>
        ///     Updates the end time for the current game in the "Games" table in the database.
        ///     
        ///     This method is called in the DisconnectFromServer method in SnakeGUI.razor class to record when a game ends.
        /// </summary>
        public void UpdateGameEndTimeInDatabase()
        {
            try
            {
                string formattedEndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                using (MySqlConnection databaseConnection = new MySqlConnection(connectionString))
                {
                    databaseConnection.Open();
                    MySqlCommand command = databaseConnection.CreateCommand();
                    command.CommandText = $"UPDATE Games SET end_time = '{formattedEndTime}' WHERE id = {currentGameId};";
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating game end time: {ex.Message}");
            }
        }

        /// <summary>
        ///     Continuously listens for messages from the server and processes them.
        /// </summary>
        public void NetworkLoop(NetworkController networkController)
        {
            while (network.IsConnected)
            {
                try
                {
                    if (networkController != null)
                    {
                        ReceiveFromServer();
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    break;
                }
            }
        }

        /// <summary>
        ///     Continuously receives data from the server while the connection is active.
        ///     Processes each message received by delegating to the <see cref="HandleServerData(string)"/> method.
        /// </summary>
        private void ReceiveFromServer()
        {
            while (network.IsConnected)
            {
                try
                {
                    string message = network.ReadLine();

                    if (message == null)
                    {
                        break;
                    }

                    HandleServerData(message);
                }
                catch (Exception)
                {
                    break;
                }
            }

            network.Disconnect();
        }

        /// <summary>
        ///     Processes data received from the server to initialize the game state or update existing information.
        /// </summary>
        /// <param name="message">The message received from the server as a string.</param>
        /// <remarks>
        ///     This method handles initial data reception, such as player ID and world size, and delegates further JSON processing
        ///     to <see cref="ParseJsonData(string)"/> for additional updates.
        /// </remarks>
        private void HandleServerData(string message)
        {
            if (!receivedID || !receivedSize)
            {
                if (!receivedID)
                {
                    if (int.TryParse(message, out int parsedPlayerID))
                    {
                        PlayerID = parsedPlayerID;
                        receivedID = true;
                    }
                }
                else if (!receivedSize)
                {
                    if (int.TryParse(message, out int parsedWorldSize))
                    {
                        worldSize = parsedWorldSize;
                        receivedSize = true;
                        TheWorld = new World(worldSize);
                        Snake userSnake = new();
                        userSnake.SetPlayerName(playerName);
                        userSnake.SetSnakeID(PlayerID);
                        AddNewSnakeToDatabase(userSnake);

                        lock (TheWorld)
                        {
                            TheWorld.Snakes[PlayerID] = userSnake;
                        }
                    }
                }
            }
            else
            {
                ParseJsonData(message);
            }
        }

        /// <summary>
        ///     Parses a JSON message from the server and updates the game world accordingly.
        /// </summary>
        /// <param name="jsonMessage">The JSON-formatted message received from the server.</param>
        /// <remarks>
        ///     This method processes different types of game objects (e.g., walls, powerups, snakes)
        ///     based on the contents of the JSON message. It updates the game world by deserializing
        ///     the data and modifying the appropriate collections in a thread-safe manner.
        /// </remarks>
        private void ParseJsonData(string jsonMessage)
        {
            try
            {
                if (jsonMessage.Contains("\"wall\""))
                {
                    Wall? wall = JsonSerializer.Deserialize<Wall>(jsonMessage);

                    if (wall != null && TheWorld?.Walls != null)
                    {
                        lock (TheWorld)
                        {
                            TheWorld.Walls[wall.WallID] = wall;
                        }
                    }
                }

                if (jsonMessage.Contains("\"power\""))
                {
                    Powerup? powerup = JsonSerializer.Deserialize<Powerup>(jsonMessage);

                    if (powerup != null && TheWorld?.Powerups != null)
                    {
                        if (powerup.PowerupDied)
                        {
                            lock (TheWorld)
                            {
                                TheWorld.Powerups.Remove(powerup.PowerupID);
                            }
                        }
                        else
                        {
                            lock (TheWorld)
                            {
                                TheWorld.Powerups[powerup.PowerupID] = powerup;
                            }
                        }
                    }
                }

                if (jsonMessage.Contains("\"snake\""))
                {
                    Snake? snake = JsonSerializer.Deserialize<Snake>(jsonMessage);

                    // If the snake is valid
                    if (snake != null && TheWorld?.Snakes != null)
                    {
                        // If the snake disconnected
                        if (snake.PlayerDisconnected)
                        {
                            UpdatePlayerLeaveTimeInDatabase(snake.SnakeID);

                            lock (TheWorld)
                            {
                                // Remove the snake from the dictionary
                                TheWorld.Snakes.Remove(snake.SnakeID);
                            }
                        }
                        // If the snake is new or already exists
                        else
                        {
                            // If the snake is new
                            if (!TheWorld.Snakes.ContainsKey(snake.SnakeID))
                            {
                                AddNewSnakeToDatabase(snake);

                                lock (TheWorld)
                                {
                                    // Add the snake to the dictionary
                                    TheWorld.Snakes[snake.SnakeID] = snake;
                                }
                            }
                            // If the snake already exists
                            else
                            {
                                // Retrieve the existing snake
                                Snake existingSnake = TheWorld.Snakes[snake.SnakeID];

                                // Retain the PlayerMaxScore from the existing snake
                                snake.UpdatePlayerMaxScore(existingSnake.PlayerMaxScore);

                                lock (TheWorld)
                                {
                                    // Update the snake to the dictionary
                                    TheWorld.Snakes[snake.SnakeID] = snake;
                                }

                                // If current score is greater than max score
                                if (snake.PlayerScore > snake.PlayerMaxScore)
                                {
                                    // Update Player Max Score
                                    snake.UpdatePlayerMaxScore(snake.PlayerScore);

                                    // Update database
                                    UpdatePlayerMaxScoreInDatabase(snake.SnakeID, snake.PlayerMaxScore);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error parsing JSON");
            }
        }

        /// <summary>
        ///     Handles a key press from the user and sends a corresponding control command to the server.
        /// </summary>
        /// <param name="key">The key pressed by the user, representing a direction (e.g., "w", "a", "s", "d").</param>
        /// <remarks>
        ///     This method maps the key to a direction, creates a <see cref="ControlCommand"/> object, and sends it to the server.
        ///     It ensures that only one command is sent per frame by using the <c>commandSentThisFrame</c> flag.
        /// </remarks>
        public void KeyPressCommand(string key)
        {
            if (commandSentThisFrame)
            {
                return;
            }

            string? direction = key switch
            {
                "w" => "up",
                "a" => "left",
                "s" => "down",
                "d" => "right",
                _ => null
            };

            if (direction != null)
            {
                ControlCommand command = new(direction);
                SendCommand(command);
                commandSentThisFrame = true;
            }
        }

        /// <summary>
        ///     Sends a serialized control command to the server.
        /// </summary>
        /// <param name="command">The control command containing the player's input (direction).</param>
        /// <remarks>
        ///     This method serializes the <see cref="ControlCommand"/> object into JSON
        ///     and sends it to the server using the network connection.
        /// </remarks>
        private void SendCommand(ControlCommand command)
        {
            string commandJson = JsonSerializer.Serialize(command);
            network.Send(commandJson);
        }

        /// <summary>
        ///     Resets the <c>commandSentThisFrame</c> flag, allowing new commands to be sent in the next frame.
        /// </summary>
        /// <remarks>
        ///     This method is called at the start of each game loop frame to ensure the player
        ///     can send new commands in subsequent frames.
        /// </remarks>
        public void ResetCommandFlag()
        {
            commandSentThisFrame = false;
        }
    }
}