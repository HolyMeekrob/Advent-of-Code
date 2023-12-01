namespace _2022._06;

public static class Day6
{
	private const string InputFileName = "Day6.txt";
	private const string TestInputFileName = "Day6_Test.txt";

	public static (object Part1, object Part2) Run(bool isTest)
	{
		var filename = isTest ? TestInputFileName : InputFileName;
		var lines = File.ReadAllLines(Path.Combine("06", filename));

		return (GetPartOne(lines), GetPartTwo(lines));
	}

	private static string GetPartOne(IReadOnlyCollection<string> lines) =>
		string.Join(", ", lines.Select(line => Run(line, 4)));
	private static string GetPartTwo(IReadOnlyCollection<string> lines) =>
		string.Join(", ", lines.Select(line => Run(line, 14)));

	private static int Run(string line, int charCount) =>
		line.Aggregate((Array.Empty<char>(), 0, false), (acc, nextChar) =>
		{
			var chars = acc.Item1;
			var index = acc.Item2 + 1;

			if (acc.Item3)
			{
				return acc;
			}

			if (chars.Length < charCount)
			{
				return (chars.Append(nextChar).ToArray(), index, false);
			}

			chars = chars[1..].Append(nextChar).ToArray();

			return (chars, index, new HashSet<char>(chars).Count == charCount);
		})
		.Item2;
}
