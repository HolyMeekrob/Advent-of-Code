using _2023.Utils;
using static _2023.Utils.InputUtils;

namespace _2023.Days;

public sealed class Day6 : IDay
{
	public (string PartOne, string PartTwo) Run(bool isTest)
	{
		var races = CreateRaces(GetAllLines(isTest, 6));
		return (RunPuzzleOne(races).ToString(), RunPuzzleTwo(races).ToString());
	}

	private static int RunPuzzleOne(IReadOnlyList<Race> races)
	{
		return races.Select(GetWinnersCount).Product();
	}

	private static int GetWinnersCount(Race race)
	{
		var pressed = 0;
		while ((race.Time - pressed) * pressed <= race.Distance)
		{
			++pressed;
		}

		return race.Time - pressed * 2 + 1;
	}

	#region Puzzle two
	private static int RunPuzzleTwo(IReadOnlyList<Race> races)
	{
		return 0;
	}

	#endregion Puzzle two

	#region Common

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

		static List<int> GetValues(string line) =>
			line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
				.Skip(1)
				.Select(int.Parse)
				.ToList();
	}

	#endregion Common

	#region Types

	private sealed record Race
	{
		public required int Time { get; init; }
		public required int Distance { get; init; }
	}

	#endregion Types
}
