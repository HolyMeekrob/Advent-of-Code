using _2022;
using _2022._01;
using _2022._02;
using _2022._03;
using _2022._04;
using _2022._05;
using _2022._06;
using _2022._07;
using _2022._08;
using _2022._09;
using _2022._10;
using _2022._11;
using _2022._12;
using _2022._13;
using _2022._14;
using _2022._15;
using _2022._16;

int day = default;
var isTest = args.Any(arg => arg.Equals("test", StringComparison.OrdinalIgnoreCase));

if (args.IsEmpty() || !int.TryParse(args[0], out day))
{
	while (day == default)
	{
		Console.WriteLine("Enter a day:");
		var input = Console.ReadLine();
		int.TryParse(input, out day);
	}
}

var (part1, part2) = RunDay(day, isTest);
Console.WriteLine($"--- Advent of Code (Day {day}) results ---");
Console.WriteLine($"Part 1: {part1}");
Console.WriteLine($"Part 2: {part2}");

static (object Part1, object Part2) RunDay(int day, bool isTest) =>
	day switch
	{
		1 => Day1.Run(isTest),
		2 => Day2.Run(isTest),
		3 => Day3.Run(isTest),
		4 => Day4.Run(isTest),
		5 => Day5.Run(isTest),
		6 => Day6.Run(isTest),
		7 => Day7.Run(isTest),
		8 => Day8.Run(isTest),
		9 => Day9.Run(isTest),
		10 => Day10.Run(isTest),
		11 => Day11.Run(isTest),
		12 => Day12.Run(isTest),
		13 => Day13.Run(isTest),
		14 => Day14.Run(isTest),
		15 => Day15.Run(isTest),
		16 => Day16.Run(isTest),
		_ => throw new ArgumentException($"Day {day} is not implemented"),
	};
