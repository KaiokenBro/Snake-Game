using GUI.Client.Models;
using System.Data.Common;
using System.Text.Json;

namespace GUI.Client.Controllers
{

    public class NetworkController
    {
        private NetworkConnection network;
        private bool receivedID = false;
        private bool receivedSize = false;
        private int worldSize;
        public int playerID;
        private string playerName;
        private bool commandSentThisFrame = false; // Flag to track if a command was sent in this frame

        public World theWorld { get; set; }

        public NetworkController(NetworkConnection connection, string playerName)
        {
            this.network = connection;
            this.playerName = playerName;
        }

        public async Task ReceiveFromServerAsync()
        {
            while (network.IsConnected)
            {
                try
                {
                    // Asynchronously receive data from the network
                    string message = await Task.Run(() => network.ReadLine());

                    HandleServerData(message);
                }
                catch (Exception)
                {
                    break;
                }
            }
        }

        public void HandleServerData(string message)
        {
            // Handle first 2 messages from server, PlayerID and WorldSize
            if (!receivedID || !receivedSize)
            {
                if (!receivedID)
                {
                    // Parse playerID from the message string
                    if (int.TryParse(message, out int parsedPlayerID))
                    {
                        playerID = parsedPlayerID;
                        receivedID = true;
                    }
                }
                else if (!receivedSize)
                {
                    // Parse worldSize from the message string
                    if (int.TryParse(message, out int parsedWorldSize))
                    {
                        worldSize = parsedWorldSize;
                        receivedSize = true;

                        // Now that we have the world size, create a new World instance
                        theWorld = new World(worldSize);

                        // Create a new Snake for the player
                        Snake userSnake = new Snake();

                        // Set new snakes ID
                        userSnake.SnakeID = playerID;

                        // Set snakes player name
                        userSnake.PlayerName = playerName;

                        lock (theWorld)
                        {
                            // Add the Snake to the world
                            theWorld.Snakes[playerID] = userSnake;
                        }
                    }
                }
            }
            // Handle remaining JSON messages
            else
            {
                ParseJsonData(message);
            }
        }

        private void ParseJsonData(string jsonMessage)
        {
            try
            {
                // Check if the JSON is for a wall
                if (jsonMessage.Contains("\"wall\""))
                {
                    // Deserialize the JSON message directly into a Wall object
                    Wall? wall = JsonSerializer.Deserialize<Wall>(jsonMessage);

                    // Add the wall in the world's dictionary
                    lock (theWorld)
                    {
                        theWorld.Walls[wall.WallID] = wall;
                    }
                }

                // Check if the JSON is a powerup
                if (jsonMessage.Contains("\"power\""))
                {
                    // Deserialize the JSON message directly into a powerup object
                    Powerup? powerup = JsonSerializer.Deserialize<Powerup>(jsonMessage);

                    // If the powerup died
                    if (powerup.PowerupDied)
                    {
                        lock (theWorld)
                        {
                            // Remove the powerup from the dictionary
                            theWorld.Powerups.Remove(powerup.PowerupID);
                        }
                    }

                    // If the powerup is new or still alive
                    else if (!powerup.PowerupDied)
                    {
                        lock (theWorld)
                        {
                            // Add/Update it in the collection
                            theWorld.Powerups[powerup.PowerupID] = powerup;
                        }
                    }
                }

                // Check if the JSON is a snake
                if (jsonMessage.Contains("\"snake\""))
                {
                    // Deserialize the JSON message directly into a snake object
                    Snake? snake = JsonSerializer.Deserialize<Snake>(jsonMessage);

                    // If the player disconnected
                    if (snake.PlayerDisconnected)
                    {
                        lock (theWorld)
                        {
                            // Remove the snake from the dictionary
                            theWorld.Snakes.Remove(snake.SnakeID);
                        }
                    }

                    // If the player is connected
                    else if (!snake.SnakeDied)
                    {
                        lock (theWorld)
                        {
                            // Add/Update it in the collection
                            theWorld.Snakes[snake.SnakeID] = snake;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error parsing JSON");
            }
        }

        public void KeyPressCommand(string key)
        {

            // If a command has already been sent this frame, don't process further
            if (commandSentThisFrame)
            {
                return;
            }

            // Map the key to a movement direction
            string direction = key switch
            {
                "w" => "up",
                "a" => "left",
                "s" => "down",
                "d" => "right"
            };

            // If the direction is valid, send it to the NetworkController
            if (direction != null)
            {
                // Create a ControlCommand object with the direction
                ControlCommand command = new ControlCommand(direction);

                // Send the command to the server
                SendCommand(command);

                // Set the flag to true to prevent sending more commands in this frame
                commandSentThisFrame = true;
            }
        }

        private void SendCommand(ControlCommand command)
        {
            // Serialize command to JSON format
            string commandJson = JsonSerializer.Serialize(command);

            // Send the JSON command to the server
            network.Send(commandJson);
        }

        // Reset the commandSentThisFrame flag at the start of the next frame (e.g., in the game loop)
        public void ResetCommandFlag()
        {
            commandSentThisFrame = false;
        }

    }
}