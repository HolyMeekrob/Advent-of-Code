using System.Diagnostics;
using System.Text.RegularExpressions;

namespace _2022._09;

public static class Day9
{
	private const string InputFileName = "Day9.txt";
	private const string TestInputFileName = "Day9_Test.txt";

	public static (object Part1, object Part2) Run(bool isTest)
	{
		var filename = isTest ? TestInputFileName : InputFileName;
		var lines = File.ReadAllLines(Path.Combine("09", filename));
		var instructions = lines.Select(GetInstruction).ToArray();

		return (GetPartOne(instructions), GetPartTwo(instructions));
	}

	private static Instruction GetInstruction(string line)
	{
		var regex = new Regex(@"(?<direction>\w) (?<steps>\d+)", RegexOptions.Compiled);
		var match = regex.Match(line);

		var direction = match.Groups["direction"].Value switch
		{
			"U" => Direction.Up,
			"D" => Direction.Down,
			"R" => Direction.Right,
			"L" => Direction.Left,
			_ => throw new ArgumentException($"Unknown direction: {match.Groups["direction"].Value}"),
		};

		return new Instruction(direction, int.Parse(match.Groups["steps"].Value));
	}

	private static int GetPartOne(IReadOnlyList<Instruction> instructions) => GetVisitedCount(2, instructions);

	private static int GetPartTwo(IReadOnlyList<Instruction> instructions) => GetVisitedCount(10, instructions);
	private static int GetVisitedCount(int knotCount, IReadOnlyList<Instruction> instructions)
	{
		var  finalState = RunInstructions(new State(knotCount), instructions);
		return finalState.VisitedByTail.Count;
	}

	private static State RunInstructions(State state, IReadOnlyList<Instruction> instructions)
	{
		while (instructions.Any())
		{
			var instruction = instructions[0];

			state = RunInstruction(state, instruction.Direction);
			var updatedInstruction = instruction with { Steps = instruction.Steps - 1 };

			instructions = updatedInstruction.Steps == 0
				? instructions.Skip(1).ToArray()
				: instructions.Skip(1).Prepend(updatedInstruction).ToArray();
		}

		return state;
	}

	private static State RunInstruction(State state, Direction direction)
	{
		state.Knots[0] = Move(state.Knots[0], direction);
		return CatchUp(state);
	}

	private static Point Move(Point point, params Direction[] directions)
	{
		if (directions.IsEmpty())
		{
			return point;
		}

		var updatedPoint = directions[0] switch
		{
			Direction.Up => point with { Y = point.Y + 1 },
			Direction.Down => point with { Y = point.Y - 1 },
			Direction.Right => point with { X = point.X + 1 },
			Direction.Left => point with { X = point.X - 1 },
			_ => throw new ArgumentException($"Unrecognized direction: {directions[0]}"),
		};

		return Move(updatedPoint, directions.Skip(1).ToArray());
	}

	private static State CatchUp(State state)
	{
		for (var i = 1; i < state.Knots.Count; ++i)
		{
			var head = state.Knots[i - 1];
			var tail = state.Knots[i];

			var updatedTail = (head.X - tail.X, head.Y - tail.Y) switch
			{
				(0, 0) => tail,
				(1, 0) => tail,
				(-1, 0) => tail,
				(0, 1) => tail,
				(0, -1) => tail,
				(1, 1) => tail,
				(1, -1) => tail,
				(-1, 1) => tail,
				(-1, -1) => tail,
				(2, 0) => Move(tail, Direction.Right),
				(-2, 0) => Move(tail, Direction.Left),
				(0, 2) => Move(tail, Direction.Up),
				(0, -2) => Move(tail, Direction.Down),
				(2, 1) => Move(tail, Direction.Right, Direction.Up),
				(2, -1) => Move(tail, Direction.Right, Direction.Down),
				(-2, 1) => Move(tail, Direction.Left, Direction.Up),
				(-2, -1) => Move(tail, Direction.Left, Direction.Down),
				(1, 2) => Move(tail, Direction.Right, Direction.Up),
				(1, -2) => Move(tail, Direction.Right, Direction.Down),
				(-1, 2) => Move(tail, Direction.Left, Direction.Up),
				(-1, -2) => Move(tail, Direction.Left, Direction.Down),
				(2, 2) => Move(tail, Direction.Right, Direction.Up),
				(2, -2) => Move(tail, Direction.Right, Direction.Down),
				(-2, 2) => Move(tail, Direction.Left, Direction.Up),
				(-2, -2) => Move(tail, Direction.Left, Direction.Down),
				_ => throw new UnreachableException($"Unexpected state: {state}"),
			};

			state.Knots[i] = updatedTail;
		}

		state.VisitedByTail.Add(state.Knots.Last());
		return state;
	}

	private sealed record State
	{
		public List<Point> Knots { get; } = new();
		public HashSet<Point> VisitedByTail { get; } = new() { new Point(0, 0) };

		public State(int knots)
		{
			this.Knots = new List<Point>(knots);
			for (var i = 0; i < knots; ++i)
			{
				this.Knots.Add(new Point(0, 0));
			}
		}

		public override string ToString() =>
			$"Knots: {string.Join(" | ", this.Knots.Select(knot => knot.ToString()))} --- Tail: { string.Join(" | ", this.VisitedByTail.Select(point => point.ToString()))}";
	}

	private sealed record Point(int X, int Y)
	{
		public override string ToString() => $"({this.X},{this.Y})";
	}

	private sealed record Instruction(Direction Direction, int Steps);

	private enum Direction
	{
		Up,
		Down,
		Right,
		Left,
	}
}
