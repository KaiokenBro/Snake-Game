using System.Text.Json.Serialization;

namespace GUI.Client.Models
{

    /// <summary>
    ///     Represents a command to control the direction in which the player's snake should move.
    /// </summary>
    /// <param name="direction">
    ///     The direction in which the player wants the snake to move.
    ///     Possible values are "up", "left", "down", or "right".
    /// </param>
    public class ControlCommand(string direction)
    {

        /// <summary>
        ///     Property that gets or sets the direction in which the player wants the snake to move.
        ///     Possible values are "up", "left", "down", or "right".
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("moving")]
        public string Moving { get; private set; } = direction;
    }
}