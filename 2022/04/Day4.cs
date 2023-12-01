using System.Text.RegularExpressions;

namespace _2022._04;

public static class Day4
{
	private const string InputFileName = "Day4.txt";
	private const string TestInputFileName = "Day4_Test.txt";

	public static (object Part1, object Part2) Run(bool isTest)
	{
		var filename = isTest ? TestInputFileName : InputFileName;
		var lines = File.ReadAllLines(Path.Combine("04", filename));
		var pairs = ParseInput(lines);

		return (GetPartOne(pairs), GetPartTwo(pairs));
	}

	private static List<(SectionRange, SectionRange)> ParseInput(IReadOnlyCollection<string> lines)
	{
		var regex = new Regex(@"(\d+)-(\d+),(\d+)-(\d+)", RegexOptions.Compiled);

		return lines.Select(line =>
		{
			var groups = regex.Match(line).Groups;
			return
			(
				new SectionRange(int.Parse(groups[1].Value), int.Parse(groups[2].Value)),
				new SectionRange(int.Parse(groups[3].Value), int.Parse(groups[4].Value))
			);
		}).ToList();
	}

	private static int GetPartOne(List<(SectionRange A, SectionRange B)> pairs) =>
		pairs.Count(pair => HasSuperset(pair.A, pair.B));

	private static int GetPartTwo(List<(SectionRange A, SectionRange B)> pairs) =>
		pairs.Count(pair => HasOverlap(pair.A, pair.B));
	private static bool HasSuperset(SectionRange a, SectionRange b) =>
		IsSuperset(a, b) || IsSuperset(b, a);

	private static bool IsSuperset(SectionRange a, SectionRange b) =>
		a.Start <= b.Start && a.End >= b.End;

	private static bool HasOverlap(SectionRange a, SectionRange b) =>
		(a.Start >= b.Start && a.Start <= b.End)
		|| (b.Start >= a.Start && b.Start <= a.End);

	private record SectionRange(int Start, int End);
}
