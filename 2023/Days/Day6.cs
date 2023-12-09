using _2023.Utils;
using static _2023.Utils.InputUtils;

namespace _2023.Days;

public sealed class Day6 : IDay
{
	public (string PartOne, string PartTwo) Run(bool isTest)
	{
		var lines = GetAllLines(isTest, 6);
		return (RunPuzzleOne(lines).ToString(), RunPuzzleTwo(lines).ToString());
	}

	#region Puzzle one

	private static long RunPuzzleOne(IReadOnlyList<string> lines) =>
		CreateRaces(lines).Select(GetWinnersCount).Product();

	private static List<Race> CreateRaces(IReadOnlyList<string> lines)
	{
		if (lines.Count != 2)
		{
			throw new ArgumentException("Input must be two lines", nameof(lines));
		}

		var times = GetValues(lines[0]);
		var distances = GetValues(lines[1]);

		return times
			.Zip(distances)
			.Select(pair => new Race { Time = pair.First, Distance = pair.Second, })
			.ToList();

		static List<long> GetValues(string line) =>
			line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
				.Skip(1)
				.Select(long.Parse)
				.ToList();
	}

	#endregion Puzzle one

	#region Puzzle two

	private static long RunPuzzleTwo(IReadOnlyList<string> lines) =>
		GetWinnersCount(CreateRace(lines));

	private static Race CreateRace(IReadOnlyList<string> lines)
	{
		if (lines.Count != 2)
		{
			throw new ArgumentException("Input must be two lines", nameof(lines));
		}

		var time = ToNumber(lines[0]);
		var distance = ToNumber(lines[1]);
		return new Race { Time = time, Distance = distance, };

		static long ToNumber(string line) =>
			long.Parse(new string(line.Where(char.IsDigit).ToArray()));
	}

	#endregion Puzzle two

	#region Common

	private static long GetWinnersCount(Race race)
	{
		var pressed = 0;
		while ((race.Time - pressed) * pressed <= race.Distance)
		{
			++pressed;
		}

		return race.Time - pressed * 2 + 1;
	}

	#endregion Common

	#region Types

	private sealed record Race
	{
		public required long Time { get; init; }
		public required long Distance { get; init; }
	}

	#endregion Types
}
