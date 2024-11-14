using System.Text.Json.Serialization;

namespace GUI.Client.Models
{
    public class ControlCommand
    {
        // The direction in which the player wants the snake to move
        // Possible values: "none", "up", "left", "down", "right"
        [JsonInclude]
        [JsonPropertyName("moving")]
        public string Moving { get; set; }

        // Default constructor required for JSON deserialization
        public ControlCommand() { }

        // Constructor to initialize the direction
        public ControlCommand(string direction)
        {
            Moving = direction;
        }
    }
}