using System.Text.Json.Serialization;

namespace GUI.Client.Models
{
    /// <summary>
    ///     TODO: XML COMMENT
    /// </summary>
    public class Snake
    {
        /// <summary>
        ///     Unique ID of the snake.
        /// </summary>
        [JsonInclude]
        public int snake { get; set; }

        /// <summary>
        ///     Players name.
        /// </summary>
        [JsonInclude]
        public string name { get; set; }

        /// <summary>
        ///     A list of Point2D representing the body segments.
        /// </summary>
        [JsonInclude]
        public List<Point2D> body { get; set; }

        /// <summary>
        ///     Property for the Snakes orientation.
        /// </summary>
        [JsonInclude]
        public Point2D dir { get; set; }

        /// <summary>
        ///     Players score (Number of powerups collected).
        /// </summary>
        [JsonInclude]
        public int score { get; set; }

        /// <summary>
        ///     True if the snake died on this frame.
        /// </summary>
        [JsonInclude]
        public bool died { get; set; }

        /// <summary>
        ///     True if the snake is alive.
        /// </summary>
        [JsonInclude]
        public bool alive { get; set; }

        /// <summary>
        ///     True if the player disconnected on this frame.
        /// </summary>
        [JsonInclude]
        public bool dc { get; set; }

        /// <summary>
        ///     True if the player joined on this frame.
        /// </summary>
        [JsonInclude]
        public bool join { get; set; }

        /// <summary>
        ///     Default constructor for JSON deserialization.
        /// </summary>
        public Snake()
        {
            body = new List<Point2D>();
        }
    }
}