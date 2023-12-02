namespace _2023.Utils;

public static class InputUtils
{
	private const string InputDir = "Inputs";
	private const string InputFilenameBase = "input";
	private const string TestInputFilenameBase = "test_input";

	public static string GetInputFilename(bool isTest, int day, int? testPart = null)
	{
		var filenameBase = isTest ? TestInputFilenameBase : InputFilenameBase;
		var filenameNumber = day.ToString("00");
		var filenamePart = isTest && testPart is not null ? $"_{testPart}" : "";
		return Path.Combine(InputDir, $"{filenameNumber}_{filenameBase}{filenamePart}.txt");
	}

	public static string[] GetAllLines(bool isTest, int day, int? testPart = null) =>
		File.ReadAllLines(GetInputFilename(isTest, day, testPart));
}
