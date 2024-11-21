/// Name: Harrison Doppelt and Victor Valdez Landa
/// Date: 11/20/2024

using System.Text.Json.Serialization;

namespace GUI.Client.Models
{
    /// <summary>
    ///     Represents a wall in the game, with two endpoints defining its position.
    /// </summary>
    public class Wall
    {
        /// <summary>
        ///     Property that gets or sets the unique ID of the wall.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("wall")]
        public int WallID { get; private set; }

        /// <summary>
        ///     Property that gets or sets the first endpoint of the wall.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("p1")]
        public Point2D P1 { get; private set; }

        /// <summary>
        ///     Property that gets or sets the second endpoint of the wall.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("p2")]
        public Point2D P2 { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Wall"/> class.
        ///     Used for JSON deserialization.
        /// </summary>
        public Wall() { }
    }
}