using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lem_in
{
    public static class Algorithm
    {
        private static int CurrentSearchCounter = 0;
        private static int MaxPaths { set; get; }

        private static bool Bfs(Room startRoom, Room roomToSearch)
        {
            var q = new Queue<Room>();
            var visited = new HashSet<Room>();

            q.Enqueue(startRoom);
            while (q.Count > 0)
            {
                var room = q.Dequeue();
                if (room == roomToSearch)
                    return true;
                visited.Add(room);
                room.Links
                    .Where(link => !visited.Contains(link.GetNeighbor(room)))
                    .ToList()
                    .ForEach(link =>
                    {
                        var nei = link.GetNeighbor(room);
                        if (nei.CameFrom.Count == 0)
                            nei.CameFrom.Add(link);
                        else
                            nei.CameFrom[0] = link;
                        q.Enqueue(nei);
                    });
            }
            return false;
        }

        private static bool BellmanFord()
        {
            var q = new Queue<ArrayList>();
            var visited = new HashSet<Room>();

            Map.StartRoom.Links
                .Where(link => link.IsNeiAccessible(Map.StartRoom))
                .ToList()
                .ForEach(link => q.Enqueue(new ArrayList() {Map.StartRoom, link}));
            while (q.Count > 0)
            {

            }
            return true;
        }

        private static int GetMaxPaths()
        {
            return Math.Min(Map.StartRoom.Links.Count(), Map.EndRoom.Links.Count());
        }

        private static void ChangeMinPath()
        {
            var room = Map.EndRoom;
            while (room != Map.StartRoom)
            {
                var link = room.CameFrom[CurrentSearchCounter];
                
                room.StepsLabel = -1;
                if (link.Weight == -1)
                    link.Disabled = true;
                else
                {
                    link.DenyAccessToRoom(room);
                    link.Weight = -1;
                }
                room = link.GetNeighbor(room);
            }
        }

        private static void Suurballe()
        {
            for (var curPathsCount = 2; curPathsCount < MaxPaths; curPathsCount++)
            {
                ChangeMinPath();
                if (!BellmanFord())
                    break;
                Solutions.SaveSolution(curPathsCount);
                Solutions.CheckSolutions();
                if (Solutions.IsEnough())
                    break;
            }
        }
        public static void Run()
        {
            MaxPaths = GetMaxPaths();
            var res = Bfs(Map.StartRoom, Map.EndRoom);
            if (MaxPaths == 0 || !res)
                throw new Exceptions.NoPathException();
            Solutions.SaveSolution(1, true);
            if (Map.AntsNumber == 1)
                return;
            //Suurballe();
        }
    }

}