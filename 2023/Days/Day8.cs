using System.Text.RegularExpressions;
using static _2023.Utils.InputUtils;
using Path = (string Left, string Right);

namespace _2023.Days;

public sealed partial class Day8 : IDay
{
	private const string Start = "AAA";
	private const string End = "ZZZ";

	public (string PartOne, string PartTwo) Run(bool isTest)
	{
		return (RunPuzzleOne(isTest).ToString(), RunPuzzleTwo(isTest).ToString());
	}

	#region Puzzle one

	private static int RunPuzzleOne(bool isTest)
	{
		var lines = isTest ? GetAllLines(isTest, 8, 2) : GetAllLines(isTest, 8);
		var instructions = lines[0].Select(ParseDirection).ToList();

		var network = GetNetwork(lines.Skip(1).ToList());
		return GetStepsToEnd(instructions, network);
	}

	private static Dictionary<string, Path> GetNetwork(IReadOnlyList<string> lines)
	{
		var pathRegex = PathRegex();
		var network = new Dictionary<string, Path>();

		foreach (var line in lines.Skip(1))
		{
			var match = pathRegex.Match(line);
			if (!match.Success)
			{
				continue;
			}

			network[match.Groups["Node"].Value] = (
				match.Groups["Left"].Value,
				match.Groups["Right"].Value
			);
		}

		return network;
	}

	private static int GetStepsToEnd(
		IReadOnlyList<Direction> instructions,
		IDictionary<string, Path> network
	)
	{
		var instructionsCount = instructions.Count;
		var steps = 0;
		var node = Start;

		while (node != End)
		{
			var instruction = instructions[steps % instructionsCount];
			var path = network[node];

			node = instruction switch
			{
				Direction.Left => path.Left,
				Direction.Right => path.Right,
				_ => throw new Exception($"Unknown direction {node} somehow"),
			};
			++steps;
		}

		return steps;
	}

	#endregion Puzzle one

	#region Puzzle two

	private static int RunPuzzleTwo(bool isTest)
	{
		return 0;
	}

	#endregion Puzzle two

	#region Common

	private static Direction ParseDirection(char direction) =>
		direction switch
		{
			'L' => Direction.Left,
			'R' => Direction.Right,
			_
				=> throw new ArgumentOutOfRangeException(
					nameof(direction),
					direction,
					"Unknown direction"
				),
		};

	#endregion Common

	#region Types

	public enum Direction
	{
		Left,
		Right,
	}

	[GeneratedRegex(
		@"^(?<Node>\w{3}) = \((?<Left>\w{3}), (?<Right>\w{3})\)$",
		RegexOptions.Compiled
	)]
	private static partial Regex PathRegex();

	#endregion Types
}
