using System.Text.RegularExpressions;

namespace _2022._08;

public static class Day8
{
	private const string InputFileName = "Day8.txt";
	private const string TestInputFileName = "Day8_Test.txt";

	public static (object Part1, object Part2) Run(bool isTest)
	{
		var filename = isTest ? TestInputFileName : InputFileName;
		var lines = File.ReadAllLines(Path.Combine("08", filename));
		var grid = CreateGrid(lines);

		return (GetPartOne(grid), GetPartTwo(grid));
	}

	private static int[][] CreateGrid(IReadOnlyList<string> lines) =>
		lines.Select(line => line.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();

	private static int GetPartOne(int[][] grid)
	{
		var count = 0;
		for (var row = 0; row < grid.Length; ++row)
		{
			for (var col = 0; col < grid[row].Length; ++col)
			{
				count = IsVisible(grid, row, col) ? count + 1 : count;
			}
		}

		return count;
	}

	private static int GetPartTwo(int[][] grid)
	{
		var maxScore = 0;
		for (var row = 0; row < grid.Length; ++row)
		{
			for (var col = 0; col < grid[row].Length; ++col)
			{
				maxScore = Math.Max(maxScore, CalculateScore(grid, row, col));
			}
		}

		return maxScore;
	}

	private static bool IsVisible(int[][] grid, int row, int col) =>
		IsVisibleLeft(grid, row, col)
			|| IsVisibleRight(grid, row, col)
			|| IsVisibleUp(grid, row, col)
			|| IsVisibleDown(grid, row, col);

	private static bool IsVisibleLeft(int[][] grid, int row, int col)
	{
		var value = grid[row][col];

		while (col - 1 > -1)
		{
			if (grid[row][col - 1] >= value)
			{
				return false;
			}

			--col;
		}

		return true;
	}

	private static bool IsVisibleRight(int[][] grid, int row, int col)
	{
		var value = grid[row][col];

		while (col + 1 < grid[row].Length)
		{
			if (grid[row][col + 1] >= value)
			{
				return false;
			}

			++col;
		}

		return true;
	}

	private static bool IsVisibleUp(int[][] grid, int row, int col)
	{
		var value = grid[row][col];

		while (row - 1 > -1)
		{
			if (grid[row - 1][col] >= value)
			{
				return false;
			}

			--row;
		}

		return true;
	}

	private static bool IsVisibleDown(int[][] grid, int row, int col)
	{
		var value = grid[row][col];

		while (row + 1 < grid.Length)
		{
			if (grid[row + 1][col] >= value)
			{
				return false;
			}

			++row;
		}

		return true;
	}

	private static int CalculateScore(int[][] grid, int row, int col) =>
		CalculateDistanceLeft(grid, row, col)
		* CalculateDistanceRight(grid, row, col)
		* CalculateDistanceUp(grid, row, col)
		* CalculateDistanceDown(grid, row, col);

	private static int CalculateDistanceLeft(int[][] grid, int row, int col)
	{
		var value = grid[row][col];
		var distance = 0;

		while (col - 1 > -1)
		{
			distance++;
			if (grid[row][col - 1] >= value)
			{
				break;
			}

			--col;
		}

		return distance;
	}

	private static int CalculateDistanceRight(int[][] grid, int row, int col)
	{
		var value = grid[row][col];
		var distance = 0;

		while (col + 1 < grid[row].Length)
		{
			distance++;
			if (grid[row][col + 1] >= value)
			{
				break;
			}

			++col;
		}

		return distance;
	}

	private static int CalculateDistanceUp(int[][] grid, int row, int col)
	{
		var value = grid[row][col];
		var distance = 0;

		while (row - 1 > -1)
		{
			distance++;
			if (grid[row - 1][col] >= value)
			{
				break;
			}

			--row;
		}

		return distance;
	}

	private static int CalculateDistanceDown(int[][] grid, int row, int col)
	{
		var value = grid[row][col];
		var distance = 0;

		while (row + 1 < grid.Length)
		{
			distance++;
			if (grid[row + 1][col] >= value)
			{
				break;
			}

			++row;
		}

		return distance;
	}
}
