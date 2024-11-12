using System.Text.Json.Serialization;

namespace GUI.Client.Models
{
    /// <summary>
    ///     A class that represents a 2D point in space (just an X and Y pair).
    /// </summary>
    public class Point2D
    {
        /// <summary>
        ///     The X-coordinate.
        /// </summary>
        [JsonInclude]
        public int X { get; set; }

        /// <summary>
        ///     The Y-coordinate.
        /// </summary>
        [JsonInclude]
        public int Y { get; set; }

        /// <summary>
        ///     Constructor to create a Point2D with specific coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point2D(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        ///     Default constructor for JSON deserialization
        /// </summary>
        public Point2D()
        {
        }
    }
}
