using System.Drawing;
using System.Numerics;

namespace GUI.Client.Models
{
    /// <summary>
    ///     Represents the game world containing snakes, walls, and powerups.
    /// </summary>
    public class World
    {
        /// <summary>
        ///     Collection of all snakes in the world, keyed by their unique IDs.
        /// </summary>
        public Dictionary<int, Snake> 
Snakes { get; set; }
        /// <summary>
        ///     Collection of all walls in the world, keyed by their unique IDs.
        /// </summary>
        public Dictionary<int, Wall> Walls { get; set; }

        /// <summary>
        ///     Collection of all powerups in the world, keyed by their unique IDs.
        /// </summary>
        public Dictionary<int, Powerup> Powerups { get; set; }

        /// <summary>
        ///     Property that gets or sets the size of the world (width and height).
        /// </summary>
        public int WorldSize { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="World"/> class with the specified world size.
        /// </summary>
        /// <param name="worldSize">The size of the world.</param>
        public World(int worldSize)
        {
            this.WorldSize = worldSize;
            this.Snakes = new Dictionary<int, Snake>();
            this.Walls = new Dictionary<int, Wall>();
            this.Powerups = new Dictionary<int, Powerup>();
        }

        /// <summary>
        ///     Shallow copy constructor.
        /// </summary>
        /// <param name="world"></param>
        public World(World world)
        {
            Snakes = new(world.Snakes);
            Powerups = new(world.Powerups);
            WorldSize = world.WorldSize;
        }

        /// <summary>
        ///     Method to add or update snake in the world.
        /// </summary>
        /// <param name="snake">The snake to add or update.</param>
        public void AddOrUpdateSnake(Snake snake)
        {
            this.Snakes[snake.SnakeID] = snake;
        }

        /// <summary>
        ///     Method to remove snake from the world.
        /// </summary>
        /// <param name="snakeID">The ID of the snake to remove.</param>
        public void RemoveSnake(int snakeID)
        {
            this.Snakes.Remove(snakeID);
        }

        /// <summary>
        ///     Method to add or update powerup in the world.
        /// </summary>
        /// <param name="powerup">The powerup to add or update.</param>
        public void AddOrUpdatePowerup(Powerup powerup)
        {
            this.Powerups[powerup.PowerupID] = powerup;
        }

        /// <summary>
        ///     Method to remove powerup from the world.
        /// </summary>
        /// <param name="powerupID">The ID of the powerup to remove.</param>
        public void RemovePowerup(int powerupID)
        {
            this.Powerups.Remove(powerupID);
        }

        /// <summary>
        ///     Method to add or update wall in the world.
        /// </summary>
        /// <param name="wall">The wall to add or update.</param>
        public void AddOrUpdateWall(Wall wall)
        {
            this.Walls[wall.WallID] = wall;
        }
    }
}