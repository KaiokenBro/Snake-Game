using System.Text.Json.Serialization;

namespace GUI.Client.Models
{
    /// <summary>
    ///     Represents a snake in the game, controlled by a player.
    /// </summary>
    public class Snake
    {
        /// <summary>
        ///     Property that gets or sets the player's name.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("name")]
        public string PlayerName { get; set; }

        /// <summary>
        ///     Property that gets or sets the player's score, representing the number of powerups collected.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("score")]
        public int PlayerScore { get; set; }

        /// <summary>
        ///     Property that gets or sets a value indicating whether the player joined on this frame.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("join")]
        public bool PlayerJoined { get; set; }

        /// <summary>
        ///     Property that gets or sets a value indicating whether the player disconnected on this frame.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("dc")]
        public bool PlayerDisconnected { get; set; }

        /// <summary>
        ///     Property that gets or sets the unique ID of the snake.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("snake")]
        public int SnakeID { get; set; }

        /// <summary>
        ///     Property that gets or sets the body of the snake as a list of <see cref="Point2D"/> representing the segments.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("body")]
        public List<Point2D> SnakeBody { get; set; }

        /// <summary>
        ///     Property that gets or sets the direction of the snake.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("dir")]
        public Point2D SnakeDirection { get; set; }

        /// <summary>
        ///     Property that gets or sets a value indicating whether the snake is alive.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("alive")]
        public bool SnakeAlive { get; set; }

        /// <summary>
        ///     Property that gets or sets a value indicating whether the snake died on this frame.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("died")]
        public bool SnakeDied { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Snake"/> class.
        ///     Used for JSON deserialization.
        /// </summary>
        public Snake()
        {
            SnakeBody = new List<Point2D>();
        }
    }
}