using GUI.Client.Models;
using System.Data.Common;
using System.Text.Json;

namespace GUI.Client.Controllers
{
    /// <summary>
    ///     TODO: XML COMMENT.
    /// </summary>
    public class NetworkController
    {
        /// <summary>
        ///     TODO: XML COMMENT.
        /// </summary>
        private NetworkConnection network;

        public World theWorld { get; set; }

        private bool receivedID = false;
        private bool receivedSize = false;

        private int worldSize;
        private int playerID;
        private string playerName;

        public NetworkController(NetworkConnection connection, string playerName)
        {
            this.network = connection;
            this.playerName = playerName;
        }

        /// <summary>
        ///     Method to start listening for data from the server.
        /// </summary>
        public async Task ReceiveFromServerAsync()
        {
            Console.WriteLine("ReceiveFromServerAsync()");

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
            Console.WriteLine("HandleServerData()");

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
                        Console.WriteLine("PlayerID Received!");
                    }
                }
                else if (!receivedSize)
                {
                    // Parse worldSize from the message string
                    if (int.TryParse(message, out int parsedWorldSize))
                    {
                        worldSize = parsedWorldSize;
                        receivedSize = true;
                        Console.WriteLine("worldSize Received!");

                        // Now that we have the world size, create a new World instance
                        theWorld = new World(worldSize);
                        Console.WriteLine("World Created!");

                        // Create a new Snake for the player
                        Snake userSnake = new Snake();
                        Console.WriteLine("Snake Created!");

                        // Set new snakes ID
                        userSnake.SnakeID = playerID;
                        Console.WriteLine("SnakeID Assigned!");

                        // Set snakes player name
                        userSnake.PlayerName = playerName;
                        Console.WriteLine("PlayerName Assigned!");

                        // Add the Snake to the world
                        theWorld.Snakes[playerID] = userSnake;
                        Console.WriteLine("Snake Added To World!");
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
            Console.WriteLine("ParseJsonData()");

            try
            {

                // Check if the JSON is for a wall
                if (jsonMessage.Contains("\"wall\""))
                {
                    // Deserialize the JSON message directly into a Wall object
                    Wall wall = JsonSerializer.Deserialize<Wall>(jsonMessage);
                    Console.WriteLine("Wall Object Created!");

                    // Add the wall in the world's dictionary
                    theWorld.Walls[wall.WallID] = wall;
                    Console.WriteLine("Wall Object Added to World!");
                }

                // Check if the JSON is a powerup
                else if (jsonMessage.Contains("\"power\""))
                {
                    // Deserialize the JSON message directly into a Powerip object
                    Powerup powerup = JsonSerializer.Deserialize<Powerup>(jsonMessage);

                    // Add the power-up in the world's dictionary
                    theWorld.Powerups[powerup.PowerupID] = powerup;
                }

                else
                {
                    Console.WriteLine("Unknown JSON format.");
                }

            }
            catch (Exception)
            {
                Console.WriteLine("Error parsing JSON");
            }
        }

        /// <summary>
        ///     TODO: XML COMMENT.
        /// </summary>
        /// <param name="key"></param>
        public void KeyPressCommand(string key)
        {
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
            }
        }

        /// <summary>
        ///     TODO: XML COMMENT.
        /// </summary>
        /// <param name="command"></param>
        private void SendCommand(ControlCommand command)
        {
            // Serialize command to JSON format
            string commandJson = JsonSerializer.Serialize(command);

            // Send the JSON command to the server
            network.Send(commandJson);
        }

    }
}