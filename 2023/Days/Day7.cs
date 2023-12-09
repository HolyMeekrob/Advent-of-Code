using System.Collections;
using System.Text.RegularExpressions;
using static _2023.Utils.InputUtils;

namespace _2023.Days;

public sealed partial class Day7 : IDay
{
	public (string PartOne, string PartTwo) Run(bool isTest)
	{
		var lines = GetAllLines(isTest, 7);
		return (RunPuzzleOne(lines).ToString(), RunPuzzleTwo(lines).ToString());
	}

	#region Puzzle one

	private static int RunPuzzleOne(IReadOnlyList<string> lines) =>
		lines
			.Select(CreateGame)
			.OrderBy(game => game.Hand, new HandComparer())
			.Select((game, index) => GetWinning(game, index + 1))
			.Sum();

	private static Game CreateGame(string line)
	{
		var regex = InputRegex();
		var match = regex.Match(line);

		if (!match.Success)
		{
			throw new ArgumentException("Input isn't formatted correctly", nameof(line));
		}

		return new Game
		{
			Hand = Hand.Create(match.Groups["Hand"].Value),
			Bid = int.Parse(match.Groups["Bid"].ValueSpan),
		};
	}

	private static int GetWinning(Game game, int rank) => game.Bid * rank;

	#endregion Puzzle one

	#region Puzzle two

	private static int RunPuzzleTwo(IReadOnlyList<string> lines)
	{
		return 0;
	}

	#endregion Puzzle two

	#region Common

	private static Card CreateCard(char card) =>
		card switch
		{
			'2' => Card.Two,
			'3' => Card.Three,
			'4' => Card.Four,
			'5' => Card.Five,
			'6' => Card.Six,
			'7' => Card.Seven,
			'8' => Card.Eight,
			'9' => Card.Nine,
			'T' => Card.Ten,
			'J' => Card.Jack,
			'Q' => Card.Queen,
			'K' => Card.King,
			'A' => Card.Ace,
			_ => throw new ArgumentException($"Invalid card value: {card}")
		};

	private static char GetCharSymbol(Card card) =>
		card switch
		{
			Card.Two => '2',
			Card.Three => '3',
			Card.Four => '4',
			Card.Five => '5',
			Card.Six => '6',
			Card.Seven => '7',
			Card.Eight => '8',
			Card.Nine => '9',
			Card.Ten => 'T',
			Card.Jack => 'J',
			Card.Queen => 'Q',
			Card.King => 'K',
			Card.Ace => 'A',
			_ => throw new ArgumentException($"Invalid card: {card}")
		};

	private static HandType GetHandType(Hand hand)
	{
		var groups = hand.ToLookup(card => card)
			.Select(group => group.Count())
			.OrderDescending()
			.ToList();

		return groups[0] switch
		{
			5 => HandType.FiveOfAKind,
			4 => HandType.FourOfAKind,
			3 => groups[1] == 2 ? HandType.FullHouse : HandType.ThreeOfAKind,
			2 => groups[1] == 2 ? HandType.TwoPair : HandType.OnePair,
			_ => HandType.HighCard
		};
	}

	private sealed class HandComparer : IComparer<Hand>
	{
		public int Compare(Hand? x, Hand? y)
		{
			if (x is null)
			{
				return y is null ? 0 : -1;
			}

			if (y is null)
			{
				return 1;
			}

			var xType = GetHandType(x);
			var yType = GetHandType(y);

			if (xType != yType)
			{
				return xType.CompareTo(yType);
			}

			var matches = x.Zip(y).ToList();
			var mismatchIndex = matches.FindIndex(cards => cards.First != cards.Second);
			return mismatchIndex == -1
				? 0
				: matches[mismatchIndex].First.CompareTo(matches[mismatchIndex].Second);
		}
	}

	#endregion Common

	#region Types

	private enum Card
	{
		Two,
		Three,
		Four,
		Five,
		Six,
		Seven,
		Eight,
		Nine,
		Ten,
		Jack,
		Queen,
		King,
		Ace,
	}

	private enum HandType
	{
		HighCard,
		OnePair,
		TwoPair,
		ThreeOfAKind,
		FullHouse,
		FourOfAKind,
		FiveOfAKind,
	}

	private sealed record Hand(Card First, Card Second, Card Third, Card Fourth, Card Fifth)
		: IEnumerable<Card>
	{
		public IEnumerator<Card> GetEnumerator()
		{
			yield return First;
			yield return Second;
			yield return Third;
			yield return Fourth;
			yield return Fifth;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public override string ToString() => new(this.Select(GetCharSymbol).ToArray());

		private static Hand Create(params Card[] cards)
		{
			if (cards.Length != 5)
			{
				throw new ArgumentException("Hand must have exactly 5 cards", nameof(cards));
			}

			return new Hand(cards[0], cards[1], cards[2], cards[3], cards[4]);
		}

		private static Hand Create(IReadOnlyList<char> cards) =>
			Create(cards.Select(CreateCard).ToArray());

		public static Hand Create(string cards) => Create(cards.ToArray());
	}

	private sealed record Game
	{
		public required Hand Hand { get; init; }
		public required int Bid { get; init; }
	}

	// ReSharper disable once StringLiteralTypo
	[GeneratedRegex(@"^(?<Hand>[23456789TJQKA]{5}) (?<Bid>\d+)$", RegexOptions.Compiled)]
	private static partial Regex InputRegex();

	#endregion Types
}
