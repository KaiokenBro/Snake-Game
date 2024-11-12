using System.Text.Json.Serialization;

namespace GUI.Client.Models
{
    /// <summary>
    /// A class that represents a 2D point in space (just an X and Y pair).
    /// </summary>
    public class Point2D
    {
        /// <summary>
        /// X-coordinate.
        /// </summary>
        [JsonInclude]
        public int X { get; private set; }

        /// <summary>
        /// Y-coordinate.
        /// </summary>
        [JsonInclude]
        public int Y { get; private set; }

        /// <summary>
        /// Constructor to initiate a Point2D object.
        /// </summary>
        /// <param name="X">X coordinate of the object</param>
        /// <param name="Y">Y coordinate of the object</param>
        public Point2D(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// Empty constructor for Point2D.
        /// </summary>
        public Point2D()
        {
           //worry when need to use Json 
        }

    }
}
