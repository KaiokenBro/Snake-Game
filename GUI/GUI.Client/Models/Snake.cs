// Name: Harrison Doppelt and Victor Valdez Landa
// Date: 11/20/2024

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
        public string? PlayerName { get; private set; }

        /// <summary>
        ///     Property that gets or sets the player's score, representing the number of powerups collected.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("score")]
        public int PlayerScore { get; private set; }

        /// <summary>
        ///     Property that gets or sets the player's max score, representing the max number of powerups collected in a single life.
        /// </summary>
        [JsonIgnore]
        public int PlayerMaxScore = 0;

        /// <summary>
        ///     Property that gets or sets a value indicating whether the player joined on this frame.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("join")]
        public bool PlayerJoined { get; private set; }

        /// <summary>
        ///     Property that gets or sets a value indicating whether the player disconnected on this frame.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("dc")]
        public bool PlayerDisconnected { get; private set; }

        /// <summary>
        ///     Property that gets or sets the unique ID of the snake.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("snake")]
        public int SnakeID { get; private set; }

        /// <summary>
        ///     Property that gets or sets the body of the snake as a list of <see cref="Point2D"/> representing the segments.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("body")]
        public List<Point2D> SnakeBody { get; private set; } = [];

        /// <summary>
        ///     Property that gets or sets the direction of the snake.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("dir")]
        public Point2D? SnakeDirection { get; private set; }

        /// <summary>
        ///     Property that gets or sets a value indicating whether the snake is alive.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("alive")]
        public bool SnakeAlive { get; private set; }

        /// <summary>
        ///     Property that gets or sets a value indicating whether the snake died on this frame.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("died")]
        public bool SnakeDied { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Snake"/> class.
        ///     Used for JSON deserialization.
        /// </summary>
        public Snake() { }

        /// <summary>
        ///     Updates the name of the player controlling the snake.
        /// </summary>
        /// <param name="playerName">The name of the player to be set.</param>
        public void SetPlayerName(string playerName)
        {
            PlayerName = playerName;
        }

        /// <summary>
        ///     Assigns a unique identifier to the snake.
        /// </summary>
        /// <param name="snakeID">The unique ID of the snake to be set.</param>
        public void SetSnakeID(int snakeID)
        {
            SnakeID = snakeID;
        }
    }
}