using System.Text.RegularExpressions;
using static _2023.Utils.InputUtils;

namespace _2023.Days;

public sealed partial class Day4 : IDay
{
	public (string PartOne, string PartTwo) Run(bool isTest)
	{
		var cards = GetAllLines(isTest, 4).Select(CreateCard).ToList();
		return (RunPuzzleOne(cards).ToString(), RunPuzzleTwo(cards).ToString());
	}

	#region Puzzle one

	private static int RunPuzzleOne(IList<Card> cards) => cards.Select(GetScore).ToList().Sum();

	private static int GetScore(Card card)
	{
		var winnerCount = GetWinnerCount(card);
		return winnerCount == 0 ? 0 : (int)Math.Pow(2, winnerCount - 1);
	}

	#endregion Puzzle one

	#region Puzzle two

	private static int RunPuzzleTwo(IList<Card> cards)
	{
		var counts = cards.Select(card => new CardCount(card, 1)).ToList();
		for (var i = 0; i < counts.Count; ++i)
		{
			var winnerCount = GetWinnerCount(counts[i].Card);
			for (var j = 1; j < winnerCount + 1 && i + j < counts.Count; ++j)
			{
				counts[i + j] = counts[i + j].AddCopies(counts[i].Count);
			}
		}

		return counts.Select(count => count.Count).Sum();
	}

	#endregion Puzzle two

	private static Card CreateCard(string line)
	{
		var regex = CardRegex();
		var match = regex.Match(line);
		var data = match.Groups["CardData"].Value.Split("|");
		var winningNumbers = ParseNumbers(data[0]);
		var numbers = ParseNumbers(data[1]);

		return new Card(winningNumbers, numbers);

		static List<int> ParseNumbers(string numbersText) =>
			numbersText
				.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
				.Select(int.Parse)
				.ToList();
	}

	private static int GetWinnerCount(Card card) =>
		card.WinningNumbers.Intersect(card.Numbers).Count();

	private sealed record Card(IReadOnlyList<int> WinningNumbers, IReadOnlyList<int> Numbers);

	private sealed record CardCount(Card Card, int Count)
	{
		public CardCount AddCopies(int count) => this with { Count = Count + count };
	}

	[GeneratedRegex(@"^Card +\d+: (?<CardData>.*)$", RegexOptions.Compiled)]
	private static partial Regex CardRegex();
}
