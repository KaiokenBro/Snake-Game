// Name: Harrison Doppelt and Victor Valdez Landa
// Date: 11/20/2024

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

        // Connection string
        private const string connectionString = 
            "server=atr.eng.utah.edu;" +
            "database=u0674744;" +
            "uid=u0674744;" +
            "password=CS3500";

        // Holds the current game id
        private int currentGameId;

        // Called in ConnectToServerAsync in client
        // Inserts a new row into the games table
        public void AddNewGameToDatabase()
        {
            try
            {
                // Create a connection to the database
                using (MySqlConnection databaseConnection = new MySqlConnection(connectionString))
                {
                    // SQL query to insert a new row into the Games table
                    // This logs the game's start time (NOW()) and leaves end_time as NULL for now
                    string insertQuery = "INSERT INTO Games (start_time, end_time) VALUES (NOW(), NULL);";

                    // Open the connection
                    databaseConnection.Open();

                    // Create a command to execute the INSERT query
                    using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, databaseConnection))
                    {
                        // Execute the INSERT command
                        // This will add a new game row to the Games table
                        insertCommand.ExecuteNonQuery();
                    }

                    // Retrieve the last inserted game ID

                    // SQL query to select the last id added to games table
                    string selectQuery = "SELECT LAST_INSERT_ID();";

                    // Create a command to execute the SELECT query
                    using (MySqlCommand selectCommand = new MySqlCommand(selectQuery, databaseConnection))
                    {
                        // Store gameId in result
                        var result = selectCommand.ExecuteScalar();

                        currentGameId = (int)Convert.ToInt64(result); // Explicitly cast to int
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error starting new game: {e.Message}");
            }
        }

        // Called in ParseJsonData in NetworkController
        // Inserts a new row into the Players table
        public void AddNewSnakeToDatabase(Snake snake, int gameId)
        {
            try
            {
                // Create a connection to the database
                using (MySqlConnection databaseConnection = new MySqlConnection(connectionString))
                {
                    // SQL query to insert a new row into the Players table
                    // This logs the game's start time (NOW()) and leaves end_time as NULL for now
                    string query = "INSERT INTO Players (id, name, max_score, enter_time, leave_time, game_id) " + "VALUES (@id, @name, @maxScore, NOW(), NULL, @gameId);";

                    // Open the database connection
                    databaseConnection.Open();

                    // Create a command to execute the INSERT query
                    using (MySqlCommand command = new MySqlCommand(query, databaseConnection))
                    {
                        // Bind the parameters to the query
                        command.Parameters.AddWithValue("@id", snake.SnakeID);
                        command.Parameters.AddWithValue("@name", snake.PlayerName);
                        command.Parameters.AddWithValue("@maxScore", snake.PlayerMaxScore);
                        command.Parameters.AddWithValue("@gameId", gameId);

                        // Execute the INSERT command
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding snake {snake.SnakeID} to the database: {ex.Message}");
            }
        }

        // Update players maxscore in database
        public void UpdatePlayerMaxScoreInDatabase()
        {

        }

        // Update player leavetime in database
        public void UpdatePlayerLeaveTimeInDatabase()
        {

        }

        // Update game endtime in database
        public void UpdateGameEndTimeInDatabase()
        {
            try
            {
                // Create a connection to the database
                using (MySqlConnection databaseConnection = new MySqlConnection(connectionString))
                {
                    // SQL query to update end-time for the current game
                    string updateQuery = "UPDATE Games SET end_time = NOW() WHERE id = @gameId;";

                    // Open the database connection
                    databaseConnection.Open();

                    // Create a command to execute the UPDATE query
                    using (MySqlCommand command = new MySqlCommand(updateQuery, databaseConnection))
                    {
                        // Execute the update command 
                        command.ExecuteNonQuery();
                    }
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
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task NetworkLoop(NetworkController networkController)
        {
            while (network.IsConnected)
            {
                try
                {
                    if (networkController != null)
                    {
                        await ReceiveFromServerAsync();
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
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ReceiveFromServerAsync()
        {
            while (network.IsConnected)
            {
                try
                {
                    string message = await Task.Run(network.ReadLine);

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
                            lock (TheWorld)
                            {
                                // Remove the snake from the dictionary
                                TheWorld.Snakes.Remove(snake.SnakeID);

                                // Update leave time for that player in the database
                                UpdatePlayerLeaveTimeInDatabase();
                            }
                        }

                        // If the snake is new or already exists
                        else
                        {
                            lock (TheWorld)
                            {
                                // If the snake is new
                                if (!TheWorld.Snakes.ContainsKey(snake.SnakeID))
                                {
                                    // Add the snake to the dictionary
                                    TheWorld.Snakes[snake.SnakeID] = snake;

                                    // Add the new snake to the database
                                    AddNewSnakeToDatabase(snake, currentGameId);
                                }
                                // If the snake already exists
                                else
                                {
                                    // Update the existing snake in the dictionary
                                    TheWorld.Snakes[snake.SnakeID] = snake;

                                    // Update players maxscore in Database
                                    UpdatePlayerMaxScoreInDatabase();
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