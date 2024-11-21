// Name: Harrison Doppelt and Victor Valdez Landa
// Date: 11/20/2024

using System.Text.Json.Serialization;

namespace GUI.Client.Models
{
    /// <summary>
    ///     Represents a command to control the direction in which the player's snake should move.
    /// </summary>
    public class ControlCommand
    {
        /// <summary>
        ///     Property that gets or sets the direction in which the player wants the snake to move.
        ///     Possible values are "up", "left", "down", or "right".
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("moving")]
        public string Moving { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ControlCommand"/> class
        ///     with a specified direction.
        /// </summary>
        /// <param name="direction">The direction in which the player wants the snake to move.</param>
        public ControlCommand(string direction)
        {
            Moving = direction;
        }
    }
}