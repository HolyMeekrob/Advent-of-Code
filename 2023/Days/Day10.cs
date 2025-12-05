using static _2023.Utils.InputUtils;
using Point = (int Row, int Col);

namespace _2023.Days;

public sealed class Day10 : IDay
{
	public (string PartOne, string PartTwo) Run(bool isTest)
	{
		var lines = GetAllLines(isTest, 10, isTest ? 1 : null);
		return (RunPuzzleOne(lines).ToString(), RunPuzzleTwo(lines).ToString());
	}

	private static int RunPuzzleOne(IReadOnlyList<string> lines)
	{
		var map = lines.Select(CreateRow).ToList();
		var start = GetStartPosition(map);
	}

	private static Point GetStartPosition(IReadOnlyList<IReadOnlyList<Tile>> map)
	{
		for (var row = 0; row < map.Count; ++row)
		{
			for (var col = 0; col < map[row].Count; ++col)
			{
				if (map[row][col] == Tile.Start)
				{
					return (row, col);
				}
			}
		}

		throw new Exception("Map doesn't contain a start");
	}

	private static List<Tile> CreateRow(string line)
	{
		return line.Select(GetTile).ToList();
	}

	private static Tile GetTile(char c) =>
		c switch
		{
			'|' => Tile.NorthSouth,
			'-' => Tile.EastWest,
			'L' => Tile.NorthEast,
			'J' => Tile.NorthWest,
			'F' => Tile.SouthEast,
			'7' => Tile.SouthWest,
			'.' => Tile.Ground,
			'S' => Tile.Start,
			_ => throw new ArgumentOutOfRangeException(nameof(c)),
		};
	
	private static int RunPuzzleTwo(IReadOnlyList<string> lines)
	{
		return 0;
	}

	private enum Tile
	{
		NorthSouth,
		EastWest,
		NorthEast,
		NorthWest,
		SouthEast,
		SouthWest,
		Ground,
		Start,
	}
}