using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Dungeon
{
    public class DungeonTask
    {
        public static MoveDirection GetMoveDirectionBetweenPoints(Point firstPoint, Point secondPoint)
        {
            if (firstPoint.Y - 1 == secondPoint.Y)
                return MoveDirection.Up;
            if (firstPoint.X + 1 == secondPoint.X)
                return MoveDirection.Right;
            if (firstPoint.Y + 1 == secondPoint.Y)
                return MoveDirection.Down;
            if (firstPoint.X - 1 == secondPoint.X)
                return MoveDirection.Left;
            throw new ArgumentException();
        }

        public static List<MoveDirection> GetMoveList(List<Point> points)
        {
            var moveList = new List<MoveDirection>();
            for (int i = 0; i < points.Count - 1; i++)
                moveList.Add(GetMoveDirectionBetweenPoints(points[i], points[i + 1]));

            return moveList;
        }

        public static List<Point> ReverseSinglyLinkedList(SinglyLinkedList<Point> list)
        {
            var result = list.ToList();
            result.Reverse();
            return result;
        }

        public static MoveDirection[] FindShortestPath(Map map)
        {
            var fromStartToChests = BfsTask.FindPaths(map, map.InitialPosition, map.Chests).ToList();
            var fromExitToChests = BfsTask.FindPaths(map, map.Exit, map.Chests).ToList();
            var fromStartToExit = BfsTask.FindPaths(map, map.InitialPosition, new[] { map.Exit })
                .OrderBy(n => n.Length)
                .FirstOrDefault();
            var connectedPaths = ConnectPaths(fromStartToChests, fromExitToChests);

            if (!connectedPaths.Any() || fromStartToExit == null)
            {
                var anyPathFromStartToExit = BfsTask.FindPaths
                    (map, map.InitialPosition, new Point[] { map.Exit }).FirstOrDefault();

                if (anyPathFromStartToExit == null) return new MoveDirection[0];
                else return GetMoveList(ReverseSinglyLinkedList(anyPathFromStartToExit)).ToArray();
            }

            if (map.Chests.Any(chest => fromStartToExit.ToList().Contains(chest)))
                return GetMoveList(ReverseSinglyLinkedList(fromStartToExit)).ToArray();

            var shortestPath = connectedPaths
                .OrderBy(path => (path.Item1.Length + path.Item2.Length))
                .First();
            var result = GetMoveList(ReverseSinglyLinkedList(shortestPath.Item1))
                .Concat(GetMoveList(shortestPath.Item2.ToList()))
                .ToArray();
            return result;
        }

        public static IEnumerable<Tuple<SinglyLinkedList<Point>, SinglyLinkedList<Point>>> ConnectPaths(
            List<SinglyLinkedList<Point>> fromStartToChests, List<SinglyLinkedList<Point>> fromExitToChests)
        {
                var connectedPaths = (from first in fromStartToChests
                                      from second in fromExitToChests
                                      where first.Value == second.Value
                                      select Tuple.Create(first, second));
                return connectedPaths;
        }
    }
}