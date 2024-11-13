using GUI.Client.Models;
using System.Data.Common;
using System.Text.Json;

namespace GUI.Client.Controllers
{
    public class NetworkController
    {
        private readonly NetworkConnection connection;

        // Event to notify when a message is received
        public event Action<string> MessageReceived;

        public NetworkController()
        {
            connection = new NetworkConnection();
        }

        // Method to send a movement command
        //public void SendMovementCommand(string command)
        //{
            //if (connection.IsConnected)
            //{
                //try
                //{
                    // JSON format for the movement command
                    //string jsonCommand = JsonSerializer.Serialize(new { moving = command });
                    //connection.Send(jsonCommand);
                //}
                //catch (Exception ex)
                //{
                    //Console.WriteLine($"Failed to send movement command: {ex.Message}");
                //}
            //}
        //}

        // Method to start listening for incoming messages
        //private async void StartListening()
        //{
            //await Task.Run(() =>
            //{
                //while (connection.IsConnected)
                //{
                    //try
                    //{
                        //string message = connection.ReadLine();
                        //if (message != null)
                        //{
                            // Trigger the event to notify the message is received
                            //MessageReceived?.Invoke(message);
                        //}
                    //}
                    //catch (Exception ex)
                    //{
                        //Console.WriteLine($"Error reading message: {ex.Message}");
                        //Disconnect();
                        //break;
                    //}
                //}
            //});
        //}

        // Method to handle user input (WASD for movement)
        //public void HandleUserInput(string direction)
        //{
            //string command = direction switch
            //{
                //"W" => "up",
                //"A" => "left",
                //"S" => "down",
                //"D" => "right",
            //};

            //if (command != null && connection != null && connection.IsConnected)
            //{
                //connection.Send(command);
            //}
        //}

    }
}