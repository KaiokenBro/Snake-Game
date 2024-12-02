// Name: Harrison Doppelt and Victor Valdez Landa
// Date: 11/20/2024

using System.Text.Json.Serialization;

namespace GUI.Client.Models
{
    /// <summary>
    ///     A class that represents a 2D point in space with X and Y coordinates.
    /// </summary>
    public class Point2D
    {
        /// <summary>
        ///     Property that gets or sets the X-coordinate of the point.
        /// </summary>
        [JsonInclude]
        public int X { get; private set; }

        /// <summary>
        ///     Property that gets or sets the Y-coordinate of the point.
        /// </summary>
        [JsonInclude]
        public int Y { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point2D"/> class with specified X and Y coordinates.
        /// </summary>
        /// <param name="x">The X-coordinate of the point.</param>
        /// <param name="y">The Y-coordinate of the point.</param>
        public Point2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        ///     Constructor that Initializes a new instance of the <see cref="Point2D"/> class.
        ///     Used for JSON deserialization.
        /// </summary>
        public Point2D() { }
    }
}