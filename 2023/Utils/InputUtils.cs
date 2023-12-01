namespace _2023.Utils;

public static class InputUtils
{
	private const string InputDir = "Inputs";
	private const string InputFilenameBase = "input";
	private const string TestInputFilenameBase = "test_input";

	public static string GetInputFilename(bool isTest, int day)
	{
		var filenameBase = isTest ? TestInputFilenameBase : InputFilenameBase;
		var filenameNumber = day.ToString("00");
		return Path.Combine(InputDir, $"{filenameNumber}_{filenameBase}.txt");
	}
}
