using _2023.Utils;

namespace _2023.Days;

using static InputUtils;

public sealed class Day1 : IDay
{
	public (string PartOne, string PartTwo) Run(bool isTest) =>
		(RunPuzzleOne(isTest).ToString(), RunPuzzleTwo(isTest).ToString());

	#region Puzzle one

	private static int RunPuzzleOne(bool isTest) =>
		GetAllLines(isTest, 1).Select(GetNumberPartOne).Sum();

	private static int GetNumberPartOne(string line) =>
		GetFirstNumberPartOne(line) * 10 + GetLastNumberPartOne(line);

	private static int GetFirstNumberPartOne(string line) => line.First(char.IsDigit) - '0';

	private static int GetLastNumberPartOne(string line) => line.Last(char.IsDigit) - '0';

	#endregion Puzzle one

	#region Puzzle two

	private static int RunPuzzleTwo(bool isTest) =>
		GetAllLines(isTest, 2).Select(GetNumberPartTwo).Sum();

	private static int GetNumberPartTwo(string line) =>
		GetFirstNumberPartTwo(line) * 10 + GetLastNumberPartTwo(line);

	private static int GetFirstNumberPartTwo(string line)
	{
		var pendingNumber = "";
		foreach (var c in line)
		{
			if (char.IsDigit(c))
			{
				return c - '0';
			}

			pendingNumber += c;

			if (NumberStrings.TryGetValue(pendingNumber, out var s))
			{
				return s;
			}

			pendingNumber = PareDown(pendingNumber, NumberStrings.Keys);
		}

		throw new ArgumentException($"Line in incorrectly encoded: {line}", nameof(line));
	}

	private static int GetLastNumberPartTwo(string line)
	{
		var pendingNumber = "";
		for (var i = line.Length - 1; i >= 0; --i)
		{
			var c = line[i];
			if (char.IsDigit(c))
			{
				return c - '0';
			}

			pendingNumber += c;

			if (ReversedNumberStrings.TryGetValue(pendingNumber, out var s))
			{
				return s;
			}

			pendingNumber = PareDown(pendingNumber, ReversedNumberStrings.Keys);
		}

		throw new ArgumentException($"Line in incorrectly encoded: {line}", nameof(line));
	}

	private static string PareDown(string pendingNumber, IReadOnlyCollection<string> numberStrings)
	{
		if (pendingNumber.IsEmpty())
		{
			return pendingNumber;
		}

		return numberStrings.Any(numberString => numberString.StartsWith(pendingNumber))
			? pendingNumber
			: PareDown(pendingNumber.Substring(1), numberStrings);
	}

	private static readonly Dictionary<string, int> NumberStrings =
		new()
		{
			{ "zero", 0 },
			{ "one", 1 },
			{ "two", 2 },
			{ "three", 3 },
			{ "four", 4 },
			{ "five", 5 },
			{ "six", 6 },
			{ "seven", 7 },
			{ "eight", 8 },
			{ "nine", 9 },
		};

	private static readonly Dictionary<string, int> ReversedNumberStrings =
		NumberStrings.ToDictionary(
			entry =>
			{
				var arr = entry.Key.ToArray();
				Array.Reverse(arr);
				return new string(arr);
			},
			entry => entry.Value
		);

	#endregion Puzzle two
}
