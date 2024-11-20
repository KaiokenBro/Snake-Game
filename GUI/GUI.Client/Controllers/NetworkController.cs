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
        private bool commandSentThisFrame = false;

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
                    string message = await Task.Run(() => network.ReadLine());

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

        public void HandleServerData(string message)
        {
            if (!receivedID || !receivedSize)
            {
                if (!receivedID)
                {
                    if (int.TryParse(message, out int parsedPlayerID))
                    {
                        playerID = parsedPlayerID;
                        receivedID = true;
                    }
                }
                else if (!receivedSize)
                {
                    if (int.TryParse(message, out int parsedWorldSize))
                    {
                        worldSize = parsedWorldSize;
                        receivedSize = true;

                        theWorld = new World(worldSize);
                        Snake userSnake = new Snake();
                        userSnake.SnakeID = playerID;
                        userSnake.PlayerName = playerName;

                        lock (theWorld)
                        {
                            theWorld.Snakes[playerID] = userSnake;
                        }
                    }
                }
            }

            else
            {
                ParseJsonData(message);
            }
        }

        private void ParseJsonData(string jsonMessage)
        {
            try
            {
                if (jsonMessage.Contains("\"wall\""))
                {
                    Wall? wall = JsonSerializer.Deserialize<Wall>(jsonMessage);

                    lock (theWorld)
                    {
                        theWorld.Walls[wall.WallID] = wall;
                    }
                }

                if (jsonMessage.Contains("\"power\""))
                {
                    Powerup? powerup = JsonSerializer.Deserialize<Powerup>(jsonMessage);

                    if (powerup.PowerupDied)
                    {
                        lock (theWorld)
                        {
                            theWorld.Powerups.Remove(powerup.PowerupID);
                        }
                    }
                    else if (!powerup.PowerupDied)
                    {
                        lock (theWorld)
                        {
                            theWorld.Powerups[powerup.PowerupID] = powerup;
                        }
                    }
                }

                if (jsonMessage.Contains("\"snake\""))
                {
                    Snake? snake = JsonSerializer.Deserialize<Snake>(jsonMessage);

                    if (snake.PlayerDisconnected)
                    {
                        lock (theWorld)
                        {
                            theWorld.Snakes.Remove(snake.SnakeID);
                        }
                    }
                    else if (snake != null)
                    {
                        lock (theWorld)
                        {
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

            if (commandSentThisFrame)
            {
                return;
            }

            string direction = key switch
            {
                "w" => "up",
                "a" => "left",
                "s" => "down",
                "d" => "right"
            };

            if (direction != null)
            {
                ControlCommand command = new ControlCommand(direction);
                SendCommand(command);
                commandSentThisFrame = true;
            }
        }

        private void SendCommand(ControlCommand command)
        {
            string commandJson = JsonSerializer.Serialize(command);
            network.Send(commandJson);
        }

        // Reset the commandSentThisFrame flag at the start of the next frame (e.g., in the game loop)
        public void ResetCommandFlag()
        {
            commandSentThisFrame = false;
        }

    }
}