using System;
using System.Collections.Generic;
using System.Linq;

namespace Lem_in
{
    public class Ants
    {
        private static int AntsToGo { set; get; } = Map.AntsNumber;
        private static int AntsPassed { set; get; }
        private static List<Ant> AntsList { set; get; } = new List<Ant>();

        private class Ant
        {
            private int RoomIndex { set; get; }
            private List<Room> Path { set; get; }
            public Room Room { private set; get; }
            public int Index { private set; get; }
            public bool Passed { private set; get; }

            public Ant(int index, List<Room> path)
            {
                Index = index;
                Path = path;
                RoomIndex = 0;
                Room = Path[0];
                Passed = false;
            }

            public void Move()
            {
                if (Room.IsEnd)
                {
                    ++AntsPassed;
                    Passed = true;
                }
                else
                    Room = Path[++RoomIndex];
            }

        }
        /*
        public static bool CheckTwoRoomsPath()
        {
            if (!(Solutions.CurrentSolution.Count == 1 && Solutions.CurrentSolution[0].Count == 2))
                return false;
            for (var i = 1; i < Map.AntsNumber; i++)
            {
                Console.Write($"L{i}-{Map.EndRoom.Name}");
                if (i < Map.AntsNumber)
                    Console.Write(' ');
            }
            Console.Write(Environment.NewLine);
            return true;
        }*/

        private static void FillPath()
        {
            for (var i = 0; i < Solutions.AntsDistribution.Length; i++)
            {
                Solutions.AntsDistribution[i]--;
                AntsToGo--;
                AntsList.Add(new Ant(Map.AntsNumber - AntsToGo, Solutions.CurrentSolution[i]));
                if (AntsToGo == 0)
                    break;
            }
        }

        private static void Output()
        {
            var output = new List<string>();
            
            AntsList.Where(ant => !ant.Passed).ToList().ForEach(ant => output.Add($"L{ant.Index}-{ant.Room.Name}"));
            Console.WriteLine(string.Join(' ', output));
        }

        public static void Go()
        {
            while (AntsPassed < Map.AntsNumber)
            {
                if (AntsToGo > 0)
                    FillPath();
                AntsList.Where(ant => !ant.Passed).ToList().ForEach(ant => ant.Move());
                Output();
            }
        }
    }
}
