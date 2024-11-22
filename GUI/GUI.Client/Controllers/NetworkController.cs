// Name: Harrison Doppelt and Victor Valdez Landa
// Date: 11/20/2024

using GUI.Client.Models;
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

                    if (snake != null && TheWorld?.Snakes != null)
                    {
                        if (snake.PlayerDisconnected)
                        {
                            lock (TheWorld)
                            {
                                TheWorld.Snakes.Remove(snake.SnakeID);
                            }
                        }
                        else
                        {
                            lock (TheWorld)
                            {
                                TheWorld.Snakes[snake.SnakeID] = snake;
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