using System;
using System.Collections.Generic;
using System.Linq;

namespace Lem_in
{
    public static class Map
    {
        public static int AntsNumber { private set; get; }
        public static Dictionary<string, Room> Rooms { get; } = new Dictionary<string, Room>();
        public static Room StartRoom { private set; get; }
        public static Room EndRoom { private set; get; }
        private static bool EndFlag { set; get; }
        private static bool StartFlag { set; get; }
        public static List<Link> Links { get; } = new List<Link>();
        public static int LinksAmount;

        private static bool ParseRooms(string line)
        {
            if (!(line.Contains(' ')))
                return false;
            var roomsItems = line.Split(' ');
            if (roomsItems.Length != 3)
                throw new Exceptions.MapException($"Wrong arguments \"{line}\"");
            
            var name = roomsItems[0];

            if (name[0] == 'L')
                throw new Exceptions.MapException($"Wrong room name \"{name}\"");
            if (Rooms.ContainsKey(name))
                throw new Exceptions.MapException($"Duplicated room name \"{name}\"");

            if (!int.TryParse(roomsItems[1], out var x))
                throw new Exceptions.MapException($"Wrong coordinate x at room \"{line}\"");
            if (!int.TryParse(roomsItems[2], out var y))
                throw new Exceptions.MapException($"Wrong coordinate y at room \"{line}\"");

            var room = new Room(name, x, y);
            if (StartFlag)
            {
                if (StartRoom != null)
                    throw new Exceptions.MapException($"Duplicated start room");
                StartRoom = room;
                room.IsStart = true;
                StartFlag = false;
            }
            else if (EndFlag)
            {
                if (EndRoom != null)
                    throw new Exceptions.MapException($"Duplicated end room");
                EndRoom = room;
                room.IsEnd = true;
                EndFlag = false;
            }
            Rooms.Add(name, room);
            return true;
        }

        private static bool ParseLinks(string line)
        {
            if (!(line.Contains('-')))
                return false;

            var linksItems = line.Split('-');
            
            if (linksItems.Length != 2)
                throw new Exceptions.MapException($"Invalid link \"{line}\"");

            var room1Name = linksItems[0];
            var room2Name = linksItems[1];

            if (room1Name.Equals(room2Name))
                throw new Exceptions.MapException($"Link is not allowed to be a loop \"{line}\"");
            if (Rooms[room1Name].Links.Any(link => link.GetNeighbor(Rooms[room1Name]) == Rooms[room2Name]))
                throw new Exceptions.MapException($"Duplicated link \"{line}\"");

            Links.Add(new Link(Rooms[room1Name], Rooms[room2Name]));
            ++LinksAmount;
            return true;
        }

        private static bool IsSharp(string line)
        {
            if (line.Equals("##start"))
                StartFlag = true;
            else if (line.Equals("##end"))
                EndFlag = true;
            return line[0] == '#';
        }

        private static bool GetAntsNumber(string line)
        {
            if (!int.TryParse(line, out var antsNumber))
                throw new Exceptions.MapException($"Invalid ants number \"{line}\"");
            AntsNumber = antsNumber;
            return false;
        }

        private static bool ParseErrorFunc(string line)
        {
            throw new Exceptions.MapException($"Invalid end of file \"{line}\"");
        }

        public static void ParseMap()
        {
            byte curOp = 0;
            var opsList = new List<Func<string, bool>>()
            {
                GetAntsNumber,
                ParseRooms,
                ParseLinks,
                ParseErrorFunc
            };
            foreach(var line in MapFile.FileStrings)
            {
                if (string.IsNullOrEmpty(line))
                    throw new Exceptions.MapException("Empty string");
                if (IsSharp(line))
                    continue;
                if (!opsList[curOp](line))
                    opsList[++curOp](line);
            }
            if (StartRoom == null || EndRoom == null)
                throw new Exceptions.MapException("No start or end room in map");
        }
    }
}
