using System;
using System.Collections.Generic;

namespace Advent._2020
{
	class Program
	{
		private static readonly Dictionary<int, IDay> Solutions = new Dictionary<int, IDay>
		{
			{ 1, new Day01() },
			{ 2, new Day02() },
			{ 3, new Day03() },
			{ 4, new Day04() },
			{ 5, new Day05() }
		};

		static void Main(string[] args)
		{
			if (args.Length == 0 || !int.TryParse(args[0], out int day))
			{
				Console.Error.WriteLine("Error: You must pass in a day number for the first variable");
				return;
			}

			if (!Solutions.TryGetValue(day, out IDay solution))
			{
				Console.Error.WriteLine($"Error: Unsupported day ({day})");
				return;
			}

			(string partOne, string partTwo) = solution.GetResult();

			Console.WriteLine($"Part one: {partOne}");
			Console.WriteLine($"Part two: {partTwo}");
		}
	}
}
