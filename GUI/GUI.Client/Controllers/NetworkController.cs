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

        /// <summary>
        /// Event triggered when data is received from the server.
        /// </summary>
        public event Action<string> DataReceived;

        private World theWorld;

        private bool receivedID = false;
        private bool receivedSize = false;

        private int worldSize;
        private int playerID;

        /// <summary>
        ///     TODO: XML COMMENT.
        /// </summary>
        /// <param name="connection"></param>
        public NetworkController(NetworkConnection connection)
        {
            this.network = connection;
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

                    if (!string.IsNullOrEmpty(message))
                    {
                        // Trigger the DataReceived event to notify subscribers of the new data
                        DataReceived?.Invoke(message);
                    }

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

                        // Create a new Snake for the player and set its ID
                        Snake userSnake = new Snake();
                        userSnake.SnakeID = playerID;

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

            // Deserialize the JSON message into a dictionary of objects
            var gameData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonMessage);

            if (gameData.ContainsKey("wall"))
            {
                // Extract wall ID
                int wallID = gameData["wall"].GetInt32();

                // Extract p1 and p2 coordinates
                int p1X = gameData["p1"].GetProperty("X").GetInt32();
                int p1Y = gameData["p1"].GetProperty("Y").GetInt32();
                int p2X = gameData["p2"].GetProperty("X").GetInt32();
                int p2Y = gameData["p2"].GetProperty("Y").GetInt32();

                // Create Point2D objects for p1 and p2
                Point2D p1 = new Point2D(p1X, p1Y);
                Point2D p2 = new Point2D(p2X, p2Y);

                // Create the Wall object
                Wall wall = new Wall(wallID, p1, p2);

                // Add or update the wall in theWorld.Walls
                theWorld.Walls[wallID] = wall;

                // Print statements for debugging
                Console.WriteLine($"Wall ID: {wall.WallID}");
                Console.WriteLine($"P1: X={wall.P1.X}, Y={wall.P1.Y}");
                Console.WriteLine($"P2: X={wall.P2.X}, Y={wall.P2.Y}");
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