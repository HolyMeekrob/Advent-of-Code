namespace _2023.Days;
using static Utils.InputUtils;

public sealed class Day1 : IDay
{
	public (string PartOne, string PartTwo) Run(bool isTest) => (RunPuzzleOne(isTest).ToString(), RunPuzzleTwo(isTest));

	#region Puzzle one
	
	private static int RunPuzzleOne(bool isTest) =>
		GetLines(isTest)
			.Select(GetNumber)
			.Sum();

	private static int GetNumber(string line) => GetFirstNumber(line) * 10 + GetLastNumber(line);

	private static int GetFirstNumber(string line) => line.First(char.IsDigit) - '0';
	private static int GetLastNumber(string line) => line.Last(char.IsDigit) - '0';
	
	#endregion Puzzle one

	#region Puzzle two

	private static string RunPuzzleTwo(bool isTest) => "Not implemented";

	#endregion Puzzle two

	private static string[] GetLines(bool isTest) => File.ReadAllLines(GetInputFilename(isTest, 1));
}

