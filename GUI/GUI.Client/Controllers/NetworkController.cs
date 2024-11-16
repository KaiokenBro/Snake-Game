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

        private int powerupCounter = 0; // Counter for unique powerup IDs

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

                        // Add the Snake to the world
                        theWorld.Snakes[playerID] = userSnake;
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

                    // Add the wall in the world's dictionary
                    theWorld.Walls[wall.WallID] = wall;
                }

                // Check if the JSON is a powerup
                else if (jsonMessage.Contains("\"power\""))
                {
                    // Deserialize the JSON message directly into a Powerip object
                    Powerup powerup = JsonSerializer.Deserialize<Powerup>(jsonMessage);

                    // Add the power-up in the world's dictionary
                    theWorld.Powerups[powerup.PowerupID] = powerup;
                }

                // Check if the JSON is a snake

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

        public void GeneratePowerup()
        {
            // Generate a unique ID for the powerup
            int powerupID = powerupCounter;

            // Increment the counter for the next powerup
            powerupCounter++;

            // Generate random location within the world bounds
            Random random = new Random();
            int x = random.Next(-worldSize / 2, worldSize / 2);
            int y = random.Next(-worldSize / 2, worldSize / 2);

            // Create the powerup object
            Powerup powerup = new Powerup(powerupID, new Point2D(x, y), false);

            // Add the powerup to the world
            theWorld.Powerups[powerupID] = powerup;

            // Notify the server about the new powerup
            SendPowerupToServer(powerup);
        }

        private void SendPowerupToServer(Powerup powerup)
        {
            string powerupJson = JsonSerializer.Serialize(powerup);
            network.Send(powerupJson);
        }

        private void CollectPowerup(int powerupID, int playerID)
        {
            if (theWorld.Powerups.ContainsKey(powerupID))
            {
                theWorld.Powerups.Remove(powerupID);

                // Notify the server about the collection
                var interaction = new { playerID = playerID, powerupID = powerupID };
                string interactionJson = JsonSerializer.Serialize(interaction);
                network.Send(interactionJson);
            }
        }
    }
}