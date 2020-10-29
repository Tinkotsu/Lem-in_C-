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
        public int VisitLabel { set; get; } = -1;
        public int VisitCounter { set; get; }
        public List<Link> Links { get; } = new List<Link>();
        public Link CameFrom { set; get; }
        public Room(string name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
        }
    }
}
