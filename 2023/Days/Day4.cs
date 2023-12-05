using System.Text;
using System.Text.RegularExpressions;
using static _2023.Utils.InputUtils;
using LineNumber = (int Row, int Col, int Num, int Length);

namespace _2023.Days;

public sealed partial class Day4 : IDay
{
	public (string PartOne, string PartTwo) Run(bool isTest)
	{
		var lines = GetAllLines(isTest, 4);
		return (RunPuzzleOne(lines).ToString(), RunPuzzleTwo(lines).ToString());
	}

	#region Puzzle one

	private static int RunPuzzleOne(IList<string> lines) =>
		lines.Select(CreateCard).Select(GetScore).ToList().Sum();

	private static Card CreateCard(string line)
	{
		var regex = CardRegex();
		var match = regex.Match(line);
		var id = int.Parse(match.Groups["Id"].ValueSpan);
		var data = match.Groups["CardData"].Value.Split("|");
		var winningNumbers = ParseNumbers(data[0]);
		var numbers = ParseNumbers(data[1]);

		return new Card(id, winningNumbers, numbers);

		static List<int> ParseNumbers(string numbersText) =>
			numbersText
				.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
				.Select(int.Parse)
				.ToList();
	}

	private static int GetScore(Card card)
	{
		var winnerCount = card.WinningNumbers.Intersect(card.Numbers).Count();
		return winnerCount == 0 ? 0 : (int)Math.Pow(2, winnerCount - 1);
	}

	#endregion Puzzle one

	#region Puzzle two

	private static int RunPuzzleTwo(IList<string> lines) => 0;

	#endregion Puzzle two

	private sealed record Card(
		int Id,
		IReadOnlyList<int> WinningNumbers,
		IReadOnlyList<int> Numbers
	);

	[GeneratedRegex(@"^Card +(?<Id>\d+): (?<CardData>.*)$", RegexOptions.Compiled)]
	private static partial Regex CardRegex();
}
