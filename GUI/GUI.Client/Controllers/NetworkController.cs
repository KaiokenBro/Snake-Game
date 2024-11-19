using GUI.Client.Models;
using System.Data.Common;
using System.Text.Json;

namespace GUI.Client.Controllers
{

    public class NetworkController
    {

        private NetworkConnection network;
        public World theWorld { get; set; }
        private bool receivedID = false;
        private bool receivedSize = false;
        private int worldSize;
        public int playerID;
        private string playerName;

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
                    Wall? wall = JsonSerializer.Deserialize<Wall>(jsonMessage);


                    /////////////////
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

                    /////
                    lock (theWorld)
                    {
                        // Add the power-up in the world's dictionary
                        theWorld.Powerups[powerup.PowerupID] = powerup;
                    }
                }

                // Check if the JSON is a snake
                if (jsonMessage.Contains("\"snake\""))
                {
                    // Deserialize the JSON message directly into a snake object
                    Snake? snake = JsonSerializer.Deserialize<Snake>(jsonMessage);

                    if (snake != null)
                    {

                        ////////////////////
                        lock (theWorld)
                        {
                            if (!theWorld.Snakes.ContainsKey(snake.SnakeID))
                            {
                                Console.WriteLine($"New snake received: ID={snake.SnakeID}, Name={snake.PlayerName}");
                            }
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

        private void SendCommand(ControlCommand command)
        {
            // Serialize command to JSON format
            string commandJson = JsonSerializer.Serialize(command);

            // Send the JSON command to the server
            network.Send(commandJson);
        }

    }
}