using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace Lem_in
{
    public static class Solutions
    {
        public static int SaveLabel { set; get; }
        public static List<LinkedList<Room>> CurrentSolution { private set; get; } = new List<LinkedList<Room>>();
        public static List<LinkedList<Room>> NextSolution { private set; get; } = new List<LinkedList<Room>>();

        public static LinkedList<Room> GetPath(int pathIndex)
        {
            var room = Map.EndRoom;
            var path = new LinkedList<Room>();

            while (room != Map.StartRoom)
            {
                path.AddFirst(room);
                var checkedLink = room.CameFrom[pathIndex].Disabled
                    ? room.CameFrom.Find(link => link.Weight == -1 && link.SaveLabel != SaveLabel)
                    : room.CameFrom[pathIndex];
                checkedLink.SaveLabel = SaveLabel;
                room = checkedLink.GetNeighbor(room);
            }
            path.AddFirst(Map.StartRoom);
            return path;
        }

        public static void SaveSolution(int pathsAmount, bool first = false)
        {
            if (first)
            {
                CurrentSolution.Add(GetPath(0));
                return;
            }
            for (var pathIndex = 0; pathIndex < pathsAmount; pathIndex++)
                NextSolution.Add(GetPath(pathIndex));
        }
    }
}
