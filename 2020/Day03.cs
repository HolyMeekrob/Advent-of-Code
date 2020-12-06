using System;
using System.IO;
using static Advent._2020.Utils;

namespace Advent._2020
{
	class Day03 : IDay
	{
		private static readonly string[] Input = File.ReadAllLines(@".\Day03.txt");

		public (string, string) GetResult() => (GetPartOne(), GetPartTwo());

		private static string GetPartOne() => GetTreeCount(GetNextPoint(3, 1)).ToString();

		private static string GetPartTwo() =>
			Product(
				GetTreeCount(GetNextPoint(1, 1)),
				GetTreeCount(GetNextPoint(3, 1)),
				GetTreeCount(GetNextPoint(5, 1)),
				GetTreeCount(GetNextPoint(7, 1)),
				GetTreeCount(GetNextPoint(1, 2))
			).ToString();

		private static uint GetTreeCount(Func<Point, Point> getNextPoint, uint count = 0, Point currentPoint = null)
		{
			var point = currentPoint ?? new Point(0, 0);
			if (point.Y >= Input.Length)
			{
				return count;
			}

			uint newCount = IsTree(Input[point.Y][point.X]) ? count + 1 : count;

			return GetTreeCount(getNextPoint, newCount, getNextPoint(point));
		}

		private static Func<Point, Point> GetNextPoint(int moveX, int moveY) => (point) =>
			new Point(
				(point.X + moveX) % Input[0].Length,
				point.Y + moveY
			);

		private static bool IsTree(char v) => v == '#';

		private record Point(int X, int Y);
	}
}
