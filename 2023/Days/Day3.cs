using System.Text;
using System.Text.RegularExpressions;
using static _2023.Utils.InputUtils;
using LineNumber = (int Row, int Col, int Num, int Length);

namespace _2023.Days;

public sealed partial class Day3 : IDay
{
	public (string PartOne, string PartTwo) Run(bool isTest)
	{
		var lines = GetAllLines(isTest, 3);
		return (RunPuzzleOne(lines).ToString(), RunPuzzleTwo(lines).ToString());
	}

	#region Puzzle one

	private static int RunPuzzleOne(IList<string> lines) =>
		lines
			.SelectMany((_, row) => GetPartNumbers(row, lines))
			.Select(lineNumber => lineNumber.Num)
			.Sum();

	#endregion Puzzle one

	#region Puzzle two

	private static int RunPuzzleTwo(IList<string> lines)
	{
		var partNumbers = lines
			.SelectMany((_, row) => GetPartNumbers(row, lines))
			.ToLookup(lineNumber => lineNumber.Row);

		return lines.SelectMany((_, row) => GetGearRatios(row, lines, partNumbers)).Sum();
	}

	private static List<int> GetGearRatios(
		int row,
		IList<string> lines,
		ILookup<int, LineNumber> partNumbers
	)
	{
		return lines[row]
			.Select((c, col) => GetGearRatio(c, col, row, lines, partNumbers))
			.Where(ratio => ratio != 0)
			.ToList();
	}

	private static int GetGearRatio(
		char c,
		int col,
		int row,
		IList<string> lines,
		ILookup<int, LineNumber> partNumbers
	)
	{
		if (c != '*')
		{
			return 0;
		}

		var prevNumbers = row == 0 ? [] : partNumbers[row - 1];
		var currNumbers = partNumbers[row];
		var nextNumbers = row == lines.Count - 1 ? [] : partNumbers[row + 1];

		var startCol = Math.Max(col - 1, 0);
		var endCol = Math.Min(col + 1, lines[0].Length - 1);

		var adjacentNumbers = prevNumbers
			.Where(IsPartNumberAdjacent)
			.Concat(currNumbers.Where(IsPartNumberAdjacent))
			.Concat(nextNumbers.Where(IsPartNumberAdjacent))
			.ToList();

		return adjacentNumbers.Count == 2 ? adjacentNumbers[0].Num * adjacentNumbers[1].Num : 0;

		bool IsPartNumberAdjacent(LineNumber partNumber)
		{
			var columns = Enumerable.Range(partNumber.Col, partNumber.Length);
			return columns.Intersect([startCol, col, endCol]).Any();
		}
	}

	#endregion Puzzle two

	private static List<LineNumber> GetPartNumbers(int row, IList<string> lines)
	{
		var line = lines[row];
		var previousLine = row == 0 ? null : lines[row - 1];
		var nextLine = row == lines.Count - 1 ? null : lines[row + 1];

		var lineNumbers = GetLineNumbers(row, line);
		return lineNumbers
			.Where(
				lineNumber =>
					IsPartNumber(lineNumber.Col, lineNumber.Length, previousLine, line, nextLine)
			)
			.ToList();
	}

	private static List<LineNumber> GetLineNumbers(int row, string line)
	{
		var regex = LineNumbersRegex();
		var matches = regex.Matches(line);

		return matches
			.Select(match =>
			{
				var capture = match.Captures.First();
				var value = capture.Value;
				return (row, capture.Index, int.Parse(value), value.Length);
			})
			.ToList();
	}

	private static bool IsPartNumber(
		int col,
		int numLength,
		string? prev,
		string curr,
		string? next
	)
	{
		var regex = SymbolRegex();
		var unNormalizedStart = col - 1;
		var start = Math.Max(0, unNormalizedStart);
		var end = Math.Min(curr.Length - 1, unNormalizedStart + numLength + 1);

		var length = end - start + 1;

		var builder = new StringBuilder();
		builder.Append(prev is null ? "" : prev.Substring(start, length));
		builder.Append(next is null ? "" : next.Substring(start, length));
		builder.Append($"{curr[start]}{curr[end]}");

		var symbols = builder.ToString();
		return regex.IsMatch(symbols);
	}

	[GeneratedRegex(@"(\d+)", RegexOptions.Compiled)]
	private static partial Regex LineNumbersRegex();

	[GeneratedRegex(@"[^\.\d]", RegexOptions.Compiled)]
	private static partial Regex SymbolRegex();
}
