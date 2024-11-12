using System.Numerics;

namespace GUI.Client.Models
{
    public class World
    {

        public Dictionary<int, Snake> Snakes;

        public Dictionary<int, Food> Foods;

        public int Size 
        { get; private set; }

        public World(int _size)
        {
            Snakes = new Dictionary<int, Snake>();
            Foods = new Dictionary<int, Food>();
            Size = _size;
        }

        public World(World world)
        {
            Snakes = new(world.Snakes);
            Foods = new(world.Foods);
            Size = world.Size;
        }

    }
}
