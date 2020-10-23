using System;
using System.IO;

namespace Lem_in
{
    internal static class MapFile
    {
        public static string[] FileStrings { private set; get; }

        public static void ReadMapFile(string[] args)
        {
            if (args.Length != 1)
            {
                throw new Exceptions.ArgumentsException();
            }
            try
            {
                FileStrings = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "maps", args[0]));
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Environment.Exit(0);
            }
        }
        public static void Output(bool toDelete = false)
        {
            Console.WriteLine(string.Join(Environment.NewLine, FileStrings));
            if (toDelete)
                FileStrings = null;
        }
    }
}
