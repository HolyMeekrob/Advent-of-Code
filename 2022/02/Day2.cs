using System.Text.RegularExpressions;

namespace _2022._02;

public static class Day2
{
	private const string InputFileName = "Day2.txt";
	private const string TestInputFileName = "Day2_Test.txt";

	public static (object Part1, object Part2) Run(bool isTest)
	{
		var filename = isTest ? TestInputFileName : InputFileName;
		var lines = File.ReadAllLines(Path.Combine("02", filename));

		var part1 = GetScore(ParseInputPartOne(lines));
		var part2 = GetScore(ParseInputPartTwo(lines));

		return (part1, part2);
	}

	private static readonly Regex InputRegex = new("(?<Them>[ABC]) (?<Other>[XYZ])");

	private static List<Round> ParseInputPartOne(IEnumerable<string> lines) =>
		lines
			.Select(line =>
			{
				var match = InputRegex.Match(line);
				return Round.CreatePartOne(match.Groups["Them"].Value, match.Groups["Other"].Value);
			})
			.ToList();

	private static List<Round> ParseInputPartTwo(IEnumerable<string> lines) =>
		lines
			.Select(line =>
			{
				var match = InputRegex.Match(line);
				return Round.CreatePartTwo(match.Groups["Them"].Value, match.Groups["Other"].Value);
			})
			.ToList();

	private static int GetScore(IReadOnlyCollection<Round> rounds) =>
		rounds.Sum(round => round.Score);

	private record Round
	{
		private Throw Me { get; init; }
		private Throw Them { get; init; }

		public int Score
		{
			get
			{
				var roundValue = Against(this.Me, this.Them) switch
				{
					Result.Win => 6,
					Result.Tie => 3,
					_ => 0,
				};

				var throwValue = this.Me switch
				{
					Throw.Rock => 1,
					Throw.Paper => 2,
					_ => 3,
				};

				return roundValue + throwValue;
			}
		}

		public static Round CreatePartOne(string them, string me)
		{
			var meThrow = me switch
			{
				"X" => Throw.Rock,
				"Y" => Throw.Paper,
				"Z" => Throw.Scissors,
				_ => throw new ArgumentOutOfRangeException(nameof(me), me, $"Unrecognized throw: {me}")
			};

			var themThrow = them switch
			{
				"A" => Throw.Rock,
				"B" => Throw.Paper,
				"C" => Throw.Scissors,
				_ => throw new ArgumentOutOfRangeException(nameof(them), them, $"Unrecognized throw: {them}")
			};

			return new Round { Me = meThrow, Them = themThrow };
		}

		public static Round CreatePartTwo(string them, string result)
		{
			var themThrow = them switch
			{
				"A" => Throw.Rock,
				"B" => Throw.Paper,
				_ => Throw.Scissors,
			};

			var meThrow = result switch
			{
				"X" => LoseTo(themThrow),
				"Y" => themThrow,
				_ => Beat(themThrow),
			};

			return new Round { Me = meThrow, Them = themThrow };
		}
	}

	private static bool Beats(Throw me, Throw them) =>
		Beat(them) == me;

	private static Result Against(Throw me, Throw them)
	{
		if (me == them)
		{
			return Result.Tie;
		}

		return Beats(me, them) ? Result.Win : Result.Loss;
	}

	private static Throw LoseTo(Throw t) =>
		t switch
		{
			Throw.Rock => Throw.Scissors,
			Throw.Paper => Throw.Rock,
			_ => Throw.Paper,
		};

	private static Throw Beat(Throw t) =>
		t switch
		{
			Throw.Rock => Throw.Paper,
			Throw.Paper => Throw.Scissors,
			_ => Throw.Rock,
		};

	private enum Throw
	{
		Rock,
		Paper,
		Scissors,
	}

	private enum Result
	{
		Win,
		Loss,
		Tie,
	}
}
