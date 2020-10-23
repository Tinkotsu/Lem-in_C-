using System;

namespace Lem_in
{
    internal static class Lemin
    {
        private static void Main(string[] args)
        {
            try
            {
                MapFile.ReadMapFile(args);
            }
            catch (Exceptions.ArgumentsException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Usage: \"Lem-in.exe MapFile\"");
                Environment.Exit(0);
            }

            try
            {
                Map.ParseMap();
            }
            catch (Exceptions.MapException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Environment.Exit(0);
            }
            try
            {
                Algorithm.Run();
            }
            catch (Exceptions.NoPathException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Environment.Exit(0);
            }
            MapFile.Output(toDelete: true);
            Console.Write(Environment.NewLine);
            Ants.Go();
        }
    }
}
