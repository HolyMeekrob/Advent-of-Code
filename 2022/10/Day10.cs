using System.Text.RegularExpressions;

namespace _2022._10;

public static class Day10
{
	private const string InputFileName = "Day10.txt";
	private const string TestInputFileName = "Day10_Test.txt";

	public static (object Part1, object Part2) Run(bool isTest)
	{
		var filename = isTest ? TestInputFileName : InputFileName;
		var lines = File.ReadAllLines(Path.Combine("10", filename));
		var instructions = lines.Select(GetInstruction).ToArray();
		var registerValues = GetRegisterValues(instructions);

		return (GetPartOne(registerValues), GetPartTwo(registerValues));
	}

	private static Instruction GetInstruction(string line)
	{
		var regex = new Regex(@"(?:(?<noop>noop)|(?<add>addx -?\d+))", RegexOptions.Compiled);
		var match = regex.Match(line);

		if (match.Groups["noop"].Success)
		{
			return new Instruction { Type = InstructionType.Noop };
		}

		if (match.Groups["add"].Success && int.TryParse(match.Groups["add"].Value[5..], out var value))
		{
			return new Instruction { Type = InstructionType.Add, Value = value };
		}

		throw new ArgumentException($"Cannot parse {line}");
	}

	private static IReadOnlyList<int> GetRegisterValues(IReadOnlyList<Instruction> instructions)
	{
		var count = instructions.Count;
		var registerValues = new List<int>(count * 2) { 1 };

		for (var i = 0; i < count; ++i)
		{
			var instruction = instructions[i];
			var currentValue = registerValues.Last();
			registerValues.Add(currentValue);

			if (instruction.Type == InstructionType.Add)
			{
				registerValues.Add(currentValue + instruction.Value.GetValueOrDefault());
			}
		}

		return registerValues;
	}

	private static int GetPartOne(IReadOnlyList<int> registerValues)
	{
		var checkedCycles = new[] { 20, 60, 100, 140, 180, 220 };
		return checkedCycles.Sum(cycle => GetSignalStrength(cycle, registerValues[cycle - 1]));
	}

	private static string GetPartTwo(IReadOnlyList<int> registerValues)
	{
		var crt = new Crt(6, 40);

		for (var i = 0; i < registerValues.Count; ++i)
		{
			var row = i / 40;
			var col = i % 40;

			if (row >= crt.Pixels.Length)
			{
				break;
			}
			var registerValue = registerValues[i];

			crt.Pixels[row][col] = Math.Abs(registerValue - col) <= 1;
		}

		return crt.ToString();
	}

	private static int GetSignalStrength(int cycleNumber, int registerValue) => cycleNumber * registerValue;

	private record Instruction
	{
		public InstructionType Type { get; init; }
		public int? Value { get; init; }
	}

	private enum InstructionType
	{
		Noop,
		Add,
	}

	private record Crt
	{
		public bool[][] Pixels { get; }

		public Crt(int rows, int cols)
		{
			this.Pixels = new bool[rows][];
			for (var row = 0; row < rows; ++row)
			{
				this.Pixels[row] = new bool[cols];
			}
		}

		public override string ToString()
		{
			return $"\n{string.Join("\n", this.Pixels.Select(GetRowString))}";

			static string GetRowString(bool[] row) => new(row.Select(GetPixelRepresentation).ToArray());

			static char GetPixelRepresentation(bool value) => value switch
			{
				true => '#',
				false => ' ',
			};
		}
	}
}
