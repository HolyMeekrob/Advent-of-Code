using static _2022.FunctionalUtils;

namespace _2022._12;

public static class Day12
{
	private const string InputFileName = "Day12.txt";
	private const string TestInputFileName = "Day12_test.txt";

	private static readonly int CharDiff = 'a' - 1;

	public static (object Part1, object Part2) Run(bool isTest)
	{
		var filename = isTest ? TestInputFileName : InputFileName;
		var lines = File.ReadAllLines(Path.Combine("12", filename));
		var grid = CreateGrid(lines);

		return (GetPartOne(grid), GetPartTwo(grid));
	}

	private static int GetPartOne(Grid grid)
	{
		var state = CreateState(grid, false);
		return GetFewestSteps(state);
	}

	private static int GetPartTwo(Grid grid)
	{
		var state = CreateState(grid, true);
		return GetFewestSteps(state);
	}

	private static int GetFewestSteps(State state)
	{
		while (true)
		{
			var nextPositions = state.Positions
				.SelectMany(position => GetNextPositions(position, state))
				.Where(Complement<Coordinate>(state.Distances.ContainsKey))
				.ToList();

			state.Distance += 1;

			if (!state.IsReversed && nextPositions.Contains(state.Grid.End))
			{
				return state.Distance;
			}

			if (state.IsReversed && nextPositions.Any(pos => GetElevation(pos, state.Grid) == 1))
			{
				return state.Distance;
			}

			foreach (var position in nextPositions)
			{
				state.Distances[position] = state.Distance;
			}

			state.Positions = nextPositions.ToHashSet();
		}
	}

	private static IReadOnlyList<Coordinate> GetNextPositions(Coordinate position, State state)
	{
		var grid = state.Grid;
		var currentElevation = GetElevation(position, grid);

		return new List<Coordinate>
			{
				position with { Row = position.Row + 1 },
				position with { Row = position.Row - 1 },
				position with { Col = position.Col + 1 },
				position with { Col = position.Col - 1 },
			}.Where(All<Coordinate>(IsInsideGrid, IsReachable))
			.ToList();

		bool IsInsideGrid(Coordinate coordinate) =>
			coordinate.Row >= 0
			&& coordinate.Row < grid.Squares.Length
			&& coordinate.Col >= 0
			&& coordinate.Col < grid.Squares[0].Length;

		bool IsReachable(Coordinate coordinate)
		{
			var elevation = GetElevation(coordinate, grid);
			return state.IsReversed
				? elevation >= currentElevation - 1
				: elevation <= currentElevation + 1;
		}
	}

	private static State CreateState(Grid grid, bool isReversed) =>
		new()
		{
			Grid = grid,
			Distance = 0,
			Distances = new Dictionary<Coordinate, int> { { isReversed ? grid.End : grid.Start, 0 } },
			Positions = new HashSet<Coordinate> { isReversed ? grid.End : grid.Start },
			IsReversed = isReversed
		};

	private static Grid CreateGrid(IReadOnlyList<string> lines)
	{
		var squares = new Square[lines.Count][];
		var start = new Coordinate(0, 0);
		var end = new Coordinate(0, 0);

		for (var row = 0; row < squares.Length; ++row)
		{
			var line = lines[row];
			squares[row] = new Square[line.Length];
			for (var col = 0; col < line.Length; ++col)
			{
				var elevation = line[col];
				squares[row][col] = CreateSquare(elevation);
				if (elevation == 'S')
				{
					start = new Coordinate(row, col);
				}
				else if (elevation == 'E')
				{
					end = new Coordinate(row, col);
				}
			}
		}

		return new Grid { Squares = squares, Start = start, End = end, };
	}

	private static Square CreateSquare(char elevation)
	{
		if (elevation == 'S')
		{
			elevation = 'a';
		}

		if (elevation == 'E')
		{
			elevation = 'z';
		}
		return new Square { Elevation = elevation - CharDiff };
	}

	private static int GetElevation(Coordinate coordinate, Grid grid) =>
		grid.Squares[coordinate.Row][coordinate.Col].Elevation;

	private record Coordinate(int Row, int Col);

	private record Square
	{
		public int Elevation { get; init; }
	}

	private record Grid
	{
		public required Square[][] Squares { get; init; }
		public required Coordinate Start { get; init; }
		public required Coordinate End { get; init; }

		public override string ToString()
		{
			return string.Join("\n", this.Squares.Select(GetRow));

			string GetRow(Square[] row, int rowIndex) =>
				string.Join("", row.Select((square, colIndex) => GetSquare(square, rowIndex, colIndex)));

			char GetSquare(Square square, int rowIndex, int colIndex)
			{
				var coordinate = new Coordinate(rowIndex, colIndex);
				var elevation = (char)(square.Elevation + CharDiff);

				if (this.Start == coordinate)
				{
					elevation = 'S';
				}

				if (this.End == coordinate)
				{
					elevation = 'E';
				}

				return elevation;
			}
		}
	}

	private record State
	{
		public required Grid Grid { get; init; }
		public required int Distance { get; set; }
		public required HashSet<Coordinate> Positions { get; set; }
		public required Dictionary<Coordinate, int> Distances { get; init; }
		public required bool IsReversed { get; init; }
	}
}
