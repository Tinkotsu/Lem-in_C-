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

        private static void FillPaths()
        {
            foreach (var path in Solutions.CurrentSolution)
            {
                AntsToGo--;
                AntsList.Add(new Ant(Map.AntsNumber - AntsToGo, path));
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
                    FillPaths();
                AntsList.Where(ant => !ant.Passed).ToList().ForEach(ant => ant.Move());
                Output();
            }
        }
    }
}
