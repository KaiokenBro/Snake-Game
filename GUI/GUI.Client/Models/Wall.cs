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
        public int WallID { get; set; }

        /// <summary>
        ///     Property that gets or sets the first endpoint of the wall.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("p1")]
        public Point2D P1 { get; set; }

        /// <summary>
        ///     Property that gets or sets the second endpoint of the wall.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("p2")]
        public Point2D P2 { get; set; }

        // Default constructor for JSON deserialization
        public Wall() { }
    }
}