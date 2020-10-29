using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace Lem_in
{
    internal static class MapFile
    {
        public static List<string> FileStrings { private set; get; } = new List<string>();

        public static void ReadMap(string[] args)
        {
            if (args.Length == 0)
            {
                while (true)
                {
                    var input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input))
                        break;
                    FileStrings.Add(input);
                }

                return;
            }
            if (args.Length != 1)
            {
                throw new Exceptions.ArgumentsException();
            }
            try
            {
                FileStrings = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "maps", args[0])).ToList();
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
