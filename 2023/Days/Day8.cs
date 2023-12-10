using System.Text.RegularExpressions;
using _2023.Utils;
using static _2023.Utils.InputUtils;
using Path = (string Left, string Right);

namespace _2023.Days;

public sealed partial class Day8 : IDay
{
	private const string Start = "AAA";
	private const string End = "ZZZ";
	private const char StartChar = 'A';
	private const char EndChar = 'Z';

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

	private static long RunPuzzleTwo(bool isTest)
	{
		var lines = isTest ? GetAllLines(isTest, 8, 3) : GetAllLines(isTest, 8);
		var instructions = lines[0].Select(ParseDirection).ToList();

		var network = GetNetwork(lines.Skip(1).ToList());
		return GetParallelStepsToEnd(instructions, network);
	}

	private static long GetParallelStepsToEnd(
		IReadOnlyList<Direction> instructions,
		IReadOnlyDictionary<string, Path> network
	)
	{
		// Implementation note: Each path cycles to one destination at a regular interval.
		// They do not end at multiple destinations, and they only have one cycle.
		// Andy's comment: This is a lame puzzle because there's no indication that that's true
		// except by trial and error and by deducing that otherwise the puzzle is not solveable in
		// a reasonable amount of time.
		var nodes = network.Keys.Where(key => key.EndsWith(StartChar)).ToList();

		var cycleLengths = nodes
			.Select(node => GetCycleLength(node, instructions, network))
			.ToList();
		return GetLeastCommonMultiple(cycleLengths);
	}

	private static long GetCycleLength(
		string start,
		IReadOnlyList<Direction> instructions,
		IReadOnlyDictionary<string, Path> network
	)
	{
		var instructionsCount = instructions.Count;
		var instructionOffset = 0;
		var steps = 0L;
		var node = start;

		while (!node.EndsWith(EndChar))
		{
			var instruction = instructions[instructionOffset];
			var path = network[node];

			node = instruction switch
			{
				Direction.Left => path.Left,
				Direction.Right => path.Right,
				_ => throw new Exception($"Unknown direction {node} somehow"),
			};
			instructionOffset = (instructionOffset + 1) % instructionsCount;
			++steps;
		}

		return steps;
	}

	private static long GetLeastCommonMultiple(IReadOnlyList<long> numbers)
	{
		if (numbers.IsEmpty())
		{
			throw new ArgumentException(
				"Cannot compute the least common multiple of an empty list"
			);
		}

		if (numbers.Count == 1)
		{
			return numbers[0];
		}

		if (numbers.Count == 2)
		{
			return numbers[0] * numbers[1] / GetGreatestCommonDivisor(numbers[0], numbers[1]);
		}

		return GetLeastCommonMultiple(
			[numbers[0], GetLeastCommonMultiple(numbers.Skip(1).ToList())]
		);
	}

	private static long GetGreatestCommonDivisor(long x, long y)
	{
		var a = Math.Max(x, y);
		var b = Math.Min(x, y);

		while (a != b)
		{
			var delta = a - b;
			a = Math.Max(delta, b);
			b = Math.Min(delta, b);
		}

		return a;
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
