using System.Collections.Generic;
using System.Linq;

namespace Lem_in
{
    public class Link
    {
        private Dictionary<Room, bool> Rooms { set; get; }
        public int Weight { set; get; } = 1;
        public bool Disabled { set; get; } = false;
        public int SaveLabel { set; get; }

        public Link(Room room1, Room room2)
        {
            Rooms = new Dictionary<Room, bool>()
            {
                [room1] = true,
                [room2] = true
            };
            Map.Rooms[room1.Name].Links.Add(this);
            Map.Rooms[room2.Name].Links.Add(this);
        }

        public bool IsNeiAccessible(Room room) => Rooms[GetNeighbor(room)];
        public bool IsRoomAccessible(Room room) => Rooms[room];
        public void DenyAccessToRoom(Room room) => Rooms[room] = false;
        public Room GetNeighbor(Room room)
        {
            var rooms = Rooms.Keys.ToList();
            return rooms[0] == room ? rooms[1] : rooms[0];
        }
    }
}
