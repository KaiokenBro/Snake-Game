using System.Text.Json.Serialization;

namespace GUI.Client.Models
{
    /// <summary>
    ///     TODO: XML COMMENT.
    /// </summary>
    public class Powerup
    {
        /// <summary>
        ///     Unique ID for the powerup.
        /// </summary>
        [JsonInclude]
        public int power { get; set; }

        /// <summary>
        ///     Location of the powerup.
        /// </summary>
        [JsonInclude]
        public Point2D loc { get; set; }

        /// <summary>
        ///     True if the powerup was collected (died) on this frame.
        /// </summary>
        [JsonInclude]
        public bool died { get; set; }

        /// <summary>
        ///     Default constructor for JSON deserialization.
        /// </summary>
        public Powerup()
        {
            loc = new Point2D();
        }
    }
}