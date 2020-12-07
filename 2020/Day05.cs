using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent._2020
{
	class Day05 : IDay
	{
		private static readonly string[] Input = File.ReadAllLines(@".\Day05.txt");

		public (string, string) GetResult()
		{
			var seats = Input.Select(input => new Seat(input).Id).ToList();
			return (GetPartOne(seats), GetPartTwo(seats));
		}

		private static string GetPartOne(List<int> seats) => seats.Max().ToString();

		private static string GetPartTwo(List<int> seats)
		{
			seats.Sort();
			int i = 0;
			while (seats[i] + 1 == seats[i + 1])
			{
				++i;
			}

			return (seats[i] + 1).ToString();
		}

		private record Seat
		{
			public int Row { get; init; }
			public int Column { get; init; }
			public int Id { get => Row * 8 + Column; }

			public Seat(string input)
			{
				var match = Regex.Match(input, "(^[BF]{7})([RL]{3})$");
				var rowInstructions = match.Groups[1].Value;
				var colInstructions = match.Groups[2].Value;

				Row = Search(rowInstructions, 'F', 0, 127);
				Column = Search(colInstructions, 'L', 0, 7);
			}

			private static int Search(string instructions, char lowerHalf, int min, int max)
			{
				if (min == max)
				{
					return min;
				}

				var diff = (max - min) / 2.0f;

				return instructions[0] == lowerHalf
					? Search(instructions.Substring(1), lowerHalf, min, (int)Math.Floor(diff) + min)
					: Search(instructions.Substring(1), lowerHalf, (int)Math.Ceiling(diff) + min, max);
			}
		}
	}
}
