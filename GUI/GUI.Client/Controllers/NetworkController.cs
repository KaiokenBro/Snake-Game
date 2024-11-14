using GUI.Client.Models;
using System.Data.Common;
using System.Text.Json;

namespace GUI.Client.Controllers
{
    public class NetworkController
    {
        private NetworkConnection network;

        public NetworkController(NetworkConnection network)
        {
            this.network = network;
        }

        // Sends direction to the server
        public void SendCommand(ControlCommand command)
        {
            // Serialize command to JSON format
            string commandJson = JsonSerializer.Serialize(command);

            // Send the JSON command to the server
            network.Send(commandJson);
        }

    }
}