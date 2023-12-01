using System.Text.RegularExpressions;

namespace _2022._11;

public static class Day11
{
	private const string InputFileName = "Day11.txt";
	private const string TestInputFileName = "Day11_test.txt";

	public static (object Part1, object Part2) Run(bool isTest)
	{
		var filename = isTest ? TestInputFileName : InputFileName;
		var notes = File.ReadAllText(Path.Combine("11", filename));

		return (GetPartOne(notes), GetPartTwo(notes));
	}

	private static long GetPartOne(string notes)
	{
		var monkeys = InitializeMonkeys(notes);
		var state = new State { Monkeys = monkeys, Round = 0, AfterInspectItem = x => x / 3 };
		var finalState = Iterate(state, 20);

		return GetFunnyBusiness(finalState
			.Monkeys
			.OrderByDescending(monkey => monkey.InspectionCount)
			.Take(2)
			.ToList());
	}

	private static long GetPartTwo(string notes)
	{
		var monkeys = InitializeMonkeys(notes);
		var state = new State { Monkeys = monkeys, Round = 0, AfterInspectItem = x => x };
		var finalState = Iterate(state, 10000);

		return GetFunnyBusiness(finalState
			.Monkeys
			.OrderByDescending(monkey => monkey.InspectionCount)
			.Take(2)
			.ToList());
	}

	private static long GetFunnyBusiness(IReadOnlyCollection<Monkey> monkeys) =>
		Product(monkeys.Select(monkey => monkey.InspectionCount));

	private static State Iterate(State state, int roundsRemaining)
	{
		if (roundsRemaining == 0)
		{
			return state;
		}

		var stateAfterMonkeyBusiness = ProcessMonkeys(state, 0);
		return Iterate(
			stateAfterMonkeyBusiness with { Round = stateAfterMonkeyBusiness.Round + 1 },
			roundsRemaining - 1);
	}

	private static State ProcessMonkeys(State state, int monkeyIndex)
	{
		if (monkeyIndex == state.Monkeys.Count)
		{
			return state;
		}

		var updatedState = ProcessItems(state, monkeyIndex);
		return ProcessMonkeys(updatedState, monkeyIndex + 1);
	}

	private static State ProcessItems(State state, int monkeyIndex)
	{
		var monkey = state.Monkeys[monkeyIndex];
		if (monkey.Items.IsEmpty())
		{
			return state;
		}

		var item = monkey.Items.First();
		item = monkey.Rule.InspectItem(item) % Product(state.Monkeys.Select(m => m.Rule.TestDivisor));
		item = state.AfterInspectItem(item);

		var updatedState = monkey.Rule.TestItem(item)
			? Throw(state, item, monkeyIndex, monkey.Rule.TargetIfTrue)
			: Throw(state, item, monkeyIndex, monkey.Rule.TargetIfFalse);

		return ProcessItems(updatedState, monkeyIndex);
	}

	private static State Throw(State state, long item, int throwerIndex, int targetIndex)
	{
		var updatedMonkeys = state.Monkeys
			.Select((monkey, index) =>
			{
				if (index == throwerIndex)
				{
					return monkey with
					{
						Items = monkey.Items.Skip(1).ToList(),
						InspectionCount = monkey.InspectionCount + 1,
					};
				}

				if (index == targetIndex)
				{
					return monkey with { Items = monkey.Items.Append(item).ToList() };
				}

				return monkey;
			})
			.ToList();

		return state with { Monkeys = updatedMonkeys };
	}

	private static IReadOnlyList<Monkey> InitializeMonkeys(string notes)
	{
		var regex = new Regex(
			string.Join(
				"",
				@"\s*Monkey (?<index>\d+):",
				@"\s*Starting items: (?<items>(?:\d+, )*\d+)",
				@"\s*Operation: new = old (?<operation>[\*|\+]) (?<inspect>(?:old|\d+))",
				@"\s*Test: divisible by (?<test>\d+)",
				@"\s*If true: throw to monkey (?<true>\d+)",
				@"\s*If false: throw to monkey (?<false>\d+)"),
			RegexOptions.Compiled | RegexOptions.Multiline);

		var matches = regex.Matches(notes);
		return matches.Select((match, i) =>
			{
				var index = int.Parse(match.Groups["index"].Value);
				if (index != i)
				{
					throw new FormatException($"Monkey index are out of order: {index} (expected {i}");
				}

				var items = match.Groups["items"].Value.Split(", ").Select(long.Parse).ToList();
				var operation = match.Groups["operation"].Value;
				var inspectValue = match.Groups["inspect"].Value;
				var testDivisor = int.Parse(match.Groups["test"].Value);
				var targetIfTrue = int.Parse(match.Groups["true"].Value);
				var targetIfFalse = int.Parse(match.Groups["false"].Value);

				return new Monkey
				{
					Rule = new Rule
					{
						InspectItem = GetInspectFunction(operation, inspectValue),
						TestDivisor = testDivisor,
						TargetIfTrue = targetIfTrue,
						TargetIfFalse = targetIfFalse,
					},
					InspectionCount = 0,
					Items = items.ToList(),
				};
			})
			.ToList();

		static Func<long, long> GetInspectFunction(string operation, string inspectValue)
		{
			if (operation == "+")
			{
				return x => x + (inspectValue == "old" ? x : int.Parse(inspectValue));
			}

			return x => x * (inspectValue == "old" ? x : int.Parse(inspectValue));
		}
	}

	private static long Product(IEnumerable<long> nums) => nums.Aggregate(1L, (prd, num) => prd * num);

	private record Rule
	{
		public required long TestDivisor { get; init; }
		public required Func<long, long> InspectItem { get; init; }
		public required int TargetIfTrue { get; init; }
		public required int TargetIfFalse { get; init; }
		public Func<long, bool> TestItem => x => x % this.TestDivisor == 0;
	}

	private record Monkey
	{
		public required long InspectionCount { get; init; }
		public required Rule Rule { get; init; }
		public required IReadOnlyList<long> Items { get; init; }
	}

	private record State
	{
		public required int Round { get; init; }
		public required Func<long, long> AfterInspectItem { get; init; }
		public required IReadOnlyList<Monkey> Monkeys { get; init; }

		public override string ToString()
		{
			var monkeyItems = string.Join(
				"\n",
				this.Monkeys.Select((monkey, index) => $"Monkey {index} has inspectionCount { monkey.InspectionCount} is holding {string.Join(", ", monkey.Items)}"));
			return $"After round {this.Round}:\n {monkeyItems}";
		}
	}
}
