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
                Map.ParseMap();
                Algorithm.Run();
            }
            catch (Exceptions.ArgumentsException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Usage: \"Lem-in.exe MapFile\"");
                Environment.Exit(0);
            }
            catch (Exception ex) when (ex is Exceptions.MapException || ex is Exceptions.NoPathException)
            {
                Console.WriteLine("Error: " + ex.Message);
                Environment.Exit(0);
            }
            MapFile.Output(toDelete: true);
            Console.Write(Environment.NewLine);
            Solutions.ViewSolution(Solutions.CurrentSolution);
            Ants.Go();
        }
    }
}
