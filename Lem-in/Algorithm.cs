using System;
using System.Collections.Generic;
using System.Linq;

namespace Lem_in
{
    public static class Algorithm
    {
        private static HashSet<Link> _disabledLinks = new HashSet<Link>();
        private static HashSet<Room> _doubledRooms = new HashSet<Room>();
        private static int _currentSearchCounter;
        private static int MaxPaths { set; get; }

        public class QueueItem
        {
            public Room Room { private set; get; }
            public Link Link { private set; get; }
            public bool PathBlock { private set; get; }

            public QueueItem(Room room, Link link, bool pathBlock = false)
            {
                Room = room;
                Link = link;
                PathBlock = pathBlock;
            }
        }

        public static bool Bfs(Room startRoom, Room roomToSearch, int saveLabel = 0)
        {
            var q = new Queue<Room>();
            var visited = new HashSet<Room>();

            q.Enqueue(startRoom);
            visited.Add(startRoom);
            while (q.Count > 0)
            {
                var room = q.Dequeue();
                if (room == roomToSearch)
                    return true;
                room.Links
                    .Where(link => !_disabledLinks.Contains(link) &&
                                   !visited.Contains(link.GetNeighbor(room)) &&
                                   link.IsNeiAccessible(room) &&
                                   (saveLabel == 0 || link.SaveLabel != saveLabel))
                    .ToList()
                    .ForEach(link =>
                    {
                        var nei = link.GetNeighbor(room);
                        nei.CameFrom = link;
                        q.Enqueue(nei);
                        visited.Add(nei);
                    });
            }
            return false;
        }

        private static bool BellmanFord()
        {
            var q = new Queue<QueueItem>();

            Map.StartRoom.Links
                .Where(link => link.IsNeiAccessible(Map.StartRoom) && !_disabledLinks.Contains(link))
                .ToList()
                .ForEach(link => q.Enqueue(new QueueItem(Map.StartRoom, link)));
            while (q.Count > 0)
            {
                var item = q.Dequeue();
                var room = item.Room;
                var nei = item.Link.GetNeighbor(room);

                if (room.VisitLabel == _currentSearchCounter && room.VisitCounter == Map.Rooms.Count)
                    break;

                var toQueue = false;

                if (nei.VisitLabel != _currentSearchCounter)
                {
                    nei.VisitCounter = 1;
                    nei.VisitLabel = _currentSearchCounter;
                    nei.StepsLabel = room.StepsLabel + item.Link.Weight;
                    nei.CameFrom = item.Link;
                    toQueue = true;
                }
                else if (nei.StepsLabel > room.StepsLabel + item.Link.Weight)
                {
                    nei.StepsLabel = room.StepsLabel + item.Link.Weight;
                    nei.VisitCounter++;
                    nei.CameFrom = item.Link;
                    toQueue = true;
                }

                if (toQueue && !nei.IsEnd && !nei.IsStart)
                    nei.Links
                        .Where(link =>
                        {
                            var neiOfNei = link.GetNeighbor(nei);
                            return !_disabledLinks.Contains(link) &&
                                   link.IsRoomAccessible(neiOfNei) &&
                                   !neiOfNei.IsStart &&
                                   !(item.PathBlock && link.Weight == 1);
                        })
                        .ToList()
                        .ForEach(link => q.Enqueue(new QueueItem(nei, link, link.Weight == 1 && _doubledRooms.Contains(link.GetNeighbor(nei)))));

            }
            return Map.EndRoom.VisitLabel == _currentSearchCounter;
        }

        private static int GetMaxPaths()
        {
            return Math.Min(Map.StartRoom.Links.Count(), Map.EndRoom.Links.Count());
        }

        private static void ChangeMinPath()
        {
            var room = Map.EndRoom;

            _doubledRooms = new HashSet<Room>();
            while (!room.IsStart)
            {
                var link = room.CameFrom;

                if (link.Weight == -1)
                    _disabledLinks.Add(link);
                else
                {
                    link.DenyAccessToRoom(room);
                    link.Weight = -1;
                    if (!room.IsEnd && !room.IsStart)
                        _doubledRooms.Add(room);
                }
                room = link.GetNeighbor(room);
            }
        }

        private static void Suurballe()
        {
            Map.StartRoom.StepsLabel = 0;
            ChangeMinPath();
            for (var curPathsCount = 2; curPathsCount <= MaxPaths; curPathsCount++)
            {
                _currentSearchCounter++;
                if (!BellmanFord())
                    break;
                ChangeMinPath();
                Solutions.SaveSolution(curPathsCount);
                if (Solutions.IsEnough())
                    break;
                _disabledLinks.ToList().ForEach(link => link.Enable());
                _disabledLinks = new HashSet<Link>();
            }
        }
        public static void Run()
        {
            MaxPaths = GetMaxPaths();
            if (MaxPaths == 0 || !Bfs(Map.StartRoom, Map.EndRoom))
                throw new Exceptions.NoPathException();
            Solutions.SaveSolution(1, true);
            if (Map.AntsNumber == 1)
                return;
            Suurballe();
        }
    }

}
