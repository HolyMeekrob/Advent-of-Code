using _2023;
using _2023.Days;
using _2023.Utils;

int day = default;
var isTest = args.Any(arg => arg.Equals("test", StringComparison.OrdinalIgnoreCase));

if (args.IsEmpty() || !int.TryParse(args[0], out day))
{
	while (day == default)
	{
		Console.WriteLine("Enter a day:");
		var input = Console.ReadLine();
		if (!int.TryParse(input, out day))
		{
			Console.WriteLine($"Invalid day: {input}");
		}
	}
}

var (part1, part2) = RunDay(day, isTest);

Console.WriteLine($"--- Advent of Code 2023 (Day {day}) results ---");
Console.WriteLine($"Part 1: {part1}");
Console.WriteLine($"Part 2: {part2}");

static (string Part1, string Part2) RunDay(int day, bool isTest) =>
	day switch
	{
		01 => new Day1().Run(isTest),
		_ => throw new ArgumentException($"Day {day} is not implemented", nameof(day), null),
	};
