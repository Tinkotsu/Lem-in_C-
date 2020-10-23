using System;

namespace Lem_in
{
    public class Ants
    {
        public static void CheckTwoRoomsPath()
        {
            if (!(Solutions.CurrentSolution.Count == 1 && Solutions.CurrentSolution[0].Count == 2))
                return;
            for (var i = 1; i < Map.AntsNumber; i++)
            {
                Console.Write($"L{i}-{Map.EndRoom.Name}");
                if (i < Map.AntsNumber)
                    Console.Write(' ');
            }
            Console.Write(Environment.NewLine);
        }

        public static void Go()
        {
            CheckTwoRoomsPath();

        }
    }
}
