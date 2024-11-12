namespace GUI.Client.Models
{
    public class Wall
    {

        // An int representing the wall's unique ID
        public int wall;

        // A Point2D representing one endpoint of the wall
        public Point2D p1;

        // A Point2D representing the other endpoint of the wall
        public Point2D p2;

        public Wall(int id, Point2D start, Point2D end)
        {
            wall = id;
            p1 = start;
            p2 = end;
        }

    }
}
