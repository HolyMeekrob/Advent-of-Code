using _2023.Days;
using _2023.Utils;

int day = default;
var isTest = args.Any(arg => arg.Equals("test", StringComparison.OrdinalIgnoreCase));

if (args.IsEmpty() || !int.TryParse(args[0], out day))
{
	while (day == default)
	{
		bool? isTesting = null;

		string? input;
		while (isTesting is null)
		{
			Console.WriteLine("Are you testing? (y/N)");
			input = Console.ReadLine();

			if (
				input is null
				|| input.Equals("")
				|| input.Equals("n", StringComparison.InvariantCultureIgnoreCase)
			)
			{
				isTesting = false;
			}
			else if (input.Equals("y", StringComparison.InvariantCultureIgnoreCase))
			{
				isTesting = true;
			}
		}

		isTest = isTesting.Value;

		Console.WriteLine("Enter a day:");
		input = Console.ReadLine();
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
		02 => new Day2().Run(isTest),
		03 => new Day3().Run(isTest),
		04 => new Day4().Run(isTest),
		05 => new Day5().Run(isTest),
		06 => new Day6().Run(isTest),
		_ => throw new ArgumentException($"Day {day} is not implemented", nameof(day), null),
	};
