using System.Text.RegularExpressions;
using static _2023.Utils.InputUtils;

namespace _2023.Days;

public partial class Day5 : IDay
{
	private const string FirstMapName = "seed";
	private const string LastMapName = "location";

	public (string PartOne, string PartTwo) Run(bool isTest)
	{
		var lines = GetAllLines(isTest, 5);
		var almanac = CreateAlmanac(lines);
		return (RunPuzzleOne(almanac).ToString(), RunPuzzleTwo(almanac).ToString());
	}

	#region Puzzle one

	private static long RunPuzzleOne(Almanac almanac) =>
		almanac
			.Seeds
			.Select(seed => MapToValue(seed, FirstMapName, LastMapName, almanac.Maps))
			.Min();

	private static long MapToValue(long val, string from, string final, IReadOnlyList<Map> maps)
	{
		while (!string.Equals(from, final, StringComparison.Ordinal))
		{
			var map = maps.First(m => m.From.Equals(from, StringComparison.Ordinal));
			long? mappedVal = null;
			foreach (var range in map.Ranges)
			{
				var offset = val - range.SourceStart;
				if (offset >= 0 && offset < range.Length)
				{
					mappedVal = range.DestinationStart + offset;
					break;
				}
			}

			val = mappedVal ?? val;
			from = map.To;
		}

		return val;
	}

	#endregion Puzzle one

	#region Puzzle two

	// Note: This is not optimized and takes a minute to run
	private static long RunPuzzleTwo(Almanac almanac)
	{
		var reversedAlmanac = GetReversedAlmanac(almanac);
		var seedRanges = GetSeedRanges(almanac.Seeds).OrderBy(range => range.Start).ToList();

		var locationNumber = 0L;
		while (true)
		{
			var mappedSeedNumber = MapToValue(
				locationNumber,
				LastMapName,
				FirstMapName,
				reversedAlmanac.Maps
			);
			if (
				seedRanges.Any(
					range => mappedSeedNumber >= range.Start && mappedSeedNumber <= range.End
				)
			)
			{
				return locationNumber;
			}

			++locationNumber;
		}
	}

	private static Almanac GetReversedAlmanac(Almanac almanac) =>
		almanac with
		{
			Maps = almanac.Maps.Select(GetReversedMap).ToList(),
		};

	private static Map GetReversedMap(Map map) =>
		new()
		{
			From = map.To,
			To = map.From,
			Ranges = GetReversedRanges(map),
		};

	private static List<MapRange> GetReversedRanges(Map map)
	{
		var mappedRanges = map.Ranges
			.Select(GetReversedRange)
			.OrderBy(range => range.SourceStart)
			.ToList();
		var allRanges = new List<MapRange>();
		long start = 0;

		// Add missing ranges
		foreach (var range in mappedRanges)
		{
			if (start < range.SourceStart)
			{
				allRanges.Add(
					new MapRange
					{
						SourceStart = start,
						DestinationStart = start,
						Length = range.SourceStart - start,
					}
				);
			}
			allRanges.Add(range);
			start = range.SourceStart + range.Length;
		}

		return allRanges;
	}

	private static MapRange GetReversedRange(MapRange range) =>
		range with
		{
			SourceStart = range.DestinationStart,
			DestinationStart = range.SourceStart,
		};

	private static List<(long Start, long End)> GetSeedRanges(IReadOnlyList<long> seeds)
	{
		if (seeds.Count % 2 != 0)
		{
			throw new ArgumentException(
				"Seed ranges must have an even number of inputs",
				nameof(seeds)
			);
		}

		return seeds.Chunk(2).Select(pair => (pair[0], pair[0] + pair[1])).ToList();
	}
	#endregion Puzzle two

	#region Common

	private static Almanac CreateAlmanac(IReadOnlyList<string> lines)
	{
		var seeds = GetSeeds(lines[0]);
		var maps = CreateMaps(lines.Skip(1).ToList());

		return new Almanac { Seeds = seeds, Maps = maps, };
	}

	private static List<long> GetSeeds(string line)
	{
		var seedsRegex = SeedsRegex();
		var matches = seedsRegex
			.Matches(line)
			.Select(match => long.Parse(match.ValueSpan))
			.ToList();
		return matches;
	}

	private static List<Map> CreateMaps(IReadOnlyList<string> lines)
	{
		var maps = new List<Map>();

		for (var lineNumber = 0; lineNumber < lines.Count; )
		{
			if (string.IsNullOrWhiteSpace(lines[lineNumber]))
			{
				++lineNumber;
			}

			var (from, to) = GetMapHeaderValues(lines[lineNumber]);
			++lineNumber;

			var rangesSource = new List<string>();
			while (lineNumber < lines.Count && !string.IsNullOrWhiteSpace(lines[lineNumber]))
			{
				rangesSource.Add(lines[lineNumber]);
				++lineNumber;
			}

			var ranges = rangesSource.Select(CreateRange).ToList();

			maps.Add(
				new Map
				{
					From = from,
					To = to,
					Ranges = ranges,
				}
			);
		}

		return maps;
	}

	private static (string From, string To) GetMapHeaderValues(string line)
	{
		var mapRegex = MapRegex();
		var match = mapRegex.Match(line);

		if (!match.Success)
		{
			throw new Exception($"Unrecognized map: {line}");
		}

		return (match.Groups["From"].Value, match.Groups["To"].Value);
	}

	private static MapRange CreateRange(string line)
	{
		var rangeRegex = RangeRegex();
		var match = rangeRegex.Match(line);

		if (!match.Success)
		{
			throw new ArgumentException($"Invalid range format: {line}", nameof(line));
		}

		return new MapRange
		{
			SourceStart = long.Parse(match.Groups["SourceStart"].ValueSpan),
			DestinationStart = long.Parse(match.Groups["DestinationStart"].ValueSpan),
			Length = long.Parse(match.Groups["Length"].ValueSpan),
		};
	}

	#endregion Common

	#region Types

	private sealed record Almanac
	{
		public required IReadOnlyList<long> Seeds { get; init; }
		public required IReadOnlyList<Map> Maps { get; init; }
	}

	private sealed record Map
	{
		public required string From { get; init; }
		public required string To { get; init; }
		public required IReadOnlyList<MapRange> Ranges { get; init; }
	}

	private sealed record MapRange
	{
		public required long SourceStart { get; init; }
		public required long DestinationStart { get; init; }
		public required long Length { get; init; }
	}

	[GeneratedRegex(@"(\d+)", RegexOptions.Compiled)]
	private static partial Regex SeedsRegex();

	[GeneratedRegex(@"^(?<From>\w+)-to-(?<To>\w+) map:$", RegexOptions.Compiled)]
	private static partial Regex MapRegex();

	[GeneratedRegex(
		@"^(?<DestinationStart>\d+) (?<SourceStart>\d+) (?<Length>\d+)$",
		RegexOptions.Compiled
	)]
	private static partial Regex RangeRegex();

	#endregion Types
}
