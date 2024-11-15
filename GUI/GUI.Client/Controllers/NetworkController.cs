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

        private bool receivedIdAndWorldSize = false;
        private int? playerId = null;
        private int worldSize = 0;

        private Snake userSnake;

        private World theWorld;

        /// <summary>
        ///     TODO: XML COMMENT.
        /// </summary>
        /// <param name="network"></param>
        public NetworkController(NetworkConnection connection)
        {
            this.network = connection;
        }

        /// <summary>
        ///     Method to start listening for data from the server.
        /// </summary>
        public async Task ReceiveFromServerAsync()
        {
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

            // If we haven't received the ID and world size yet
            if (!receivedIdAndWorldSize)
            {
                // First message should be the player ID
                if (playerId == null)
                {
                    if (int.TryParse(message, out int id))
                    {
                        userSnake = new Snake();
                        userSnake.SnakeID = id;
                        Console.WriteLine("SHIT");
                    }
                }
                // Second message should be the world size
                else if (worldSize == 0)
                {
                    if (int.TryParse(message, out int size))
                    {
                        worldSize = size;
                        theWorld = new World(worldSize);
                        receivedIdAndWorldSize = true;
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
            Wall wall = JsonSerializer.Deserialize<Wall>(jsonMessage);
            theWorld.AddOrUpdateWall(wall);
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