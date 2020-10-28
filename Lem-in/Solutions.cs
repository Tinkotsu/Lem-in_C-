using System;
using System.Collections.Generic;
using System.Linq;

namespace Lem_in
{
    public static class Solutions
    {
        public static int SaveLabel { set; get; } = 1;
        public static List<List<Room>> CurrentSolution { private set; get; } = new List<List<Room>>();
        public static List<List<Room>> NextSolution { private set; get; } = new List<List<Room>>();
        public static int[] AntsDistribution { private set; get; } = new int[] {Map.AntsNumber};

        public static List<Room> GetPath(bool first = false)
        {
            var path = new List<Room>();

            if (first)
            {
                for (var room = Map.EndRoom; !room.IsStart; room = room.CameFrom.GetNeighbor(room))
                    path.Add(room);
                path.Add(Map.StartRoom);
                path.Reverse();
            }
            else
            {
                Algorithm.Bfs(Map.EndRoom, Map.StartRoom, SaveLabel);
                for (var room = Map.StartRoom; !room.IsEnd;)
                {
                    path.Add(room);
                    var link = room.CameFrom;
                    link.SaveLabel = SaveLabel;
                    room = link.GetNeighbor(room);
                }
                path.Add(Map.EndRoom);
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

            CurrentSolution = NextSolution;
            NextSolution = new List<List<Room>>();

            return ret;
        }

        public static void ViewSolution(List<List<Room>> sol)
        {
            Console.WriteLine("PATHS AMOUNT: " + Solutions.CurrentSolution.Count);
            var i = 1;
            foreach (var pathOutput in sol.Select(path => path.Select(room => room.Name).ToList()))
            {
                Console.WriteLine($"path #{i} => " + string.Join('-', pathOutput) + $" [len = {pathOutput.Count - 1}]");
                i++;
            }
            Console.WriteLine();
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
