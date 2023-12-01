using System.Text.RegularExpressions;

namespace _2022._05;

public static class Day5
{
	private const string InputFileName = "Day5.txt";
	private const string TestInputFileName = "Day5_Test.txt";

	public static (object Part1, object Part2) Run(bool isTest)
	{
		var filename = isTest ? TestInputFileName : InputFileName;
		var lines = File.ReadAllLines(Path.Combine("05", filename));
		var (state, instructions) = ParseInput(lines);

		return (GetPartOne(new State(state), instructions), GetPartTwo(new State(state), instructions));
	}

	private static (State State, List<Instruction> instructions) ParseInput(IReadOnlyList<string> lines)
	{
		var split = lines.ToList().IndexOf(string.Empty);

		var initialState = lines.Take(split).ToList();
		var instructions = lines.Skip(split + 1).ToList();

		return (GetState(initialState), GetInstructions(instructions));
	}

	private static State GetState(List<string> stateSpec)
	{
		var specReverse = stateSpec.ToList();
		specReverse.Reverse();

		var count = (specReverse.First().Length + 2) / 4;
		var state = new State(count);

		foreach (var line in specReverse.Skip(1))
		{
			if (line == null)
			{
				throw new Exception("This should not happen");
			}

			for (var i = 0; i < line.Length; ++i)
			{
				var character = line[i];
				if (!char.IsUpper(character))
				{
					continue;
				}

				var stackIndex = i / 4;

				state.Stacks[stackIndex].Crates.Push(character);
			}
		}

		return state;
	}

	private static List<Instruction> GetInstructions(List<string> instructionText)
	{
		var regex = new Regex(@"move (?<count>\d+) from (?<from>\d+) to (?<to>\d+)", RegexOptions.Compiled);

		return instructionText.Select(line =>
		{
			var match = regex.Match(line);
			return new Instruction
			{
				Count = int.Parse(match.Groups["count"].Value),
				From = int.Parse(match.Groups["from"].Value),
				To = int.Parse(match.Groups["to"].Value),
			};
		}).ToList();
	}

	private static string GetPartOne(State state, List<Instruction> instructions)
	{
		var finalState = instructions.Aggregate(state, RunInstructionIndividual);
		return new string(finalState.Stacks.Select(stack => stack.Peek()).ToArray());
	}

	private static string GetPartTwo(State state, List<Instruction> instructions)
	{
		var finalState = instructions.Aggregate(state, RunInstructionAll);
		return new string(finalState.Stacks.Select(stack => stack.Peek()).ToArray());
	}

	private static State RunInstructionIndividual(State state, Instruction instruction)
	{
		for (var movesRemaining = instruction.Count; movesRemaining > 0; --movesRemaining)
		{
			state.Stacks[instruction.To - 1].Push(state.Stacks[instruction.From - 1].Pop());
		}

		return state;
	}

	private static State RunInstructionAll(State state, Instruction instruction)
	{
		var crates = new Stack<char>(instruction.Count);
		for (var cratesRemaining = instruction.Count; cratesRemaining > 0; --cratesRemaining)
		{
			crates.Push(state.Stacks[instruction.From - 1].Pop());
		}

		for (var cratesRemaining = instruction.Count; cratesRemaining > 0; --cratesRemaining)
		{
			state.Stacks[instruction.To - 1].Push(crates.Pop());
		}

		return state;
	}

	private record State
	{
		public List<CrateStack> Stacks { get; }

		public State(int count)
		{
			var stacks = new List<CrateStack>(count);
			for (var i = 0; i < count; ++i)
			{
				stacks.Add(new CrateStack());
			}

			this.Stacks = stacks;
		}

		public State(State original)
		{
			var size = original.Stacks.Count;
			this.Stacks = new List<CrateStack>(size);
			for (var i = 0; i < size; ++i)
			{
				this.Stacks.Add(new CrateStack(original.Stacks[i]));
			}
		}

		public override string ToString() =>
			string.Join(" | ", this.Stacks.Select((stack, i) => $"{i} :: {stack.ToString()}"));
	}

	private record CrateStack()
	{
		public Stack<char> Crates { get; } = new();

		public CrateStack(CrateStack original)
		{
			this.Crates = new Stack<char>(original.Crates.Reverse());
		}

		public void Push(char id) => this.Crates.Push(id);
		public char Pop() => this.Crates.Pop();
		public char Peek() => this.Crates.Peek();

		public override string ToString() =>
			string.Join("-", this.Crates.ToArray());
	}

	private record Instruction
	{
		public int Count { get; init; }
		public int From { get; init; }
		public int To { get; init; }
	}
}
