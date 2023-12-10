namespace _2023.Utils;

public static class StringExtensions
{
	/// <summary>
	/// Determines whether a string does not contain a given character.
	/// </summary>
	/// <param name="source">The string to check.</param>
	/// <param name="c">The character to search for.</param>
	/// <returns>
	/// <see langword="true" /> if the string doesn't contain the character; <see langword="false" /> if it does.
	/// </returns>
	public static bool DoesNotContain(this string source, char c) => !source.Contains(c);
}
