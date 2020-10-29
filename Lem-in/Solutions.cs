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
        public static int[] NextSolAntsDis { private set; get; } = new int[] {Map.AntsNumber};
        public static int[] CurSolAntsDis { private set; get; } = new int[] { Map.AntsNumber };
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

            NextSolAntsDis = new int[NextSolution.Count];
            for (var i = 1; i < NextSolution.Count; i++)
            {
                var dif = NextSolution[i].Count - NextSolution[i - 1].Count;

                if (dif == 0)
                    continue;
                for (var j = i - 1; j >= 0; j--)
                {
                    NextSolAntsDis[j] += dif;
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
                    NextSolAntsDis[i] += div;
            }

            var iterator = 0;
            for (; mod > 0; mod--)
            {
                NextSolAntsDis[iterator]++;
                ++iterator;
            }

            CurrentSolution = NextSolution;
            CurSolAntsDis = NextSolAntsDis;
            NextSolution = new List<List<Room>>();

            return ret;
        }

        public static void ViewSolution(List<List<Room>> sol)
        {
            Console.WriteLine("PATHS AMOUNT: " + sol.Count + Environment.NewLine);
            for (var i = 0; i < sol.Count; i++)
            {
                Console.WriteLine($"path #{i + 1}: |" 
                                  + $" len = {sol[i].Count - 1}"
                                  + $" | ants = {CurSolAntsDis[i]} |\n\t["
                                  + string.Join(" -> ", sol[i].Select(room => room.Name).ToList())
                                  + "]\n");
            }
        }

        public static void SaveSolution(int pathsAmount, bool first = false)
        {
            SaveLabel++;
            if (first)
            {
                CurrentSolution.Add(GetPath(true));
                CurSolAntsDis[0] = Map.AntsNumber;
                return;
            }
            for (var pathIndex = 0; pathIndex < pathsAmount; pathIndex++)
                NextSolution.Add(GetPath());
        }
    }
}
