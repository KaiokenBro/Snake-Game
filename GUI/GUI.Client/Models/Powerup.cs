// Name: Harrison Doppelt and Victor Valdez Landa
// Date: 11/20/2024

using System.Text.Json.Serialization;

namespace GUI.Client.Models
{
    /// <summary>
    ///     Represents a powerup in the game, which can be collected by players.
    /// </summary>
    public class Powerup
    {
        /// <summary>
        ///     Property that gets or sets the unique ID for the powerup.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("power")]
        public int PowerupID { get; private set; }

        /// <summary>
        ///     Property that gets or sets the location of the powerup in the world.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("loc")]
        public Point2D? PowerupLocation { get; private set; }

        /// <summary>
        ///     Property that gets or sets a value indicating whether the powerup was collected (died) on this frame.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("died")]
        public bool PowerupDied { get; private set; }

        /// <summary>
        ///     Constructor that Initializes a new instance of the <see cref="Powerup"/> class.
        ///     Used for JSON deserialization.
        /// </summary>
        public Powerup() { }
    }
}