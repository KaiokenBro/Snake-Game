using System.Numerics;

namespace GUI.Client.Models
{
    /// <summary>
    ///     TODO: XML COMMENT.
    /// </summary>
    public class World
    {
        /// <summary>
        ///     Collection of all snakes, keyed by their IDs.
        /// </summary>
        public Dictionary<int, Snake> snakes { get; set; }

        /// <summary>
        ///     Collection of all walls, keyed by their IDs.
        /// </summary>
        public Dictionary<int, Wall> walls { get; set; }

        /// <summary>
        ///     Collection of all powerups, keyed by their IDs.
        /// </summary>
        public Dictionary<int, Powerup> powerups { get; set; }

        /// <summary>
        ///     Size of the world (width and height).
        /// </summary>
        public int worldSize { get; set; }

        /// <summary>
        ///     Default constructor for initialization.
        /// </summary>
        /// <param name="worldSize"></param>
        public World(int worldSize)
        {
            this.worldSize = worldSize;
            this.snakes = new Dictionary<int, Snake>();
            this.walls = new Dictionary<int, Wall>();
            this.powerups = new Dictionary<int, Powerup>();
        }

        /// <summary>
        ///     Method to add or update snake in the world.
        /// </summary>
        /// <param name="snake"></param>
        public void AddOrUpdateSnake(Snake snake)
        {
            this.snakes[snake.snake] = snake;
        }

        /// <summary>
        ///     Method to add or update wall in the world.
        /// </summary>
        /// <param name="wall"></param>
        public void AddOrUpdateWall(Wall wall)
        {
            this.walls[wall.wall] = wall;
        }

        /// <summary>
        ///     Method to add or update powerup in the world.
        /// </summary>
        /// <param name="powerup"></param>
        public void AddOrUpdatePowerup(Powerup powerup)
        {
            this.powerups[powerup.power] = powerup;
        }

        /// <summary>
        ///     Method to remove snake from the world.
        /// </summary>
        /// <param name="snakeID"></param>
        public void RemoveSnake(int snakeID)
        {
            this.snakes.Remove(snakeID);
        }

        /// <summary>
        ///     Method to remove powerup from the world.
        /// </summary>
        /// <param name="powerupID"></param>
        public void RemovePowerup(int powerupID)
        {
            this.powerups.Remove(powerupID);
        }
    }
}