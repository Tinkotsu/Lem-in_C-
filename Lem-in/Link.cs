using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lem_in
{
    public class Link
    {
        private Dictionary<Room, bool> Rooms { set; get; }
        public int Weight { set; get; } = 1;
        public int SaveLabel { set; get; }

        public Link(Room room1, Room room2)
        {
            Rooms = new Dictionary<Room, bool>()
            {
                [room1] = true,
                [room2] = true
            };
            room1.Links.Add(this);
            room2.Links.Add(this);
        }
        public bool IsNeiAccessible(Room room) => Rooms[GetNeighbor(room)];
        public bool IsRoomAccessible(Room room) => Rooms[room];
        public void DenyAccessToRoom(Room room) => Rooms[room] = false;
        public void Enable()
        {
            Weight = 1;
            var rooms = Rooms.Keys.ToList();
            Rooms[rooms[0]] = true;
            Rooms[rooms[1]] = true;
        }
        public Room GetNeighbor(Room room)
        {
            var rooms = Rooms.Keys.ToList();
            return rooms[0] == room ? rooms[1] : rooms[0];
        }
    }
}
