using System.Globalization;
using System.Text;

namespace _2022._13;

public static class Day13
{
	private const string InputFileName = "Day13.txt";
	private const string TestInputFileName = "Day13_test.txt";

	public static (object Part1, object Part2) Run(bool isTest)
	{
		var filename = isTest ? TestInputFileName : InputFileName;
		var lines = File.ReadAllLines(Path.Combine("13", filename));
		var pairs = lines
			.Where(line => line.Any())
			.Chunk(2)
			.Select(CreatePair)
			.ToList();

		return (GetPartOne(pairs), GetPartTwo(pairs));
	}

	private static int GetPartOne(IReadOnlyList<Pair> pairs)
	{
		return pairs.Aggregate(
			(0, 1),
			GetInOrderIndices,
			GetIndicesValue);

		static (int Indices, int PairIndex) GetInOrderIndices((int Indices, int PairIndex) acc, Pair pair) =>
			IsInOrder(pair)
				? (acc.Indices + acc.PairIndex, acc.PairIndex + 1)
				: (acc.Indices, acc.PairIndex + 1);

		static int GetIndicesValue((int Indices, int PairIndex) acc) => acc.Indices;
	}

	private static int GetPartTwo(IReadOnlyList<Pair> pairs)
	{
		var dividerPacket1 = CreateDividerPacket(2);
		var dividerPacket2 = CreateDividerPacket(6);

		var sequences = pairs
			.SelectMany(pair => new[] { pair.Left.Sequence, pair.Right.Sequence })
			.Concat(new[] { dividerPacket1.Sequence, dividerPacket2.Sequence })
			.Order(new SequenceIsInOrderComparer())
			.ToList();

		return sequences.Aggregate(
			(1, 1),
			GetDecoderKeyIndices,
			GetDecoderKey);

		(int DecoderKey, int SequenceIndex) GetDecoderKeyIndices(
			(int DecoderKey, int SequenceIndex) acc,
			Element.Sequence sequence) =>
			IsDividerPacket(sequence)
				? (acc.DecoderKey * acc.SequenceIndex, acc.SequenceIndex + 1)
				: (acc.DecoderKey, acc.SequenceIndex + 1);

		static int GetDecoderKey((int DecoderKey, int SequenceIndex) acc) => acc.DecoderKey;
	}

	private static Packet CreateDividerPacket(int value)
	{
		var sequence = CreateSequence();
		var innerSequence = CreateSequence();
		innerSequence.Elements.Add(new Element.Number(value));
		sequence.Elements.Add(innerSequence);

		return new Packet { Sequence = sequence };
	}

	private static bool IsDividerPacket(Element.Sequence sequence) =>
		sequence.ToString() == "[[2]]" || sequence.ToString() == "[[6]]";

	private sealed class SequenceIsInOrderComparer : IComparer<Element.Sequence>
	{
		public int Compare(Element.Sequence? x, Element.Sequence? y) =>
			IsInOrder(x!, y!) switch
			{
				ComparisonResult.InOrder => -1,
				ComparisonResult.OutOfOrder => 1,
				_ => 0,
			};
	}

	private static bool IsInOrder(Pair pair) => IsInOrder(pair.Left.Sequence, pair.Right.Sequence) switch
	{
		ComparisonResult.InOrder => true,
		ComparisonResult.OutOfOrder => false,
		_ => throw new Exception("Comparison result could not be determined"),
	};

	private static ComparisonResult IsInOrder(Element.Sequence left, Element.Sequence right)
	{
		if (left.Elements.IsEmpty() && right.Elements.Any())
		{
			return ComparisonResult.InOrder;
		}

		if (left.Elements.Any() && right.Elements.IsEmpty())
		{
			return ComparisonResult.OutOfOrder;
		}
		for (var i = 0; i < Math.Max(left.Elements.Count, right.Elements.Count); ++i)
		{
			if (i >= left.Elements.Count)
			{
				return ComparisonResult.InOrder;
			}

			if (i >= right.Elements.Count)
			{
				return ComparisonResult.OutOfOrder;
			}

			var leftElement = left.Elements[i];
			var rightElement = right.Elements[i];
			if (leftElement is Element.Number l1 && rightElement is Element.Number r1)
			{
				if (l1.Value < r1.Value)
				{
					return ComparisonResult.InOrder;
				}

				if (l1.Value > r1.Value)
				{
					return ComparisonResult.OutOfOrder;
				}

				continue;
			}

			if (leftElement is Element.Sequence l2 && rightElement is Element.Sequence r2)
			{
				var result1 = IsInOrder(l2, r2);
				if (result1 == ComparisonResult.NotDetermined)
				{
					continue;
				}

				return result1;
			}

			if (leftElement is Element.Number l3 && rightElement is Element.Sequence r3)
			{
				var lSeq = CreateSequence();
				lSeq.Elements.Add(l3);
				var result2 = IsInOrder(lSeq, r3);
				if (result2 == ComparisonResult.NotDetermined)
				{
					continue;
				}

				return result2;
			}

			var l4 = leftElement as Element.Sequence;
			var r4 = rightElement as Element.Number;

			var rSeq = CreateSequence();
			rSeq.Elements.Add(r4!);

			var result3 = IsInOrder(l4!, rSeq);
			if (result3 == ComparisonResult.NotDetermined)
			{
				continue;
			}

			return result3;
		}

		return ComparisonResult.NotDetermined;
	}

	private static Pair CreatePair(IReadOnlyList<string> lines)
	{
		if (lines.Count != 2)
		{
			throw new FormatException("Could not identify pairs in input");
		}

		return new Pair { Left = CreatePacket(lines[0]), Right = CreatePacket(lines[1]) };
	}

	private static Packet CreatePacket(string line)
	{
		var sequences = new Stack<Element.Sequence>();

		using var reader = new StringReader(line);

		int character;
		var num = new StringBuilder();
		while ((character = reader.Read()) != -1)
		{
			var c = (char)character;

			if (char.IsDigit(c))
			{
				num.Append(c);
			}

			else if (c == ',' && num.Length > 0)
			{
				sequences.Peek().Elements.Add(new Element.Number(int.Parse(num.ToString())));
				num.Clear();
			}

			else if (c == '[')
			{
				var nextSequence = CreateSequence();
				if (sequences.Any())
				{
					sequences.Peek().Elements.Add(nextSequence);
				}
				sequences.Push(nextSequence);
			}

			else if (c == ']')
			{
				if (num.Length > 0)
				{
					sequences.Peek().Elements.Add(new Element.Number(int.Parse(num.ToString())));
					num.Clear();
				}
				if (sequences.Count > 1)
				{
					sequences.Pop();
				}
			}
		}

		if (sequences.Count != 1)
		{
			throw new Exception($"Unexpected number of sequences: {sequences.Count}");
		}

		return new Packet { Sequence = sequences.Pop() };
	}

	private static Element.Sequence CreateSequence() => new(new List<Element>());

	private abstract record Element
	{
		private Element() { }

		public sealed record Number(int Value) : Element
		{
			public override string ToString() => this.Value.ToString(NumberFormatInfo.InvariantInfo);
		}

		public sealed record Sequence(List<Element> Elements) : Element
		{
			public override string ToString() =>
				$"[{string.Join(",", this.Elements.Select(element => element.ToString()))}]";
		}
	}

	private sealed record Packet
	{
		public required Element.Sequence Sequence { get; init; }

		public override string ToString() => this.Sequence.ToString();
	}

	private sealed record Pair
	{
		public required Packet Left { get; init; }
		public required Packet Right { get; init; }

		public override string ToString() => $"{this.Left}\n{this.Right}";
	}

	private enum ComparisonResult
	{
		NotDetermined,
		InOrder,
		OutOfOrder,
	}
}
