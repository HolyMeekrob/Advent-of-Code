namespace _2022._03;

public static class Day3
{
	private const string InputFileName = "Day3.txt";
	private const string TestInputFileName = "Day3_Test.txt";

	public static (object Part1, object Part2) Run(bool isTest)
	{
		var filename = isTest ? TestInputFileName : InputFileName;
		var lines = File.ReadAllLines(Path.Combine("03", filename));

		var rucksacks = ParseInput(lines);

		return (GetPartOne(rucksacks), GetPartTwo(rucksacks));
	}

	private static List<Rucksack> ParseInput(IReadOnlyCollection<string> lines) =>
		lines.Select(line => new Rucksack(line)).ToList();

	private static int GetPartOne(IReadOnlyCollection<Rucksack> rucksacks) =>
		rucksacks.Select(GetCommonItem).Sum(GetPriority);

	private static int GetPartTwo(IReadOnlyCollection<Rucksack> rucksacks) =>
		rucksacks.Chunk(3).Select(GetBadge).Sum(GetPriority);

	private static char GetCommonItem(Rucksack rucksack) =>
		rucksack.Compartment1.Items.Intersect(rucksack.Compartment2.Items).First();

	private static char GetBadge(IReadOnlyCollection<Rucksack> rucksacks) =>
		rucksacks
			.Select(rucksack => rucksack.Compartment1.Items.Union(rucksack.Compartment2.Items).ToHashSet())
			.Aggregate((commonItems, rucksackItems) => commonItems.Intersect(rucksackItems).ToHashSet())
			.First();

	private static int GetPriority(char item) =>
		char.ToLower(item) - 'a' + (char.IsUpper(item) ? 27 : 1);

	private record Rucksack
	{
		public Compartment Compartment1 { get; }
		public Compartment Compartment2 { get; }

		public Rucksack(string items)
		{
			this.Compartment1 = new Compartment(items[..(items.Length / 2)]);
			this.Compartment2 = new Compartment(items[(items.Length / 2)..]);
		}
	}

	private record Compartment
	{
		public HashSet<char> Items { get; }

		public Compartment(string items)
		{
			this.Items = items.ToHashSet();
		}
	}
}
