using System.Text.RegularExpressions;

namespace _2022._14;

public static class Day14
{
	private const string InputFileName = "Day14.txt";
	private const string TestInputFileName = "Day14_test.txt";

	public static (object Part1, object Part2) Run(bool isTest)
	{
		var filename = isTest ? TestInputFileName : InputFileName;
		var lines = File.ReadAllLines(System.IO.Path.Combine("14", filename));
		var paths = lines.Select(GetPath).ToList();

		var left = paths.SelectMany(path => path.Segments.Select(coordinate => coordinate.X)).Min();
		var right = paths.SelectMany(path => path.Segments.Select(coordinate => coordinate.X)).Max();
		var bottom = paths.SelectMany(path => path.Segments.Select(coordinate => coordinate.Y)).Max();

		var caveOne = CreateCave(left, right, bottom, paths);

		bottom += 2;
		var totalWidth = ((bottom - 1) * 2) + 1;
		left = 500 - ((totalWidth - 1) / 2);
		right = 500 + ((totalWidth - 1) / 2);

		var caveTwo = CreateCave(
			left,
			right,
			bottom,
			paths.Append(new Path
			{
				Segments = new List<Coordinate> { new(left, bottom), new(right, bottom) }
			}).ToList());

		return (GetPartOne(caveOne), GetPartTwo(caveTwo));
	}

	private static int GetPartOne(Cave cave)
	{
		var coordinate = cave.Entry;
		var sandCount = 0;

		while (true)
		{
			var nextCoordinate = LetTheSandFall(coordinate, cave);

			// Nowhere for the sand to go. Drop more sand!
			if (coordinate == nextCoordinate)
			{
				cave.Tiles[coordinate.Y][coordinate.X] = Tile.Sand;
				coordinate = cave.Entry;
				++sandCount;
				continue;
			}

			// Falling into the abyss forever.
			if (GetTileAt(nextCoordinate, cave) == Tile.Abyss)
			{
				return sandCount;
			}

			// Let the sand fall!
			coordinate = nextCoordinate;
		}
	}

	private static int GetPartTwo(Cave cave)
	{
		var coordinate = cave.Entry;
		var sandCount = 0;

		while (true)
		{
			var nextCoordinate = LetTheSandFall(coordinate, cave);

			if (nextCoordinate == cave.Entry)
			{
				return sandCount + 1;
			}

			// Nowhere for the sand to go. Drop more sand!
			if (coordinate == nextCoordinate || GetTileAt(nextCoordinate, cave) == Tile.Abyss)
			{
				cave.Tiles[coordinate.Y][coordinate.X] = Tile.Sand;
				coordinate = cave.Entry;
				++sandCount;
				continue;
			}

			// Let the sand fall!
			coordinate = nextCoordinate;
		}
	}

	// Does not work. Haven't figured out why. Tries to calculate rather than simulate.
	private static int GetPartTwoFast(Cave cave)
	{
		var sandCount = 1;

		for (var row = 1; row < cave.Tiles.Length - 1; ++row)
		{
			var start = GetStartForRow(row, cave);
			var end = GetEndForRow(row, cave);
			for (var col = start; col <= end; ++col)
			{
				var coordinate = new Coordinate(col, row);

				if (IsFillable(GetTileAt(coordinate, cave)) && IsOverheadOpen(coordinate, cave))
				{
					++sandCount;
				}
			}
		}

		return sandCount;
	}

	private static int GetStartForRow(int row, Cave cave) => GetMiddle(cave) - row;
	private static int GetEndForRow(int row, Cave cave) => GetMiddle(cave) + row;
	private static int GetMiddle(Cave cave) => cave.Tiles[0].Length / 2;

	private static bool IsOverheadOpen(Coordinate coordinate, Cave cave)
	{

		for (var row = coordinate.Y - 1; row >= 0; --row)
		{
			var start = Math.Max(GetStartForRow(row, cave), coordinate.X - (coordinate.Y - row));
			var end = Math.Min(GetEndForRow(row, cave), coordinate.X + (coordinate.Y - row));

			var isRowFillable = false;
			for (var col = start; col <= end && !isRowFillable; ++col)
			{
				if (GetTileAt(new Coordinate(col, row), cave) == Tile.Open)
				{
					isRowFillable = true;
				}
			}

			if (!isRowFillable)
			{
				return false;
			}
		}

		return true;
	}

	private static Coordinate LetTheSandFall(Coordinate coordinate, Cave cave)
	{
		var coordinatesToCheck = new[]
		{
			new Coordinate(coordinate.X, coordinate.Y + 1),
			new Coordinate(coordinate.X - 1, coordinate.Y + 1),
			new Coordinate(coordinate.X + 1, Y: coordinate.Y + 1),
		};

		return coordinatesToCheck.FirstOrDefault(c => IsFillable(GetTileAt(c, cave))) ?? coordinate;
	}

	private static bool IsFillable(Tile tile) => tile switch
	{
		Tile.Open => true,
		Tile.Abyss => true,
		_ => false,
	};

	private static bool IsOutOfRange(Coordinate coordinate, Cave cave) =>
		coordinate.Y < 0
		|| coordinate.Y >= cave.Tiles.Length
		|| coordinate.X < 0
		|| coordinate.X >= cave.Tiles[0].Length;

	private static Tile GetTileAt(Coordinate coordinate, Cave cave) =>
		IsOutOfRange(coordinate, cave) ? Tile.Abyss : cave.Tiles[coordinate.Y][coordinate.X];

	private static Path GetPath(string line)
	{
		return new Path { Segments = line.Split(" -> ").Select(GetCoordinates).ToList() };

		static Coordinate GetCoordinates(string pair)
		{
			var regex = new Regex(@"(?<x>\d+),(?<y>\d+)", RegexOptions.Compiled);
			var match = regex.Match(pair);
			return new Coordinate(
				int.Parse(match.Groups["x"].Value),
				int.Parse(match.Groups["y"].Value));
		}
	}

	private static Cave CreateCave(int left, int right, int bottom, IReadOnlyList<Path> paths)
	{
		var cave = BuildCave(left, right, bottom);
		cave = PopulateCave(cave, paths);

		return cave;

		static Cave BuildCave(int left, int right, int bottom)
		{
			var depth = bottom + 1;
			var length = right - left + 1;

			var cave = new Cave { Tiles = new Tile[depth][], Shift = left };

			for (var row = 0; row < depth; ++row)
			{
				cave.Tiles[row] = Enumerable.Repeat(Tile.Open, length).ToArray();
			}

			return cave;
		}

		static Cave PopulateCave(Cave cave, IReadOnlyList<Path> paths)
		{
			foreach (var path in paths)
			{
				for (var i = 0; i < path.Segments.Count - 1; ++i)
				{
					var coordinates = GetFullPath(path.Segments[i], path.Segments[i + 1]);
					foreach (var coordinate in coordinates)
					{
						cave.Tiles[coordinate.Y][coordinate.X - cave.Shift] = Tile.Rock;
					}
				}
			}

			return cave;
		}

		static IReadOnlyList<Coordinate> GetFullPath(Coordinate a, Coordinate b)
		{
			int min;
			int max;
			if (a.X != b.X)
			{
				min = Math.Min(a.X, b.X);
				max = Math.Max(a.X, b.X);
				return Enumerable.Range(min, max - min + 1).Select(x => new Coordinate(x, a.Y)).ToList();
			}

			min = Math.Min(a.Y, b.Y);
			max = Math.Max(a.Y, b.Y);
			return Enumerable.Range(min, max - min + 1).Select(y => new Coordinate(a.X, y)).ToList();
		}
	}

	private sealed record Coordinate(int X, int Y)
	{
		public override string ToString() => $"{this.X},{this.Y}";
	}

	private sealed record Path
	{
		public required IReadOnlyList<Coordinate> Segments { get; init; }

		public override string ToString() => string.Join(" -> ", Segments.Select(segment => segment.ToString()));
	}

	private sealed record Cave
	{
		public required Tile[][] Tiles { get; init; }
		public required int Shift { get; init; }
		public Coordinate Entry => new(500 - this.Shift, 0);

		public override string ToString()
		{
			return string.Join("\n", this.Tiles.Select((row, rowIndex) => string.Join("", row.Select((tile, colIndex) => GetTileDisplay(rowIndex, colIndex, tile)))));

			string GetTileDisplay(int rowIndex, int colIndex, Tile tile)
			{
				if (new Coordinate(colIndex, rowIndex) == this.Entry)
				{
					return "+";
				}

				return tile switch
				{
					Tile.Open => ".",
					Tile.Rock => "#",
					Tile.Sand => "o",
					Tile.Abyss => "X",
					_ => throw new Exception($"Unrecognized tile: {tile}"),
				};
			}
		}
	}

	private enum Tile
	{
		Open,
		Rock,
		Sand,
		Abyss,
	}
}
