using System.Collections.Generic;
using System.Drawing;

namespace Dungeon
{
	public class BfsTask
	{
		private static IEnumerable<Point> GetPossibleStep(Point currentPosition)
		{
			yield return new Point(currentPosition.X, currentPosition.Y + 1);
			yield return new Point(currentPosition.X + 1, currentPosition.Y);
			yield return new Point(currentPosition.X, currentPosition.Y - 1);
			yield return new Point(currentPosition.X - 1, currentPosition.Y);
		}

		private static IEnumerable<Point> GetAvailableStep(Map map, Point currentPosition)
		{
			foreach (var step in GetPossibleStep(currentPosition))
				if (map.InBounds(step) && map.Dungeon[step.X, step.Y] == MapCell.Empty)
					yield return step;
		}

		public static IEnumerable<SinglyLinkedList<Point>> FindPaths(Map map, Point start, Point[] chests)
        {
			var queue = new Queue<SinglyLinkedList<Point>>();
			var visitedPoints = new HashSet<Point>();
			var chestsHashSet = new HashSet<Point>();
			foreach (var chest in chests)
				chestsHashSet.Add(chest);

			visitedPoints.Add(start);
			queue.Enqueue(new SinglyLinkedList<Point>(start));
			while (queue.Count > 0)
            {
				var currentPosition = queue.Dequeue();
				foreach (var step in GetAvailableStep(map, currentPosition.Value))
                {
					if (visitedPoints.Contains(step)) continue;
					visitedPoints.Add(step);
					var newTail = new SinglyLinkedList<Point>(step, currentPosition);
					queue.Enqueue(newTail);
					if (chestsHashSet.Contains(step)) yield return newTail;
                }
            }
		}
	}
}