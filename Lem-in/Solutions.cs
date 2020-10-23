using System.Collections.Generic;

namespace Lem_in
{
    public static class Solutions
    {
        public static int SaveLabel { set; get; } = 1;
        public static List<LinkedList<Room>> CurrentSolution { private set; get; } = new List<LinkedList<Room>>();
        public static List<LinkedList<Room>> NextSolution { private set; get; } = new List<LinkedList<Room>>();
        public static int[] AntsDistribution { private set; get; }

        public static LinkedList<Room> GetPath(bool first = false)
        {
            var path = new LinkedList<Room>();

            if (first)
            {
                for (var room = Map.EndRoom; !room.IsStart; room = room.CameFrom[^1].GetNeighbor(room))
                    path.AddFirst(room);
                path.AddFirst(Map.StartRoom);
            }
            else
            {
                Algorithm.Bfs(Map.EndRoom, Map.StartRoom);
                for (var room = Map.StartRoom; !room.IsEnd; room = room.CameFrom[^1].GetNeighbor(room))
                    path.AddLast(room);
                path.AddLast(Map.EndRoom);
            }

            return path;
        }

        public static bool IsEnough()
        {
            var antsAmount = Map.AntsNumber;

            AntsDistribution = new int[NextSolution.Count];
            for (var i = 1; i < NextSolution.Count; i++)
            {
                var dif = NextSolution[i].Count - NextSolution[i - 1].Count;

                if (dif == 0)
                    continue;
                for (var j = i - 1; j >= 0; j--)
                {
                    AntsDistribution[j] += dif;
                    antsAmount -= dif;
                    if (antsAmount <= 0)
                        return true;
                }
            }

            var div = antsAmount / NextSolution.Count;
            var mod = antsAmount % NextSolution.Count;
            var ret = div == 0 || (div == 1 && mod == 0);

            if (div != 0)
            {
                for (var i = 0; i < NextSolution.Count; i++)
                    AntsDistribution[i] += div;
            }

            var iterator = 0;
            for (; mod > 0; mod--)
            {
                AntsDistribution[iterator]++;
                ++iterator;
            }

            Solutions.CurrentSolution = Solutions.NextSolution;
            Solutions.NextSolution = new List<LinkedList<Room>>();

            return ret;
        }

        public static void SaveSolution(int pathsAmount, bool first = false)
        {
            SaveLabel++;
            if (first)
            {
                CurrentSolution.Add(GetPath(true));
                return;
            }
            for (var pathIndex = 0; pathIndex < pathsAmount; pathIndex++)
                NextSolution.Add(GetPath());
        }
    }
}
