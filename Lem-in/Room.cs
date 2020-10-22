using System.Collections.Generic;

namespace Lem_in
{
    public class Room
    {
        public string Name { private set; get; }
        public int X { private set; get; }
        public int Y { private set; get; }
        public bool IsStart { set; get; } = false;
        public bool IsEnd { set; get; } = false;
        public int StepsLabel { set; get; } = -1;
        public List<Link> Links { get; } = new List<Link>();
        public List<Link> CameFrom { set; get; } = new List<Link>();
        public Room(string name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
        }
    }
}
