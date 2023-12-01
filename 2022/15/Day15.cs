using System.Text.RegularExpressions;

namespace _2022._15;

public static class Day15
{
	private const string InputFileName = "Day15.txt";
	private const string TestInputFileName = "Day15_test.txt";

	public static (object Part1, object Part2) Run(bool isTest)
	{
		var filename = isTest ? TestInputFileName : InputFileName;
		var lines = File.ReadAllLines(Path.Combine("15", filename));
		var pairs = lines.Select(GetPair).ToList();

		return (GetPartOne(pairs, isTest ? 10 : 2000000), GetPartTwo(pairs, isTest ? 20 : 4000000));
	}

	private static int GetPartOne(IReadOnlyList<Pair> pairs, int row) =>
		ColumnsCannotHaveBeacon(pairs, row).Count;

	private static long GetPartTwo(IReadOnlyList<Pair> pairs, int max)
	{
		var possiblePoints = pairs
			.SelectMany(pair => JustOutsideTheRange(pair, max))
			.Distinct()
			.ToList();

		var beacon = possiblePoints.First(point => pairs.None(pair => IsInRange(pair, point)));
		Console.WriteLine(beacon);
		return ((long)beacon.X * 4_000_000) + beacon.Y;
	}

	private static HashSet<Coordinate> JustOutsideTheRange(Pair pair, int max)
	{
		var distance = GetDistance(pair) + 1;
		var coordinates = new HashSet<Coordinate>();

		for (var row = Math.Max(0, pair.Sensor.Y - distance); row <= max && row <= pair.Sensor.Y + distance; ++row)
		{
			var remainingDistance = distance - Math.Abs(pair.Sensor.Y - row);
			if (pair.Sensor.X - remainingDistance >= 0 && pair.Sensor.X - remainingDistance <= max)
			{
				coordinates.Add(new Coordinate(pair.Sensor.X - remainingDistance, row));
			}

			if (pair.Sensor.X + remainingDistance >= 0 && pair.Sensor.X + remainingDistance <= max)
			{
				coordinates.Add(new Coordinate(pair.Sensor.X + remainingDistance, row));
			}
		}

		return coordinates;
	}

	private static bool IsInRange(Pair pair, Coordinate point)
	{
		var pairDistance = GetDistance(pair);
		var pointDistance = GetDistance(pair.Sensor, point);
		return pointDistance <= pairDistance;
	}

	private static IReadOnlyList<int> ColumnsCannotHaveBeacon(IReadOnlyList<Pair> pairs, int row) =>
		pairs
			.Where(pair => HasVerticalOverlap(pair, row))
			.SelectMany(pair => PairCannotHaveBeaconAtColumns(pair, row))
			.Distinct()
			.Except(pairs.Where(pair => pair.Beacon.Y == row).Select(pair => pair.Beacon.X))
			.ToList();


	private static bool HasVerticalOverlap(Pair pair, int row)
	{
		var distance = GetDistance(pair);
		return Math.Abs(pair.Sensor.Y - row) <= distance;
	}

	private static IReadOnlyList<int> PairCannotHaveBeaconAtColumns(Pair pair, int row)
	{
		var distance = GetDistance(pair);
		var verticalDistance = GetDistance(pair.Sensor, pair.Sensor with { Y = row });

		var roomToMoveHorizontally = distance - verticalDistance;

		return Enumerable
			.Range(pair.Sensor.X - roomToMoveHorizontally, (roomToMoveHorizontally * 2) + 1)
			.ToList();
	}

	private static int GetDistance(Pair pair) => GetDistance(pair.Sensor, pair.Beacon);
	private static int GetDistance(Coordinate a, Coordinate b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

	private static Pair GetPair(string line)
	{
		var regex = new Regex(
			@"Sensor at x=(?<sensorx>-?\d+), y=(?<sensory>-?\d+): closest beacon is at x=(?<beaconx>-?\d+), y=(?<beacony>-?\d+)",
			RegexOptions.Compiled);

		var match = regex.Match(line);

		var sensor = new Coordinate(int.Parse(match.Groups["sensorx"].Value), int.Parse(match.Groups["sensory"].Value));
		var beacon = new Coordinate(int.Parse(match.Groups["beaconx"].Value), int.Parse(match.Groups["beacony"].Value));

		return new(sensor, beacon);
	}

	private sealed record Coordinate(int X, int Y)
	{
		public override string ToString() => $"({this.X},{this.Y})";
	}

	private sealed record Pair(Coordinate Sensor, Coordinate Beacon);
}
