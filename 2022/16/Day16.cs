using System.Text.RegularExpressions;
using static _2022.FunctionalUtils;

namespace _2022._16;

public static class Day16
{
	private const string InputFileName = "Day16.txt";
	private const string TestInputFileName = "Day16_test.txt";

	public static (object Part1, object Part2) Run(bool isTest)
	{
		var filename = isTest ? TestInputFileName : InputFileName;
		var lines = File.ReadAllLines(Path.Combine("16", filename));
		var valves = lines.Select(GetValve).ToDictionary(valve => valve.Name, Identity);

		return (GetPartOne(valves), "Not implemented");
	}

	private static int GetPartOne(IDictionary<string, Valve> valves)
	{
		var sorted = valves.Values.OrderByDescending(valve => valve.FlowRate);
		var position = "AA";

		Console.WriteLine(string.Join("\n", valves.Values.Select(valve => valve.ToString())));

		return -1;
	}

	private static Valve GetValve(string line)
	{
		var regex = new Regex(
			@"Valve (?<name>[A-Z]{2}) has flow rate=(?<flowrate>\d+); tunnel(?:s)? lead(?:s)? to valve(?:s)? (?<connections>.*)",
			RegexOptions.Compiled);

		var match = regex.Match(line);

		return new Valve
		{
			Name = match.Groups["name"].Value,
			FlowRate = int.Parse(match.Groups["flowrate"].Value),
			Connections = match.Groups["connections"].Value.Split(", ").ToList(),
		};
	}

	private sealed record Edge
	{
		public required Valve A { get; init; }
		public required Valve B { get; init; }
	}

	private sealed record Valve
	{
		public required string Name { get; init; }
		public int FlowRate { get; init; }
		public IReadOnlyList<string> Connections { get; init; }
		public override string ToString() =>
			$"{this.Name} ({this.FlowRate}) -> {string.Join(", ", this.Connections)}";
	}
}
