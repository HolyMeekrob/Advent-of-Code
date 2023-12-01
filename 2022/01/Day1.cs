namespace _2022._01;

public static class Day1
{
	private const string InputFileName = "Day1.txt";
	private const string TestInputFileName = "Day1_Test.txt";

	public static (object Part1, object Part2) Run(bool isTest)
	{
		var filename = isTest ? TestInputFileName : InputFileName;
		var lines = File.ReadAllLines(Path.Combine("01", filename));
		var elves = ParseInput(lines);

		var part1 = GetPartOne(elves);
		var part2 = GetPartTwo(elves);

		return (part1, part2);
	}

	private static List<Elf> ParseInput(IEnumerable<string> lines) =>
		lines.Aggregate(new List<Elf> { new() }, (elves, line) =>
				line.Trim().IsEmpty()
					? elves.Append(new Elf()).ToList()
					: elves.Take(elves.Count - 1).Append(elves.Last().AddCalories(int.Parse(line))).ToList());

	private static int GetPartOne(IReadOnlyCollection<Elf> elves) => elves.Max()?.CalorieCount ?? 0;

	private static int GetPartTwo(IReadOnlyCollection<Elf> elves) =>
		elves.OrderDescending().Take(3).Sum(elf => elf.CalorieCount);

	private record Elf : IComparable<Elf>
	{
		public int CalorieCount { get; private init; }
		public Elf AddCalories(int calories) => new() { CalorieCount = this.CalorieCount + calories };

		public int CompareTo(Elf? other) => this.CalorieCount - (other?.CalorieCount ?? int.MinValue);
	}
}
